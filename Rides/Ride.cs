using RideSharing.Observers;
using RideSharing.Pricing;
using RideSharing.Users;

namespace RideSharing.Rides
{
    public class Ride
    {
        public string Id { get; set; }
        public Rider Rider { get; set; }
        public Driver Driver { get; set; }
        public double Distance { get; set; }
        public string Status { get; set; }
        private IPricingStrategy _pricingStrategy;
        public List<IRideObserver> _observers = new List<IRideObserver>();

        public void AddObserver(IRideObserver observer)
        {
            _observers.Add(observer);
        }

        public void SetStatus(string status)
        {
            Status = status;
            foreach (IRideObserver observer in _observers)
            {
                observer.Update(Id, status);
            }
        }

        public double CalculateFare()
        {
            return _pricingStrategy.CalculateFare(Distance, Driver.Vehicle.GetBaseFare());
        }

        public void SetPricingStrategy(IPricingStrategy strategy)
        {
            _pricingStrategy = strategy;
        }
    }
}
