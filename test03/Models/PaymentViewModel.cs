using System;
using System.ComponentModel.DataAnnotations;

namespace test03.Models
{
    public class PaymentViewModel
    {
        [Required]
        public int ReservationID { get; set; } // Link to the reservation

        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; } // Payment amount

        [Required]
        public string PaymentMethod { get; set; } // E.g., Credit Card, PayPal

        [Required]
        [StringLength(16, MinimumLength = 13, ErrorMessage = "Card number must be between 13 and 16 digits.")]
        public string CardNumber { get; set; } // For card payments

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$", ErrorMessage = "Invalid expiration date format.")]
        public string CardExpiry { get; set; } // MM/YY format

        [Required]
        [StringLength(3, ErrorMessage = "CVC must be 3 digits.")]
        public string CardCVC { get; set; } // Security code

        [Required]
        public int CustomerID { get; set; } // Add this property for linking customer

        [Required]
        public DateTime ReservationDate { get; set; } // Add this property for linking customer

        [Required]
        public DateTime ReservationTime { get; set; } // Add this property for linking customer

        [Required]
        public int NumberOfGuests { get; set; } // Add this property for linking customer

        [Required]
        public int TableNumber { get; set; } // Add this property for linking customer

        [Required]
        public int PaymentAmount { get; set; } // Add this property for linking customer
    }
}

