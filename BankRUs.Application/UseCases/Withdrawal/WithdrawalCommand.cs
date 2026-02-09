using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Withdrawal
{
    public class WithdrawalCommand
    {
        public Guid AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string? Currency { get; set; } = "SEK";
        public Guid UserId { get; set; }
    }
}
