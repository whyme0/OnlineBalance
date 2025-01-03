using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBalance.Tools;
using OnlineBalance.Data;
using OnlineBalance.Managers;
using OnlineBalance.Models;
using OnlineBalance.Tools;
using OnlineBalance.Models.ViewModels;

namespace OnlineBalance.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public AccountController(UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [NonAction]
        private async Task<User> GetAuthorizedUser() => await _userManager.FindByNameAsync(User.Identity.Name);

        [Authorize]
        [HttpGet]
        [Route("/accounts")]
        public async Task<IActionResult> List()
        {
            var user = await GetAuthorizedUser();
            List<Account> accounts = await _dbContext.Accounts.Where(a => a.User == user).ToListAsync();
            ViewData["User"] = user;
            return View(accounts);
        }

        [Authorize]
        [HttpGet]
        [Route("/accounts/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            Account? account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == id);
            
            if (account == null || await GetAuthorizedUser() != account.User)
                return NotFound();

            ViewData["AccountOperations"] = await _dbContext.Operations.Where(o => o.SenderNumber == account.Number || o.RecipientNumber == account.Number).OrderByDescending(x => x.Date).ToListAsync();
            return View(account);
        }

        [Authorize]
        [HttpGet]
        [Route("/accounts/create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("/accounts/create")]
        public async Task<IActionResult> Create([FromForm] CreateAccountDTO accountDTO)
        {
            if (!ModelState.IsValid)
                return View(accountDTO);

            var user = await GetAuthorizedUser();
            Account account = new Account() { Balance = 0, Currency = accountDTO.Currency, User = user };
            account.Number = new Random().NextInt64(1000000000000000, 9999999999999999);
            
            user.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            TempData["AccountCreatedOk"] = "New bank account was opened successfully";
            return RedirectToAction("List");
        }

        [Authorize]
        [HttpGet]
        [Route("/accounts/{id}/send")]
        public async Task<IActionResult> TransferMoney(long id)
        {
            Account? senderAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == id);
            if (senderAccount == null || await GetAuthorizedUser() != senderAccount.User)
                return NotFound();

            ViewData["SenderAccount"] = senderAccount;
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("/accounts/{id}/send")]
        public async Task<IActionResult> TransferMoney(long id, [FromForm] TransferMoneyDTO transferMoneyDTO)
        {
            Account? senderAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == id);
            if (senderAccount == null || await GetAuthorizedUser() != senderAccount.User)
                return NotFound();
            ViewData["SenderAccount"] = senderAccount;
            
            if (!ModelState.IsValid)
                return View(transferMoneyDTO);
            
            long recipAccNum = Convert.ToInt64(transferMoneyDTO.RecipientAccountNumber.ToString().Replace(" ", ""));
            Account? recipientAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == recipAccNum);

            if (senderAccount.Balance < transferMoneyDTO.Amount)
            {
                ModelState.TryAddModelError("Amount", "Not enough money to make a transfer");
            }
            else if(recipientAccount == null)
            {
                ModelState.TryAddModelError("RecipientAccountNumber", "Account not found");
            }
            if (recipientAccount?.Currency != senderAccount.Currency)
            {
                ModelState.TryAddModelError("RecipientAccountNumber", $"Cannot transfer money from {senderAccount.Currency} to {recipientAccount?.Currency}");
            }
            if (recipientAccount == senderAccount)
            {
                ModelState.TryAddModelError("RecipientAccountNumber", "The recipient's account cannot be the sender's account.");
            }
            if (!ModelState.IsValid)
            {
                return View(transferMoneyDTO);
            }

            TemporaryOperation temporaryOperation = new() {
                Id = Guid.NewGuid(),
                Amount = transferMoneyDTO.Amount,
                Description = transferMoneyDTO.Description,
                SenderNumber = senderAccount.Number,
                RecipientNumber = recipientAccount.Number,
            };

            await _dbContext.TemporaryOperations.AddAsync(temporaryOperation);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ConfirmMoneyTransfer", new { id = temporaryOperation.Id });
        }

        [Authorize]
        [HttpGet]
        [Route("/accounts/confirm-money-transfer/{id}")]
        public async Task<IActionResult> ConfirmMoneyTransfer(Guid id)
        {
            TemporaryOperation? temporaryOperation = await _dbContext.TemporaryOperations.FirstOrDefaultAsync(t => t.Id == id);
            Account senderAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == temporaryOperation.SenderNumber);
            Account recipientAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == temporaryOperation.RecipientNumber);

            User recipientUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Accounts.Contains(recipientAccount));
            var usr = await GetAuthorizedUser();

            if (temporaryOperation == null || senderAccount.User != usr)
                return NotFound();

            // https://localhost:7282/accounts/confirm-money-transfer/54dc5bbe-e658-42ad-8177-c060c38ef096
            ViewData["tempOperation"] = temporaryOperation;
            ViewData["RecipientFullName"] = recipientUser.FullName;
            ViewData["Currency"] = recipientAccount.Currency;

            
            return View("TransferMoneyConfirmation", temporaryOperation);
        }

        [Authorize]
        [HttpPost]
        [Route("/accounts/confirm-money-transfer/{id}")]
        public async Task<IActionResult> ConfirmMoneyTransferPOST(Guid id)
        {
            TemporaryOperation? temporaryOperation = await _dbContext.TemporaryOperations.FirstOrDefaultAsync(t => t.Id == id);
            if (temporaryOperation == null)
                return NotFound();
            
            var usr = await GetAuthorizedUser();

            // sender and recipient wont validate for null here, due to null validation at TransferMoney method
            Account senderAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == temporaryOperation.SenderNumber);
            Account recipientAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == temporaryOperation.RecipientNumber);

            if (temporaryOperation == null || senderAccount.User != usr)
                return NotFound();

            var operation = await AccountManager.TransferMoney(temporaryOperation, _dbContext);

            _dbContext.TemporaryOperations.Remove(temporaryOperation);
            await _dbContext.Operations.AddAsync(operation);
            await _dbContext.SaveChangesAsync();

            return Content(@$"Money amount of 
                {StringFormatter.ReadableAccountMoney(temporaryOperation.Amount, senderAccount.Currency)}
                was successfully sent to recipient account with number
                {StringFormatter.ReadableAccountNumber(recipientAccount.Number)}");
        }

        [Authorize]
        [HttpGet]
        [Route("/accounts/reject-money-transfer/{id}")]
        public async Task<IActionResult> RejectMoneyTransfer(Guid id)
        {
            TemporaryOperation? tempOperation = await _dbContext.TemporaryOperations.FindAsync(id);
            var usr = await GetAuthorizedUser();
            Account? senderAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == tempOperation.SenderNumber);

            if (tempOperation == null || senderAccount?.User != usr)
                return NotFound();

            _dbContext.Remove(tempOperation);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("TransferMoney", new { id = senderAccount.Id });
        }

        [Authorize]
        [HttpGet]
        [Route("/accounts/gift")]
        public async Task<IActionResult> MoneyGift()
        {
            var usr = await GetAuthorizedUser();
            List<Account> accounts = await _dbContext.Accounts.Where(a => a.User == usr).ToListAsync();

            ViewData["IsGotGift"] = usr.IsGotMoneyGift;
            MoneyGiftViewModel viewModel = new MoneyGiftViewModel() { Accounts = accounts };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [Route("/accounts/gift/")]
        public async Task<IActionResult> MoneyGift([FromForm] MoneyGiftViewModel moneyGiftViewModel)
        {
            Account? account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == moneyGiftViewModel.AccountId);
            var usr = await GetAuthorizedUser();
            
            if (account == null || account.User != usr)
                return NotFound();


            account.Balance += 1000;
            usr.IsGotMoneyGift = true;
            await _dbContext.SaveChangesAsync();

            TempData["MoneyGiftProceededSuccessfully"] = $"You got a money gift in amount of {StringFormatter.ReadableAccountMoney(1000, account.Currency)}!";

            return RedirectToAction("Details", new { id = account.Number });
        }
    }
}
