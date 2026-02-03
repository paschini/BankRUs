namespace BankRUs.Api.Dtos.BankAccounts
{
    public record DepositResponseDto
    {
        public Guid TransactionId { get; set; }
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "SEK";
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal BalanceAfter { get; set; }
    }
}
