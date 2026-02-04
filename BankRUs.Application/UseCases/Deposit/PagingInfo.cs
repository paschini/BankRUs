using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Deposit
{
    public record PagingInfo
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages { get; init; }
    }
}
