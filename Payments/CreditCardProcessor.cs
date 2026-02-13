namespace RideSharing.Payments
{
    public class CreditCardProcessor : IPaymentProcessor
    {
        public void Pay(string paymentInfo, double amount)
        {
            if (string.IsNullOrWhiteSpace(paymentInfo))
            {
                throw new ArgumentException("Card number is required.", nameof(paymentInfo));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            }

            Console.WriteLine($"Charging Credit Card {MaskCardNumber(paymentInfo)} for ${amount:F2}");
        }

        public string GetPaymentMethod()
        {
            return "Credit Card";
        }

        private static string MaskCardNumber(string cardNumber)
        {
            string digitsOnly = new string(cardNumber.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length <= 4)
            {
                return digitsOnly;
            }

            string lastFour = digitsOnly[^4..];
            return $"**** **** **** {lastFour}";
        }
    }
}
