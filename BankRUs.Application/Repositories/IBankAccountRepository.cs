using BankRUs.Domain.Entities;

namespace BankRUs.Application.Repositories;

public interface IBankAccountRepository
{
    Task Add(BankAccount bankAccount);

    Task<BankAccount?> GetByAccountNumber(Guid accountNumber);

    Task UpdateBalance(BankAccount bankAccount);
}
