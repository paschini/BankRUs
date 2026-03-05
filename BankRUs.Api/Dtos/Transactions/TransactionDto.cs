namespace BankRUs.Api.Dtos.Transactions
{
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public string TransactionType { get; set; } = string.Empty; // deposit|withdrawal
        public decimal TransactionAmount { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public DateTime TransactionCreatedAt { get; set; }
        public string Currency { get; set; } = "SEK";
        public decimal BalanceAfterTransaction { get; set; }
    }
}

