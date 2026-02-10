namespace RideSharing.Observers
{
    public interface IRideObserver
    {
        void Update(string rideId, string status);
    }
}