namespace RideSharing.Pricing
{
    public class MidnightPricing : IPricingStrategy
    {
        public double CalculateFare(double distance, double baseFare)
        {
            return (baseFare) + (distance * .75);
        }

        public string GetStrategyName()
        {
            return "Midnight";
        }
    }
}