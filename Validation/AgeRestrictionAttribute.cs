using System.ComponentModel.DataAnnotations;

namespace OnlineBalance.Validation
{
    public class AgeRestrictionAttribute : ValidationAttribute
    {
        private int _minAge;

        public int MinAge => _minAge;

        public AgeRestrictionAttribute(int minAge = 0)
        {
            _minAge = minAge;
        }

        public override bool IsValid(object? value)
        {
            DateTime? valueDt = DateTime.Parse(value?.ToString()!);
            var nowDt = DateTime.Now;

            if (value == null || valueDt >= nowDt)
            {
                return false;
            }

            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan dateDifference = (TimeSpan) (DateTime.Now - valueDt);
            int year = (zeroTime + dateDifference).Year - 1;

            return year >= MinAge;
        }
    }
}
