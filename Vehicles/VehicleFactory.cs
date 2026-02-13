namespace RideSharing.Vehicles
{
    public class VehicleFactory
    {
        public IVehicle CreateVehicle(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Vehicle type is required.", nameof(type));
            }

            return type.Trim().ToUpperInvariant() switch
            {
                "BIKE" => new Bike(),
                "CNG" => new CNG(),
                "CAR" => new Car(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported vehicle type: {type}")
            };
        }
    }
}
