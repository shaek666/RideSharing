using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace RideSharing.Management
{
    public class RideManager
    {
        private static RideManager _instance = null;
        private List<Driver> _drivers;
        private RideManager()
        {
            _drivers = new List<Driver>();
        }

        public void RegisterDriver(Driver driver)
        {
            _drivers.Add(driver);
        }

        public List<Driver> GetAllDrivers()
        {
            return _drivers;
        }

        public List<Driver> GetAvailableDrivers(string vehicleType)
        {
            List<Driver> available = new List<Driver>();
            foreach(Driver d in _drivers)
            {
                if (d.Vehicle.GetVehicleType() == vehicleType && d.IsAvailable)
                {
                    available.Add(d);
                }
            }
            return available;
        }

        public static RideManager GetInstance()
        {
            if ( _instance == null )
            {
                _instance = new RideManager();
            }
            return _instance;
        }
    }
}