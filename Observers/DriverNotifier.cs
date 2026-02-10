namespace RideSharing.Observers
{
    public class DriverNotifier : IRideObserver
    {
        public void Update(string rideId, string status)
        {
            Console.WriteLine($"App Push to Driver: Ride {rideId} is now {status}");
        }
    }
}