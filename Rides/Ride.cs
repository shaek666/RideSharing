using RideSharing.Observers;
using RideSharing.Pricing;
using RideSharing.Users;

namespace RideSharing.Rides
{
    public class Ride
    {
        public const string RequestedStatus = "Requested";
        public const string AcceptedStatus = "Accepted";
        public const string InProgressStatus = "In Progress";
        public const string CompletedStatus = "Completed";

        private static readonly Dictionary<string, string> CanonicalStatuses =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [RequestedStatus] = RequestedStatus,
                [AcceptedStatus] = AcceptedStatus,
                [InProgressStatus] = InProgressStatus,
                [CompletedStatus] = CompletedStatus
            };

        private static readonly Dictionary<string, string?> NextStatus =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [RequestedStatus] = AcceptedStatus,
                [AcceptedStatus] = InProgressStatus,
                [InProgressStatus] = CompletedStatus,
                [CompletedStatus] = null
            };

        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public Rider Rider { get; set; } = null!;
        public Driver Driver { get; set; } = null!;
        public double Distance { get; set; }
        public string Status { get; private set; } = string.Empty;
        public bool IsPaid { get; private set; }
        public string PaymentMethod { get; private set; } = "Unpaid";

        private IPricingStrategy _pricingStrategy = new StandardPricing();
        private readonly List<IRideObserver> _observers = new List<IRideObserver>();

        public void AddObserver(IRideObserver observer)
        {
            if (observer is null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            _observers.Add(observer);
        }

        public IReadOnlyList<string> GetNextAllowedStatuses()
        {
            if (string.IsNullOrWhiteSpace(Status))
            {
                return new List<string> { RequestedStatus };
            }

            if (!NextStatus.TryGetValue(Status, out string? next) || string.IsNullOrWhiteSpace(next))
            {
                return new List<string>();
            }

            return new List<string> { next };
        }

        public void SetStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status is required.", nameof(status));
            }

            if (!CanonicalStatuses.TryGetValue(status, out string? nextCanonicalStatus))
            {
                throw new ArgumentException($"Invalid status: {status}", nameof(status));
            }

            IReadOnlyList<string> allowed = GetNextAllowedStatuses();
            bool canTransition = allowed.Any(s => s.Equals(nextCanonicalStatus, StringComparison.OrdinalIgnoreCase));
            if (!canTransition)
            {
                string fromStatus = string.IsNullOrWhiteSpace(Status) ? "(none)" : Status;
                string allowedValues = allowed.Count == 0 ? "(none)" : string.Join(", ", allowed);
                throw new InvalidOperationException(
                    $"Invalid status transition. Current: {fromStatus}. Requested: {nextCanonicalStatus}. Allowed: {allowedValues}."
                );
            }

            Status = nextCanonicalStatus;
            foreach (IRideObserver observer in _observers)
            {
                observer.Update(Id, Status);
            }
        }

        public double CalculateFare()
        {
            if (Driver is null || Driver.Vehicle is null)
            {
                throw new InvalidOperationException("Ride is missing an assigned driver or vehicle.");
            }

            if (Distance <= 0)
            {
                throw new InvalidOperationException("Distance must be greater than zero.");
            }

            return _pricingStrategy.CalculateFare(Distance, Driver.Vehicle.GetBaseFare());
        }

        public void SetPricingStrategy(IPricingStrategy strategy)
        {
            if (strategy is null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            _pricingStrategy = strategy;
        }

        public string GetPricingStrategyName()
        {
            return _pricingStrategy.GetStrategyName();
        }

        public void MarkAsPaid(string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ArgumentException("Payment method is required.", nameof(paymentMethod));
            }

            IsPaid = true;
            PaymentMethod = paymentMethod;
        }
    }
}
