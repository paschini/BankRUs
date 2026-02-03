using BankRUs.Application.Repositories;
using BankRUs.Intrastructure.Persistance;
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
    }
}
