using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Transactions
{
    public record TransactionsQuery
    {
        public Guid UserId { get; init; }
        public Guid BankAccountId { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
        public string Type { get; init; }
        public string Sort { get; init; }
    }
}
