using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Withdrawal.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException() : base("Insufficient funds for withdrawal.")
        {
        }

        public InsufficientFundsException(string message) : base(message)
        {
        }
    }
}
