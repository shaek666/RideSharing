namespace RideSharing.Payments
{
    public class BkashPaymentGateway
    {    
        public string SendMoney(string phoneNumber, double amount)
            {
                Console.WriteLine($"bKash: Sending ${amount} to {phoneNumber}");  
                return "TXN" + new Random().Next(1000, 9999);
            }

            public bool CheckStatus(string transactionId)
            {
                Console.WriteLine($"bKash: {transactionId} successful");        
                return true;
            }
    }

}