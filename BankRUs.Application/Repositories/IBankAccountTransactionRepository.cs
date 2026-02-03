using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Repositories
{
    public  interface IBankAccountTransactionRepository
    {
        Task Add(BankAccountTransaction transaction);
    }
}
