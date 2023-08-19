using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using OnlineBalance.Data;
using OnlineBalance.Models;

namespace OnlineBalance.Managers
{
    public class AccountManager
    {

        public static Operation TransferMoney(Account senderAccount, Account recipientAccount, double moneyAmount, string? description)
        {
            senderAccount.Balance = senderAccount.Balance - moneyAmount;
            recipientAccount.Balance = recipientAccount.Balance + moneyAmount;

            Operation operation = new() {
                SenderNumber = senderAccount.Number,
                RecipientNumber = recipientAccount.Number,
                Amount = moneyAmount,
                Description = description,
                Date = DateTime.UtcNow
            };

            return operation;
        }

        public static async Task<Operation> TransferMoney(TemporaryOperation tempOperation, ApplicationDbContext dbContext)
        {
            var senderAccount = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == tempOperation.SenderNumber);
            var recipientAccount = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == tempOperation.RecipientNumber);

            senderAccount.Balance = senderAccount.Balance - tempOperation.Amount;
            recipientAccount.Balance = recipientAccount.Balance + tempOperation.Amount;

            return new Operation()
            {
                SenderNumber = senderAccount.Number,
                RecipientNumber = recipientAccount.Number,
                Amount = tempOperation.Amount,
                Description = tempOperation.Description,
                Date = DateTime.UtcNow
            };
        }
    }
}
