using BankRUs.Application.Repositories;
using BankRUs.Domain.Entities;
using BankRUs.Intrastructure.Persistance;

namespace BankRUs.Intrastructure.Repositories;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly ApplicationDbContext _db;
    public BankAccountRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Add(BankAccount bankAccount)
    {
        _db.BankAccounts.Add(bankAccount);
        await _db.SaveChangesAsync();
    }

    public async Task<BankAccount?> GetByAccountNumber(Guid accountNumber)
    {
        return await _db.BankAccounts.FindAsync(accountNumber);
    }

    public async Task UpdateBalance(BankAccount bankAccount)
    {
        _db.BankAccounts.Update(bankAccount);
        await _db.SaveChangesAsync();
    }
}
