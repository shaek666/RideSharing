namespace RideSharing.Payments
{
    public interface IPaymentProcessor
    {
        void Pay(string paymentInfo, double amount);
        string GetPaymentMethod();
    }
}