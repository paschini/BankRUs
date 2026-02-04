using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Api.Dtos.Transactions;
using BankRUs.Application.UseCases.Deposit;
using BankRUs.Application.UseCases.OpenBankAccount;
using BankRUs.Application.UseCases.Transactions;
using BankRUs.Application.UseCases.Transactions.Exceptions;
using BankRUs.Application.UseCases.Withdrawal;
using BankRUs.Application.UseCases.Withdrawal.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankRUs.Api.Controllers;

[Route("api/bank-accounts")]
[Authorize(Roles = "Customer")]
[ApiController]
public class BankAccountsController : ControllerBase
{
    private readonly OpenBankAccountHandler _openBankAccountHandler;
    private readonly DepositHandler _depositToAccountHandler;
    private readonly WithdrawalHandler _withdrawalFromAccountHandler;
    private readonly TransactionsQueryHandler _transactionsQueryHandler;

    public BankAccountsController(
        OpenBankAccountHandler openBankAccountHandler, 
        DepositHandler depositToAccountHandler,
        WithdrawalHandler withdrawalFromAccountHandler,
        TransactionsQueryHandler transactionQueryHandler)
    {
        _openBankAccountHandler = openBankAccountHandler;
        _depositToAccountHandler = depositToAccountHandler;
        _withdrawalFromAccountHandler = withdrawalFromAccountHandler;
        _withdrawalFromAccountHandler = withdrawalFromAccountHandler;
        _transactionsQueryHandler = transactionQueryHandler;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
                AccountId = bankAccountId,
                UserId = Guid.Parse(userId),
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
            return BadRequest(new { ex.Message });
        }
    }

    // POST /api/bank-accounts/{bankAccountId}/withdrawal
    [HttpPost]
    [Route("{bankAccountId}/withdrawal")]
    public async Task<IActionResult> WithdrawalFromBankAccount(Guid bankAccountId, DepositRequestDto request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var withdrawalResult = await _withdrawalFromAccountHandler.HandleAsync(
            new WithdrawalCommand
            {
                AccountNumber = bankAccountId,
                Amount = request.Amount,
                Reference = request.Reference,
                Currency = "SEK",
                UserId = Guid.Parse(userId ?? string.Empty)
            });

            var response = new WithdrawalResponseDto
            {
                TransactionId = withdrawalResult.TransactionId,
                AccountId = bankAccountId,
                UserId = Guid.Parse(userId),
                Type = withdrawalResult.Type,
                Amount = withdrawalResult.Amount,
                Currency = withdrawalResult.Currency,
                Reference = withdrawalResult.Reference,
                CreatedAt = withdrawalResult.CreatedAt,
                BalanceAfter = withdrawalResult.BalanceAfter
            };

            return Created(string.Empty, response);
        }
        catch (InsufficientFundsException ex)
        {
            return Conflict(new { ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    // GET /api/bank-accounts/{bankAccountId}/transactions
    [HttpGet]
    [Route("{bankAccountId}/transactions")]
    public async Task<IActionResult> GetTransactions(Guid bankAccountId, [FromQuery] TransactionQueryParamsDto query)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transactions = await _transactionsQueryHandler.HandleAsync(
                new TransactionsQuery
                {
                    UserId = Guid.Parse(userId),
                    BankAccountId = bankAccountId,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    From = query.From,
                    To = query.To,
                    Type = query.Type,
                    Sort = query.Sort
                });

            var response = new TransactionsQueryResponseDto
            {
                AccountId = transactions.AccountId,
                Balance = transactions.Balance,
                Paging = new PagingInfoDto
                {
                    Page = transactions.Paging.Page,
                    PageSize = transactions.Paging.PageSize,
                    TotalCount = transactions.Paging.TotalCount
                },
                Items = transactions.Items.Select(tr => new TransactionDto
                {
                    TransactionId = tr.Id,
                    TransactionType = tr.TransactionType,
                    TransactionAmount = tr.Amount,
                    Currency = tr.TransactionCurrency,
                    TransactionReference = tr.Reference,
                    TransactionCreatedAt = tr.TransactionDate,
                    BalanceAfterTransaction = tr.BalanceAfterTransaction
                }).ToList()
            };
            return Ok(response);
        } catch (NotOwnedException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}
