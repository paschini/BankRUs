namespace BankRUs.Api.Dtos.BankAccounts
{
    public record DepositRequestDto
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
}
