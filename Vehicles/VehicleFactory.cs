using System.Collections.Concurrent;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace RideSharing.Vehicles
{
    public class VehicleFactory
    {
        public IVehicle CreateVehicle(string type)
        {
            switch (type)
            {
                case "Bike":
                return new Bike();
                case "CNG":
                return new CNG();
                case "Car":
                return new Car();
                default:
                return null;
            }
        }
    }
}