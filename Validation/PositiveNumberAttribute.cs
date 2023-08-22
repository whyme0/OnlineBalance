using System.ComponentModel.DataAnnotations;

namespace OnlineBalance.Validation
{
    public class PositiveNumberAttribute : ValidationAttribute
    {
        protected string errorMessage;
        public PositiveNumberAttribute(string ErrorMessage = "Invalid number") => errorMessage = ErrorMessage;
        public string GetErrorMessage() => errorMessage;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            double amount = (double)value;

            if (amount <= 0)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}
