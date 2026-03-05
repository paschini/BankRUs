namespace BankRUs.Application.UseCases.OpenBankAccount;

public sealed class OpenBankAccountHandler
{
    public async Task<OpenBankAccountResult> HandleAsync(OpenBankAccountCommand command)
    {
        // TODO: Implementera HandleAsync

        // SÄTT BREAKPOINT
        return new OpenBankAccountResult(Guid.NewGuid());
    }
}
