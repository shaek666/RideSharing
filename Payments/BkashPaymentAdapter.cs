namespace RideSharing.Payments
{
    public class BkashPaymentAdapter : IPaymentProcessor
    {
        private readonly BkashPaymentGateway _bkashGateway = new BkashPaymentGateway();

        public void Pay(string paymentInfo, double amount)
        {
            if (string.IsNullOrWhiteSpace(paymentInfo))
            {
                throw new ArgumentException("bKash phone number is required.", nameof(paymentInfo));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            }

            string txnId = _bkashGateway.SendMoney(paymentInfo, amount);
            bool isSuccess = _bkashGateway.CheckStatus(txnId);
            if (!isSuccess)
            {
                throw new InvalidOperationException("bKash transaction failed.");
            }

            Console.WriteLine($"bKash transaction successful. Transaction ID: {txnId}");
        }

        public string GetPaymentMethod()
        {
            return "bKash";
        }
    }
}
