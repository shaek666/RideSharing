namespace RideSharing.Observers
{
    public class RiderNotifier : IRideObserver
    {
        public void Update(string rideId, string status)
        {
            Console.WriteLine($"SMS to Rider: Ride {rideId} is now {status}");
        }
    }
}