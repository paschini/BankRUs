namespace BankRUs.Api.Dtos.BankAccounts
{
    public class DepositRequestDto
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
}
