using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeMarket.Model
{
    public class BuyCurrencyOrder
    {
        /// <summary>
        /// User who makes the transaction (provided he is able to)
        /// </summary>
        [Required]
        [RegularExpression("^[a-zA-Z0-9-]*$", ErrorMessage = "The field UserId can only have alphanumeric values and dashes(-)")]
        [StringLength(50)]
        public string UserId { get; set; }

        /// <summary>
        /// Amount: Expressed in argentinian pesos
        /// </summary>
        [Required]
        [Column(TypeName= "decimal(13,4)")]
        [Range(0.01, 999999999.9999, ErrorMessage = "Invalid amount, the number must be between 1 and 999999999.9999")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency: This will be the currency to buy
        /// </summary>
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "The provided currency is not ISO Formatted")]
        public string Currency { get; set; }
    }
}
