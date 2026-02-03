using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public class BankAccountTransaction
    {
        public Guid Id { get; set; }
        public Guid BankAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
        public string TransactionType { get; set; }
        public string TransactionCurrency { get; set; }

        [MaxLength(140)]
        public string Reference { get; set; }
    }
}
