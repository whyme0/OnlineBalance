using Microsoft.EntityFrameworkCore;
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
    }
}
