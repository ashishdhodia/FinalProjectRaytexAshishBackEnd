namespace TransactionServiceBus.Model
{
    public class TransactionData
    {
        public int Id { get; set; }
        public string containerId { get; set; }
        public float containerFees { get; set; }
        public string userId { get; set; }
        public string cardOwnerName { get; set; }
        public string cardType { get; set; }
        public int cardNumber { get; set; }
        public string txnTime { get; set; }
    }
}
