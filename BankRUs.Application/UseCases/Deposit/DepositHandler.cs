using BankRUs.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Deposit
{
    public class DepositHandler
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IBankAccountTransactionRepository _bankAccountTransactionRepository;

        public DepositHandler(
            IBankAccountRepository bankAccountRepository,
            IBankAccountTransactionRepository bankAccountTransactionRepository) 
        {
            _bankAccountRepository = bankAccountRepository;
            _bankAccountTransactionRepository = bankAccountTransactionRepository;
        }

        public async Task<DepositResult> HandleAsync(DepositCommand command)
        {
            var bankAccount = await _bankAccountRepository.GetByAccountNumber(command.AccountNumber);
            if (bankAccount == null || command.Amount < 0)
            {
                throw new Exception("Bank account not found or bad amount.");
            }

            bankAccount.SetBalance(bankAccount.Balance + command.Amount);
            await _bankAccountRepository.UpdateBalance(bankAccount);

            var transaction = new Domain.Entities.BankAccountTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = bankAccount.Id,
                TransactionDate = DateTime.Now,
                Amount = command.Amount,
                BalanceAfterTransaction = bankAccount.Balance,
                TransactionType = "Deposit",
                TransactionCurrency = command.Currency,
                Reference = command.Reference
            };

            await _bankAccountTransactionRepository.Add(transaction);

            return new DepositResult
            {
                TransactionId = transaction.Id,
                Type = transaction.TransactionType,
                Amount = transaction.Amount,
                Currency = transaction.TransactionCurrency ?? "SEK",
                Reference = transaction.Reference,
                CreatedAt = transaction.TransactionDate,
                BalanceAfter = transaction.BalanceAfterTransaction
            };
        }
    }
}
