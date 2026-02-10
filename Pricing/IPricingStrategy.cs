namespace RideSharing.Pricing
{
    public interface IPricingStrategy
    {
        double CalculateFare(double distance, double baseFare);

        string GetStrategyName();
    }
}