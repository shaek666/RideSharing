namespace RideSharing.Vehicles
{
    public class Car : IVehicle
    {
        public double GetBaseFare()
        {
            return 5.0; //$5 for Car
        }

        public string GetVehicleType()
        {
            return "Car";
        }
    }
}