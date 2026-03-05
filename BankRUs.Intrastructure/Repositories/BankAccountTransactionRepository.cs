using BankRUs.Application.Repositories;
using BankRUs.Application.UseCases.Deposit;
using BankRUs.Application.UseCases.Transactions;
using BankRUs.Domain.Entities;
using BankRUs.Intrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Intrastructure.Repositories
{
    public class BankAccountTransactionRepository : IBankAccountTransactionRepository
    {
        private readonly ApplicationDbContext _db;
        public BankAccountTransactionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Add(Domain.Entities.BankAccountTransaction transaction)
        {
            _db.BankAccountTransactions.Add(transaction);
            await _db.SaveChangesAsync();
        }

        public async Task<TransactionQueryResult> GetTransactionsAsync(TransactionsQuery query)
        {
            var baseQuery = _db.BankAccountTransactions
                .Where(t => t.BankAccountId == query.BankAccountId);

            if (query.From.HasValue) baseQuery = baseQuery.Where(t => t.TransactionDate >= query.From.Value);
            if (query.To.HasValue) baseQuery = baseQuery.Where(t => t.TransactionDate <= query.To.Value);
            if (!string.IsNullOrEmpty(query.Type)) baseQuery = baseQuery.Where(t => t.TransactionType == query.Type);

            var totalCount = await baseQuery.CountAsync();

            baseQuery = query.Sort?.ToLower() == "desc"
                ? baseQuery.OrderBy(t => t.TransactionDate)
                : baseQuery.OrderByDescending(t => t.TransactionDate);

            var items = await baseQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new TransactionQueryResult
            {
                AccountId = query.BankAccountId,
                Balance = await _db.BankAccounts
                    .Where(a => a.Id == query.BankAccountId)
                    .Select(a => a.Balance)
                    .FirstAsync(),
                Paging = new PagingInfo
                {
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalCount = totalCount
                },
                Items = items
            };
        }
    }
}
