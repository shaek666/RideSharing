using RideSharing.Vehicles;

namespace RideSharing.Users
{
    public class Driver : User
    {
        public IVehicle Vehicle { get; set; } = null!;

        public bool IsAvailable { get; set; }

        public override void DisplayInfo()
        {
            Console.WriteLine(
                $"Role: {GetRole()}, ID: {Id}, Name: {Name}, Phone: {Phone}, Vehicle: {Vehicle.GetVehicleType()}, Available: {IsAvailable}"
            );
        }

        public override string GetRole()
        {
            return "Driver";
        }
    }
}
