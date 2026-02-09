namespace RideSharing.Vehicles
{
    public class CNG : IVehicle
    {
        public double GetBaseFare()
        {
            return 3.0; //$3 for CNG
        }

        public string GetVehicleType()
        {
            return "CNG";
        }
    }
}