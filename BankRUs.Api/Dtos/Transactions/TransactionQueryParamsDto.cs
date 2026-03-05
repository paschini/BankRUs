namespace BankRUs.Api.Dtos.Transactions
{
    public class TransactionQueryParamsDto
    {
        public int Page { get; set; } = 1;

        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Min(value, 100);
        }

        public DateTime? From { get; set; } 
        public DateTime? To { get; set; }

        public string Type { get; set; } 
        public string Sort { get; set; } = "desc"; // desc|asc
    }
}
