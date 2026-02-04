using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Withdrawal
{
    public record WithdrawalResult
    {
        public Guid TransactionId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "SEK";
        public string Reference { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal BalanceAfter { get; set; }
    }
}
