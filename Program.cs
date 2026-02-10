using RideSharing.Management;
using RideSharing.Observers;
using RideSharing.Payments;
using RideSharing.Pricing;
using RideSharing.Rides;
using RideSharing.Users;
using RideSharing.Vehicles;

namespace RideSharing
{
    class Program
    {
        static void Main(string[] args)
        {
            RideManager rideManager = RideManager.GetInstance();

            VehicleFactory vehicleFactory = new VehicleFactory();
            IVehicle car = vehicleFactory.CreateVehicle("Car");
            IVehicle bike = vehicleFactory.CreateVehicle("Bike");

            Rider rider = new Rider();
            rider.Name = "Maruf";
            rider.Phone = "+8801904626203";
            rider.WalletBalance = 500.00;

            Driver driver1 = new Driver();
            driver1.Id = Guid.NewGuid();
            driver1.Name = "Shakil";
            driver1.Phone = "+8801308625894";
            driver1.Vehicle = bike;
            rideManager.RegisterDriver(driver1);
            driver1.IsAvailable = true;

            Driver driver2 = new Driver();
            driver2.Id = Guid.NewGuid();
            driver2.Name = "Ahmed";
            driver2.Phone = "+8801308625893";
            driver2.Vehicle = car;
            rideManager.RegisterDriver(driver2);
            driver2.IsAvailable = true;

            string input;

            while (true)
            {
                Console.WriteLine("1. Request Ride\n2. Exit");
                input = Console.ReadLine();
                int choice = int.Parse(input);
                if (choice == 1)
                {
                    List<Driver> drivers = rideManager.GetAvailableDrivers("Car");
                    if (drivers.Count > 0)
                    {
                        Ride ride = new Ride();
                        ride.Id = Guid.NewGuid().ToString();
                        ride.Rider = rider;
                        ride.Driver = drivers[0];
                        ride.Distance = 100.00;
                        ride.SetPricingStrategy(new StandardPricing());
                        ride.AddObserver(new RiderNotifier());
                        ride.AddObserver(new DriverNotifier());
                        ride.SetStatus("Requested");
                        IPaymentProcessor payment = new BkashPaymentAdapter();
                        double fare = ride.CalculateFare();
                        Console.WriteLine($"Fare: {fare}");
                        payment.Pay(rider.Phone, fare);
                        ride.SetStatus("Completed");
                    }
                    else
                    {
                        Console.WriteLine("No drivers available.");
                    }
                }
                else if (choice == 2)
                {
                    break;
                }
            }
        }
    }
}
