namespace RideSharing.Users
{
    public class Driver : User
    {
        public IVehicle Vehicle { get; set; }

        public bool IsAvailable { get; set; }

        public override void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id}, Name: {Name}, Phone: {Phone}, Vehicle: {Vehicle.GetVehicleType()}");
        }

        public override string GetRole()
        {
            return "Driver";
        }
    }
}