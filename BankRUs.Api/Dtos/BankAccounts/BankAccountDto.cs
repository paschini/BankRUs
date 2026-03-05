namespace BankRUs.Api.Dtos.BankAccounts;

public record BankAccountDto(
    Guid Id,
    string AccountNumber,
    string Name,
    bool IsLocked,
    decimal Balance,
    Guid UserId);