namespace BankRUs.Api.Dtos.BankAccounts
{
    public record WithdrawalRequestDto
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
}
