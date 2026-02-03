using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Application.UseCases.Deposit;
using BankRUs.Application.UseCases.Withdrawal;
using BankRUs.Application.UseCases.OpenBankAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers;

[Route("api/bank-accounts")]
[Authorize(Roles = "Customer")]
[ApiController]
public class BankAccountsController : ControllerBase
{
    private readonly OpenBankAccountHandler _openBankAccountHandler;
    private readonly DepositHandler _depositToAccountHandler;
    private readonly WithdrawalHandler _withdrawalFromAccountHandler;

    public BankAccountsController(
        OpenBankAccountHandler openBankAccountHandler, 
        DepositHandler depositToAccountHandler,
        WithdrawalHandler withdrawalFromAccountHandler)
    {
        _openBankAccountHandler = openBankAccountHandler;
        _depositToAccountHandler = depositToAccountHandler;
        _withdrawalFromAccountHandler = withdrawalFromAccountHandler;
        _withdrawalFromAccountHandler = withdrawalFromAccountHandler;
    }

    // POST /api/bank-accounts
    // {
    //    "userId": "",
    //    "bankAccountName": "Semester"
    // }
    [HttpPost]
    public async Task<IActionResult> CreateBankAccount(CreateBankAccountRequestDto request)
    {
        var openBankAccountResult = await _openBankAccountHandler.HandleAsync(
            new OpenBankAccountCommand(UserId: request.UserId));

        // TODO: Hårdkodad information nedan ska komma från 
        // resultatobjektet
        var response = new BankAccountDto(
            Id: openBankAccountResult.Id,
            AccountNumber: "100.200.300",
            Name: "Standardkonto",
            IsLocked: false,
            Balance: 0m,
            UserId: Guid.NewGuid());

        return Created(string.Empty, response);
    }

    // POST /api/bank-accounts/{bankAccountId}/deposit
    [HttpPost]
    [Route("{bankAccountId}/deposit")]
    public async Task<IActionResult> DepositToBankAccount(Guid bankAccountId, DepositRequestDto request)
    {
        try
        {
            var depositResult = await _depositToAccountHandler.HandleAsync(
            new DepositCommand
            {
                AccountNumber = bankAccountId,
                Amount = request.Amount,
                Reference = request.Reference,
                Currency = "SEK"
            });

            var response = new DepositResponseDto
            {
                TransactionId = depositResult.TransactionId,
                Type = depositResult.Type,
                Amount = depositResult.Amount,
                Currency = depositResult.Currency,
                Reference = depositResult.Reference,
                CreatedAt = depositResult.CreatedAt,
                BalanceAfter = depositResult.BalanceAfter
            };

            return Created(string.Empty, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // POST /api/bank-accounts/{bankAccountId}/withdrawal
    [HttpPost]
    [Route("{bankAccountId}/withdrawal")]
    public async Task<IActionResult> WithdrawalFromBankAccount(Guid bankAccountId, DepositRequestDto request)
    {
        try
        {
            var withdrawalResult = await _withdrawalFromAccountHandler.HandleAsync(
            new WithdrawalCommand
            {
                AccountNumber = bankAccountId,
                Amount = request.Amount,
                Reference = request.Reference,
                Currency = "SEK"
            });

            var response = new WithdrawalResponseDto
            {
                TransactionId = withdrawalResult.TransactionId,
                Type = withdrawalResult.Type,
                Amount = withdrawalResult.Amount,
                Currency = withdrawalResult.Currency,
                Reference = withdrawalResult.Reference,
                CreatedAt = withdrawalResult.CreatedAt,
                BalanceAfter = withdrawalResult.BalanceAfter
            };

            return Created(string.Empty, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
