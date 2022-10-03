using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace drones_api.Validators
{
    public class ListStringValidator : ValidationAttribute
    {
        private readonly IEnumerable<string> _values;

        public ListStringValidator(string[] values)
        {
            _values = values.Select(x => x.ToString());
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (_values != null && !_values.Contains(value.ToString()))
            {
                var valuesString = string.Join(", ", _values.Select(x => $"'{x}'"));
                var message = $"valor {value} no es valido. Valores validos son {valuesString}.";
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }
    }
}
