using BankRUs.Application.Repositories;
using BankRUs.Application.UseCases.Transactions.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Transactions
{
    public class TransactionsQueryHandler
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IBankAccountTransactionRepository _bankAccountTransactionRepository;

        public TransactionsQueryHandler(
            IBankAccountRepository bankAccountRepository,
            IBankAccountTransactionRepository bankAccountTransactionRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _bankAccountTransactionRepository = bankAccountTransactionRepository;
        }

        public async Task<TransactionQueryResult> HandleAsync(TransactionsQuery query)
        {
            var bankAccount = await _bankAccountRepository.GetByAccountNumber(query.BankAccountId);
            if (Guid.Parse(bankAccount.UserId) != query.UserId)
            {
                throw new NotOwnedException("Bank account not found.");
            }

            return await _bankAccountTransactionRepository.GetTransactionsAsync(query);
        }
    }
}
