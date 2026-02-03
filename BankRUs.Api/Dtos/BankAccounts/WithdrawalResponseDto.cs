namespace BankRUs.Api.Dtos.BankAccounts
{
    public record WithdrawalResponseDto
    {
        public Guid TransactionId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "SEK";
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal BalanceAfter { get; set; }
    }
}
