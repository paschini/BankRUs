using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.Transactions.Exceptions
{
    public class NotOwnedException : Exception
    {

        public NotOwnedException() : base("The bank account is not owned by the user.")
        {
        }

        public NotOwnedException(string message) : base(message)
        {
        }
    }
}
