namespace RideSharing.Pricing
{
    public class StandardPricing : IPricingStrategy
    {
        public double CalculateFare(double distance, double baseFare)
        {
            return (baseFare) + (distance * 0.50);
        }

        public string GetStrategyName()
        {
            return "Standard";
        }
    }
}