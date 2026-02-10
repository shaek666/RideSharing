namespace RideSharing.Pricing
{
    public class RushHourPricing : IPricingStrategy
    {
        public double CalculateFare(double distance, double baseFare)
        {
            return (baseFare) + (distance * 1);
        }

        public string GetStrategyName()
        {
            return "Rush Hour";
        }
    }
}