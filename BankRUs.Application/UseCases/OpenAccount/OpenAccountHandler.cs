using BankRUs.Application.Identity;
using BankRUs.Application.Repositories;
using BankRUs.Application.Services;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.OpenAccount;

public class OpenAccountHandler
{
    private readonly IIdentityService _identityService;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IEmailSender _emailSender;

    public OpenAccountHandler(
        IIdentityService identityService,
        IBankAccountRepository bankAccountRepository,
        IEmailSender emailSender)
    {
        _identityService = identityService;
        _bankAccountRepository = bankAccountRepository;
        _emailSender = emailSender;
    }

    public async Task<OpenAccountResult> HandleAsync(OpenAccountCommand command)
    {
        // 1 - Validera indata

        // 2 - Skapa användarkonto
        var createUserResult = await _identityService.CreateUserAsync(new CreateUserRequest(
            FirstName: command.FirstName,
            LastName: command.LastName,
            SocialSecurityNumber: command.SocialSecurityNumber,
            Email: command.Email
         ));

        // 3 - Skapa ett första bankkonto åt kunden

        // TODO: Använd service för att generera ett kontonummer
        var bankAccount = new BankAccount(
            accountNumber: "100.200.300",  // accountNumberGenerator.Generate()
            name: "Standardkonto",
            userId: createUserResult.UserId.ToString());

        await _bankAccountRepository.Add(bankAccount);

        // 4 - Skicka välkomstmail till kund
        await _emailSender.SendEmailAsync(
            from: "no-reply@bankrus.com", 
            to: command.Email,
            subject: "Ditt konto är klart",
            body: "Välkommen till Bank-R-Us! Ditt konto är nu klart!");

        // 5 - Returnera resultatet av kommandot

        return new OpenAccountResult(UserId: createUserResult.UserId);
    }
}