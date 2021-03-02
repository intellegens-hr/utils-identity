using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Commons.Validation
{
    public static class ModelValidator
    {
        public static (bool isValid, ICollection<ValidationResult> validationResults) ValidateDataAnnotations<T>(T instanceToValidate)
        {
            var context = new ValidationContext(instanceToValidate);

            if (instanceToValidate is IValidatableObject validatableObject)
                validatableObject.Validate(context);

            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(instanceToValidate, context, results, true);

            return (isValid, results);
        }
    }
}