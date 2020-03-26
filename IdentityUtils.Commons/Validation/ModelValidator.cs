using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Commons.Validation
{
    public static class ModelValidator
    {
        public static (bool isValid, List<ValidationResult> validationResults) ValidateDataAnnotations<T>(T instanceToValidate)
        {
            var context = new ValidationContext(instanceToValidate);
            List<ValidationResult> results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(instanceToValidate, context, results, true);

            return (isValid, results);
        }
    }
}