using RideSharing.Management;
using RideSharing.Observers;
using RideSharing.Payments;
using RideSharing.Pricing;
using RideSharing.Rides;
using RideSharing.Users;
using RideSharing.Vehicles;
using RideSharing.Utils;

namespace RideSharing
{
    internal static class Program
    {
        private static readonly RideManager RideManagerInstance = RideManager.GetInstance();
        private static readonly VehicleFactory VehicleFactoryInstance = new VehicleFactory();
        private static readonly List<Rider> Riders = new List<Rider>();
        private static readonly List<Ride> Rides = new List<Ride>();

        static void Main(string[] args)
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                PrintMenu();
                int choice = ValidationHelper.ReadInt("Select an option: ", 0, 9);

                try
                {
                    switch (choice)
                    {
                        case 1:
                            RegisterRider();
                            break;
                        case 2:
                            RegisterDriver();
                            break;
                        case 3:
                            ViewRiders();
                            break;
                        case 4:
                            ViewDrivers();
                            break;
                        case 5:
                            CreateRide();
                            break;
                        case 6:
                            ChangeRidePricingStrategy();
                            break;
                        case 7:
                            UpdateRideStatus();
                            break;
                        case 8:
                            ProcessRidePayment();
                            break;
                        case 9:
                            ViewRides();
                            break;
                        case 0:
                            keepRunning = false;
                            Console.WriteLine("Exiting Ride Sharing System.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Operation failed: {e.Message}");
                }
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== Ride Sharing System =====");
            Console.WriteLine("1. Register Rider");
            Console.WriteLine("2. Register Driver");
            Console.WriteLine("3. View Riders");
            Console.WriteLine("4. View Drivers");
            Console.WriteLine("5. Create Ride");
            Console.WriteLine("6. Change Ride Pricing Strategy");
            Console.WriteLine("7. Update Ride Status");
            Console.WriteLine("8. Process Ride Payment");
            Console.WriteLine("9. View Rides");
            Console.WriteLine("0. Exit");
        }

        private static void RegisterRider()
        {
            Console.WriteLine();
            Console.WriteLine("Register Rider");
            string phone = ValidationHelper.ReadUniqueBangladeshPhoneNumber("Phone: ", GetRegisteredPhoneNumbers());

            Rider rider = new Rider
            {
                Name = ValidationHelper.ReadName("Name: "),
                Phone = phone,
                WalletBalance = ValidationHelper.ReadWalletBalance("Wallet balance: ")
            };

            Riders.Add(rider);
            Console.WriteLine($"Rider registered successfully. Rider ID: {rider.Id}");
        }

        private static void RegisterDriver()
        {
            Console.WriteLine();
            Console.WriteLine("Register Driver");
            string vehicleType = ReadVehicleType();
            string phone = ValidationHelper.ReadUniqueBangladeshPhoneNumber("Phone: ", GetRegisteredPhoneNumbers());

            Driver driver = new Driver
            {
                Name = ValidationHelper.ReadName("Name: "),
                Phone = phone,
                Vehicle = VehicleFactoryInstance.CreateVehicle(vehicleType),
                IsAvailable = true
            };

            RideManagerInstance.RegisterDriver(driver);
            Console.WriteLine($"Driver registered successfully. Driver ID: {driver.Id}");
        }

        private static void ViewRiders()
        {
            Console.WriteLine();
            Console.WriteLine("Riders");
            if (Riders.Count == 0)
            {
                Console.WriteLine("No riders registered.");
                return;
            }

            foreach (Rider rider in Riders)
            {
                rider.DisplayInfo();
            }
        }

        private static void ViewDrivers()
        {
            Console.WriteLine();
            Console.WriteLine("Drivers");
            if (RideManagerInstance.GetAllDrivers().Count == 0)
            {
                Console.WriteLine("No drivers registered.");
                return;
            }

            Console.WriteLine("1. View all drivers");
            Console.WriteLine("2. View available drivers by vehicle type");
            int option = ValidationHelper.ReadInt("Select view mode: ", 1, 2);

            List<Driver> drivers = option == 1 ? RideManagerInstance.GetAllDrivers() : RideManagerInstance.GetAvailableDrivers(ReadVehicleType());

            if (drivers.Count == 0)
            {
                Console.WriteLine("No matching drivers found.");
                return;
            }

            foreach (Driver driver in drivers)
            {
                driver.DisplayInfo();
            }
        }

        private static void CreateRide()
        {
            Console.WriteLine();
            Console.WriteLine("Create Ride");

            Rider? rider = SelectRider();
            if (rider is null)
            {
                return;
            }

            string vehicleType = ReadVehicleType();
            List<Driver> availableDrivers = RideManagerInstance.GetAvailableDrivers(vehicleType);
            if (availableDrivers.Count == 0)
            {
                Console.WriteLine($"No available drivers found for vehicle type {vehicleType}.");
                return;
            }

            Driver driver = SelectDriver(availableDrivers);
            double distance = ValidationHelper.ReadDouble("Distance in km: ", 0.1);
            IPricingStrategy pricingStrategy = SelectPricingStrategy();
            double estimatedFare = pricingStrategy.CalculateFare(distance, driver.Vehicle.GetBaseFare());

            if (!ValidationHelper.CanCreateRide(estimatedFare, rider.WalletBalance, out string rideCreationError))
            {
                Console.WriteLine(rideCreationError);
                return;
            }

            Ride ride = new Ride
            {
                Rider = rider,
                Driver = driver,
                Distance = distance
            };
            ride.SetPricingStrategy(pricingStrategy);
            ride.AddObserver(new RiderNotifier());
            ride.AddObserver(new DriverNotifier());
            ride.SetStatus(Ride.RequestedStatus);

            driver.IsAvailable = false;
            Rides.Add(ride);

            Console.WriteLine($"Ride created successfully. Ride ID: {ride.Id}");
            Console.WriteLine($"Current status: {ride.Status}");
            Console.WriteLine($"Pricing strategy: {ride.GetPricingStrategyName()}");
            Console.WriteLine($"Estimated fare: ${estimatedFare:F2}");
        }

        private static void ChangeRidePricingStrategy()
        {
            Console.WriteLine();
            Console.WriteLine("Change Ride Pricing Strategy");

            Ride? ride = SelectRide();
            if (ride is null)
            {
                return;
            }

            if (!ValidationHelper.CanChangePricingStrategy(ride.Status, ride.IsPaid, out string pricingStateError))
            {
                Console.WriteLine(pricingStateError);
                return;
            }

            IPricingStrategy strategy = SelectPricingStrategy();
            double projectedFare = strategy.CalculateFare(ride.Distance, ride.Driver.Vehicle.GetBaseFare());
            if (!ValidationHelper.CanApplyPricingUpdate(projectedFare, ride.Rider.WalletBalance, out string pricingFareError))
            {
                Console.WriteLine(pricingFareError);
                return;
            }

            ride.SetPricingStrategy(strategy);
            Console.WriteLine($"Pricing strategy updated to {ride.GetPricingStrategyName()}.");
            Console.WriteLine($"Recalculated fare: ${projectedFare:F2}");
        }

        private static void UpdateRideStatus()
        {
            Console.WriteLine();
            Console.WriteLine("Update Ride Status");

            Ride? ride = SelectRide();
            if (ride is null)
            {
                return;
            }

            IReadOnlyList<string> nextStatuses = ride.GetNextAllowedStatuses();
            if (nextStatuses.Count == 0)
            {
                Console.WriteLine("This ride is already completed.");
                return;
            }

            Console.WriteLine($"Current status: {(string.IsNullOrWhiteSpace(ride.Status) ? "(none)" : ride.Status)}");
            Console.WriteLine();
            Console.WriteLine("Next Available Status:");
            Console.WriteLine();
            for (int i = 0; i < nextStatuses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {nextStatuses[i]}");
            }

            int choice = ValidationHelper.ReadInt("Select next status: ", 1, nextStatuses.Count);
            string selectedStatus = nextStatuses[choice - 1];
            ride.SetStatus(selectedStatus);

            if (ride.Status == Ride.CompletedStatus)
            {
                ride.Driver.IsAvailable = true;
            }

            Console.WriteLine($"Ride {ride.Id} status updated to {ride.Status}.");
        }

        private static void ProcessRidePayment()
        {
            Console.WriteLine();
            Console.WriteLine("Process Ride Payment");

            Ride? ride = SelectRide();
            if (ride is null)
            {
                return;
            }

            if (!string.Equals(ride.Status, Ride.CompletedStatus, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Ride must be completed before payment can be processed.");
                return;
            }

            if (ride.IsPaid)
            {
                Console.WriteLine($"Ride already paid using {ride.PaymentMethod}.");
                return;
            }

            double fare = ride.CalculateFare();
            Console.WriteLine($"Payable fare: ${fare:F2}");

            if (!ValidationHelper.CanProcessPayment(fare, ride.Rider.WalletBalance, out string paymentError))
            {
                Console.WriteLine(paymentError);
                return;
            }

            IPaymentProcessor paymentProcessor = SelectPaymentProcessor();
            string paymentInfo =
                paymentProcessor is BkashPaymentAdapter ? ValidationHelper.ReadBangladeshPhoneNumber("bKash phone number: ") : ValidationHelper.ReadRequired("Credit card number: ");

            paymentProcessor.Pay(paymentInfo, fare);
            ride.Rider.WalletBalance -= fare;
            ride.MarkAsPaid(paymentProcessor.GetPaymentMethod());

            Console.WriteLine($"Payment successful via {ride.PaymentMethod}.");
            Console.WriteLine($"Updated rider wallet balance: ${ride.Rider.WalletBalance:F2}");
        }

        private static void ViewRides()
        {
            Console.WriteLine();
            Console.WriteLine("Rides");
            if (Rides.Count == 0)
            {
                Console.WriteLine("No rides created.");
                return;
            }

            foreach (Ride ride in Rides)
            {
                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Ride ID: {ride.Id}");
                Console.WriteLine($"Rider: {ride.Rider.Name} ({ride.Rider.Id})");
                Console.WriteLine($"Driver: {ride.Driver.Name} ({ride.Driver.Id})");
                Console.WriteLine($"Vehicle: {ride.Driver.Vehicle.GetVehicleType()}");
                Console.WriteLine($"Distance: {ride.Distance:F2} km");
                Console.WriteLine($"Status: {ride.Status}");
                Console.WriteLine($"Pricing Strategy: {ride.GetPricingStrategyName()}");
                Console.WriteLine($"Fare: ${ride.CalculateFare():F2}");
                Console.WriteLine($"Paid: {ride.IsPaid} ({ride.PaymentMethod})");
            }
            Console.WriteLine("----------------------------------------");
        }

        private static Rider? SelectRider()
        {
            if (Riders.Count == 0)
            {
                Console.WriteLine("No riders available. Register a rider first.");
                return null;
            }

            Console.WriteLine("Available riders:");
            for (int i = 0; i < Riders.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Riders[i].Name} ({Riders[i].Id})");
            }

            int choice = ValidationHelper.ReadInt("Select rider: ", 1, Riders.Count);
            return Riders[choice - 1];
        }

        private static Driver SelectDriver(IReadOnlyList<Driver> drivers)
        {
            Console.WriteLine("Available drivers:");
            for (int i = 0; i < drivers.Count; i++)
            {
                Console.WriteLine(
                    $"{i + 1}. {drivers[i].Name} ({drivers[i].Id}) - {drivers[i].Vehicle.GetVehicleType()}"
                );
            }

            int choice = ValidationHelper.ReadInt("Select driver: ", 1, drivers.Count);
            return drivers[choice - 1];
        }

        private static Ride? SelectRide()
        {
            if (Rides.Count == 0)
            {
                Console.WriteLine("No rides available.");
                return null;
            }

            Console.WriteLine("Available rides:");
            for (int i = 0; i < Rides.Count; i++)
            {
                Console.WriteLine(
                    $"{i + 1}. Ride {Rides[i].Id} | Rider: {Rides[i].Rider.Name} | Driver: {Rides[i].Driver.Name} | Status: {Rides[i].Status}"
                );
            }

            int choice = ValidationHelper.ReadInt("Select ride: ", 1, Rides.Count);
            return Rides[choice - 1];
        }

        private static IPricingStrategy SelectPricingStrategy()
        {
            Console.WriteLine("Select pricing strategy:");
            Console.WriteLine("1. Standard ($0.50/km)");
            Console.WriteLine("2. Rush Hour ($1.00/km)");
            Console.WriteLine("3. Midnight ($0.75/km)");

            int choice = ValidationHelper.ReadInt("Choice: ", 1, 3);
            return choice switch
            {
                1 => new StandardPricing(),
                2 => new RushHourPricing(),
                3 => new MidnightPricing(),
                _ => new StandardPricing()
            };
        }

        private static IPaymentProcessor SelectPaymentProcessor()
        {
            Console.WriteLine("Select payment method:");
            Console.WriteLine("1. bKash");
            Console.WriteLine("2. Credit Card");

            int choice = ValidationHelper.ReadInt("Choice: ", 1, 2);
            return choice == 1 ? new BkashPaymentAdapter() : new CreditCardProcessor();
        }

        private static string ReadVehicleType()
        {
            Console.WriteLine("Select vehicle type:");
            Console.WriteLine("1. Bike ($2 base fare)");
            Console.WriteLine("2. CNG ($3 base fare)");
            Console.WriteLine("3. Car ($5 base fare)");

            int choice = ValidationHelper.ReadInt("Choice: ", 1, 3);
            return choice switch
            {
                1 => "Bike",
                2 => "CNG",
                3 => "Car",
                _ => "Bike"
            };
        }

        private static IEnumerable<string> GetRegisteredPhoneNumbers()
        {
            foreach (Rider rider in Riders)
            {
                yield return rider.Phone;
            }

            foreach (Driver driver in RideManagerInstance.GetAllDrivers())
            {
                yield return driver.Phone;
            }
        }
    }
}
