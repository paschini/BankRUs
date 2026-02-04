namespace BankRUs.Api.Dtos.Transactions
{
    public record TransactionsQueryResponseDto
    {
        public Guid AccountId { get; set; }
        public string Currency { get; set; } = "SEK";
        public decimal Balance { get; set; }
        public PagingInfoDto Paging { get; set; } = new PagingInfoDto();
        public List<TransactionDto> Items { get; set; } = new List<TransactionDto>();
    }
}
