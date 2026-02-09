using System.ComponentModel;
using System.Data.SqlTypes;
using System.Reflection.Metadata.Ecma335;

namespace RideSharing.Vehicles
{
    public class Bike : IVehicle
    {
        public double GetBaseFare()
        {
            return 2.0; //$2 for Bike
        }

        public string GetVehicleType()
        {
            return "Bike";
        }
    }
}