using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class PaymentInfo
    {
        [Required(ErrorMessage = "Card number is required.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Please enter a valid 16-digit credit card number.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Holder name is required.")]
        public string HolderName { get; set; }

        [Required(ErrorMessage = "Expiration date is required.")]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Expiration date cannot be in the past.")]

        public DateTime ExpireOnDate { get; set; }

        [Required(ErrorMessage = "CSV is required.")]
        [Range(100, 9999, ErrorMessage = "CSV must be a 3 or 4 digit number.")]
        public int CSV { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateTime = (DateTime)value;
            if (dateTime < DateTime.Today)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }

}
