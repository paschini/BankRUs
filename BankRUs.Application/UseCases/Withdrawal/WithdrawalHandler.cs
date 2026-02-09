using BankRUs.Application.Repositories;
using BankRUs.Application.UseCases.Withdrawal.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BankRUs.Application.UseCases.Withdrawal
{
    public class WithdrawalHandler
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IBankAccountTransactionRepository _bankAccountTransactionRepository;

        public WithdrawalHandler(
            IBankAccountRepository bankAccountRepository,
            IBankAccountTransactionRepository bankAccountTransactionRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _bankAccountTransactionRepository = bankAccountTransactionRepository;
        }

        public async Task<WithdrawalResult> HandleAsync(WithdrawalCommand command)
        {
            var bankAccount = await _bankAccountRepository.GetByAccountNumber(command.AccountNumber);

            if (bankAccount == null || command.Amount < 0)
            {
                throw new Exception("Bank account not found or bad amount.");
            }

            if (bankAccount.Balance < command.Amount)
            {
                throw new InsufficientFundsException("Insufficient funds for withdrawal.");
            }

            if (bankAccount.UserId != command.UserId.ToString())
            {
                throw new Exception("Unauthorized withdrawal attempt.");
            }

            bankAccount.SetBalance(bankAccount.Balance - command.Amount);
            await _bankAccountRepository.UpdateBalance(bankAccount);

            var transaction = new Domain.Entities.BankAccountTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = bankAccount.Id,
                TransactionDate = DateTime.Now,
                Amount = -command.Amount,
                BalanceAfterTransaction = bankAccount.Balance,
                TransactionType = "Withdrawal",
                TransactionCurrency = command.Currency,
                Reference = command.Reference
            };

            await _bankAccountTransactionRepository.Add(transaction);

            return new WithdrawalResult
            {
                TransactionId = transaction.Id,
                Type = transaction.TransactionType,
                Amount = -transaction.Amount,
                Currency = transaction.TransactionCurrency ?? "SEK",
                Reference = transaction.Reference,
                CreatedAt = transaction.TransactionDate,
                BalanceAfter = transaction.BalanceAfterTransaction
            };
        }
    }
}
