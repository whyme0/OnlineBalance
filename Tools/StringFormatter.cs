using OnlineBalance.Models;

namespace OnlineBalance.Tools
{
    public static class StringFormatter
    {
        public static string ReadableAccountMoney(double money, CurrencyType currencyType)
        {
            string currency = string.Empty;
            switch (currencyType)
            {
                case CurrencyType.RUB:
                    currency = "₽";
                    break;
                case CurrencyType.USD:
                    currency = "$";
                    break;
                case CurrencyType.UAH:
                    currency = "₴";
                    break;
                case CurrencyType.EUR:
                    currency = "€";
                    break;
            }
            return $"{money} {currency}";
        }
        public static string ReadableAccountNumber(long number)
        {
            string n = number.ToString();
            return $"{n.Substring(0, 4)} {n.Substring(4, 4)} {n.Substring(8, 4)} {n.Substring(12, 4)}";
        }
        public static string ReadableAccountNumber(string number)
        {
            var n = number;
            return $"{n.Substring(0, 4)} {n.Substring(4, 4)} {n.Substring(8, 4)} {n.Substring(12, 4)}";
        }
    }
}
