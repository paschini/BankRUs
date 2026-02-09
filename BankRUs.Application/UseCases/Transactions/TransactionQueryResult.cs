using BankRUs.Application.UseCases.Deposit;
using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Transactions
{
    public record TransactionQueryResult
    {
        public Guid AccountId { get; init; }
        public string Currency { get; init; } = "SEK";
        public decimal Balance { get; init; }
        public PagingInfo Paging { get; init; } = new PagingInfo();
        public List<BankAccountTransaction> Items { get; init; } = new List<BankAccountTransaction>();
    }
}
