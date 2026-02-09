namespace RideSharing.Users
{
    public class Rider : User
    {
        public double WalletBalance { get; set; }

        public override void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id}, Name:  {Name}, Phone: {Phone}");
        }

        public override string GetRole()
        {
            return "Rider";
        }
    }


}