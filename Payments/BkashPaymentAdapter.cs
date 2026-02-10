namespace RideSharing.Payments
{
    public class BkashPaymentAdapter : IPaymentProcessor
    {
        private BkashPaymentGateway _bkashGateway = new BkashPaymentGateway();
        public void Pay(string paymentInfo, double amount)
        {
            string txnId = _bkashGateway.SendMoney(paymentInfo, amount);
            Console.WriteLine($"Transaction ID: {txnId}");
        }

        public string GetPaymentMethod()
        {
            return "bKash";
        }
    }
}