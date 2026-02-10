using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RideSharing.Payments
{
    public class CreditCardProcessor : IPaymentProcessor
    {
        public void Pay(string paymentInfo, double amount)
        {
            Console.WriteLine($"Charging Credit Card {paymentInfo} for ${amount}");
        }

        public string GetPaymentMethod()
        {
            return "Credit Card";
        }
    }
}