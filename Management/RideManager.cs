using RideSharing.Users;

namespace RideSharing.Management
{
    public class RideManager
    {
        private static RideManager? _instance;
        private static readonly object InstanceLock = new object();
        private readonly List<Driver> _drivers;

        private RideManager()
        {
            _drivers = new List<Driver>();
        }

        public void RegisterDriver(Driver driver)
        {
            if (driver is null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            bool alreadyExists = _drivers.Any(d => d.Id == driver.Id);
            if (alreadyExists)
            {
                throw new InvalidOperationException($"Driver with ID {driver.Id} is already registered.");
            }

            _drivers.Add(driver);
        }

        public List<Driver> GetAllDrivers()
        {
            return new List<Driver>(_drivers);
        }

        public List<Driver> GetAvailableDrivers(string vehicleType)
        {
            if (string.IsNullOrWhiteSpace(vehicleType))
            {
                return new List<Driver>();
            }

            List<Driver> available = new List<Driver>();
            foreach (Driver d in _drivers)
            {
                if (
                    d.IsAvailable
                    && d.Vehicle is not null
                    && string.Equals(d.Vehicle.GetVehicleType(), vehicleType, StringComparison.OrdinalIgnoreCase)
                )
                {
                    available.Add(d);
                }
            }
            return available;
        }

        public static RideManager GetInstance()
        {
            if (_instance is not null)
            {
                return _instance;
            }

            lock (InstanceLock)
            {
                _instance ??= new RideManager();
                return _instance;
            }
        }
    }
}
