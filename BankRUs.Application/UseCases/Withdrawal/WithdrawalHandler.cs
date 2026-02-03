using BankRUs.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BankRUs.Application.UseCases.Withdrawal
{
    public class WithdrawalHandler
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IBankAccountTransactionRepository _bankAccountTransactionRepository

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

            if (bankAccount == null || command.Amount < 0 || bankAccount.Balance < command.Amount)
            {
                throw new Exception("Bank account not found, bad amount, or insufficient funds.");
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
                UserId = Guid.Parse(bankAccount.UserId),
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
