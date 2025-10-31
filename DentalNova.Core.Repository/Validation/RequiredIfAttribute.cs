using System.ComponentModel.DataAnnotations;

namespace DentalNova.Core.Repository.Validation
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object _expectedValue;

        public RequiredIfAttribute(string propertyName, object expectedValue)
        {
            _propertyName = propertyName;
            _expectedValue = expectedValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var propertyValue = type.GetProperty(_propertyName).GetValue(instance, null);

            if (propertyValue.ToString() == _expectedValue.ToString() && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
