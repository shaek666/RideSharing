namespace RideSharing.Users
{
    public class Rider : User
    {
        public double WalletBalance { get; set; }

        public override void DisplayInfo()
        {
            Console.WriteLine(
                $"Role: {GetRole()}, ID: {Id}, Name: {Name}, Phone: {Phone}, Wallet: ${WalletBalance:F2}"
            );
        }

        public override string GetRole()
        {
            return "Rider";
        }
    }
}
