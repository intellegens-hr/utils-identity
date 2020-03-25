using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Api.Extensions.Cli.Utils
{
    internal class GuidValidatorAttribute : ValidationAttribute
    {
        public GuidValidatorAttribute()
            : base("The value {0} must be a valid Guid")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var isValidGuid = Guid.TryParse(value.ToString(), out Guid result);
            if (value != null && !isValidGuid)
            {
                return new ValidationResult(FormatErrorMessage(value.ToString()));
            }

            return ValidationResult.Success;
        }
    }
}