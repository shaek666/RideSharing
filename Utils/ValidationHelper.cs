using System;
using System.Globalization;
using System.Linq;
using RideSharing.Rides;

namespace RideSharing.Utils
{
    /// <summary>
    /// Helper methods for reading and validating console input.
    /// </summary>
    public static class ValidationHelper
    {
        private const string BangladeshCountryCode = "+880";

        /// <summary>
        /// Reads a required non-empty string from the console.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        /// <returns>A trimmed non-empty string entered by the user.</returns>
        public static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("Input is required.");
            }
        }

        /// <summary>
        /// Reads a person's name and rejects names containing numeric characters.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        /// <returns>A validated name.</returns>
        public static string ReadName(string prompt)
        {
            while (true)
            {
                string name = ReadRequired(prompt);
                if (name.Any(char.IsDigit))
                {
                    Console.WriteLine("Name cannot contain numbers.");
                    continue;
                }

                return name;
            }
        }

        /// <summary>
        /// Reads and normalizes Bangladesh phone numbers.
        /// Accepted formats:
        /// - Local: 11 digits, starts with 0 (e.g., 017XXXXXXXX)
        /// - International: +880 followed by 10 digits (e.g., +88017XXXXXXXX)
        /// Stored format is always +880XXXXXXXXXX.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        /// <returns>A normalized Bangladesh phone number in +880 format.</returns>
        public static string ReadBangladeshPhoneNumber(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? rawInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(rawInput))
                {
                    Console.WriteLine("Phone number is required.");
                    continue;
                }

                string input = rawInput.Trim();

                if (input.StartsWith('+'))
                {
                    if (input.Length != 14)
                    {
                        Console.WriteLine("Invalid length. +880 format must be exactly 14 characters.");
                        continue;
                    }

                    if (!input.StartsWith(BangladeshCountryCode, StringComparison.Ordinal))
                    {
                        Console.WriteLine("Invalid country code. Number must start with +880.");
                        continue;
                    }

                    string localPart = input[4..];
                    if (!localPart.All(char.IsDigit))
                    {
                        Console.WriteLine("Invalid number. Only digits are allowed after +880.");
                        continue;
                    }

                    if (!localPart.StartsWith("1", StringComparison.Ordinal))
                    {
                        Console.WriteLine("Invalid mobile prefix after +880.");
                        continue;
                    }

                    return input;
                }

                if (!input.All(char.IsDigit))
                {
                    Console.WriteLine("Invalid number. Use digits only, or use +880 format.");
                    continue;
                }

                if (input.Length < 11)
                {
                    Console.WriteLine("Invalid number. It must be at least 11 digits.");
                    continue;
                }

                if (input.Length > 14)
                {
                    Console.WriteLine("Invalid number. It cannot be more than 14 digits.");
                    continue;
                }

                if (input.Length == 14)
                {
                    Console.WriteLine("Invalid 14-digit input. For 14-character format, include +880.");
                    continue;
                }

                if (input.Length != 11)
                {
                    Console.WriteLine("Invalid number. Use 11-digit local format or +880 format.");
                    continue;
                }

                if (!input.StartsWith('0'))
                {
                    Console.WriteLine("Invalid local number. 11-digit numbers must start with 0.");
                    continue;
                }

                return $"{BangladeshCountryCode}{input[1..]}";
            }
        }

        /// <summary>
        /// Reads a Bangladesh phone number and enforces uniqueness against existing registered numbers.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        /// <param name="existingPhoneNumbers">Existing phone numbers to enforce uniqueness against.</param>
        /// <returns>A normalized and unique Bangladesh phone number.</returns>
        public static string ReadUniqueBangladeshPhoneNumber(string prompt, IEnumerable<string> existingPhoneNumbers)
        {
            HashSet<string> existing = new HashSet<string>(
                existingPhoneNumbers.Where(p => !string.IsNullOrWhiteSpace(p)),
                StringComparer.Ordinal
            );

            while (true)
            {
                string phone = ReadBangladeshPhoneNumber(prompt);
                if (existing.Contains(phone))
                {
                    Console.WriteLine("Phone number already exists. Please use a different phone number.");
                    continue;
                }

                return phone;
            }
        }

        /// <summary>
        /// Reads an integer from the console constrained to a min/max range.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        /// <param name="min">Minimum allowed value (inclusive).</param>
        /// <param name="max">Maximum allowed value (inclusive).</param>
        /// <returns>The validated integer entered by the user.</returns>
        public static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int value) && value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine($"Please enter a whole number between {min} and {max}.");
            }
        }

        /// <summary>
        /// Reads a double value from the console using current or invariant culture and enforces a minimum value.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        /// <param name="minValue">Minimum allowed value (inclusive).</param>
        /// <returns>The validated double entered by the user.</returns>
        public static double ReadDouble(string prompt, double minValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                bool parsedCurrentCulture = double.TryParse(
                    input,
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out double currentCultureValue
                );
                bool parsedInvariantCulture = double.TryParse(
                    input,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double invariantCultureValue
                );

                double value = parsedCurrentCulture ? currentCultureValue : invariantCultureValue;
                if ((parsedCurrentCulture || parsedInvariantCulture) && value >= minValue)
                {
                    return value;
                }

                Console.WriteLine($"Please enter a number greater than or equal to {minValue}.");
            }
        }

        /// <summary>
        /// Reads wallet balance and enforces the minimum required amount.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        /// <returns>A validated wallet balance.</returns>
        public static double ReadWalletBalance(string prompt)
        {
            const double minimumWalletBalance = 6.0;

            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                bool parsedCurrentCulture = double.TryParse(
                    input,
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out double currentCultureValue
                );
                bool parsedInvariantCulture = double.TryParse(
                    input,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double invariantCultureValue
                );

                if (!parsedCurrentCulture && !parsedInvariantCulture)
                {
                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                    continue;
                }

                double value = parsedCurrentCulture ? currentCultureValue : invariantCultureValue;
                if (value < minimumWalletBalance)
                {
                    Console.WriteLine("Wallet balance cannot be less than 6.");
                    continue;
                }

                return value;
            }
        }

        /// <summary>
        /// Validates whether a ride can be created based on rider wallet affordability.
        /// </summary>
        /// <param name="estimatedFare">Estimated ride fare.</param>
        /// <param name="walletBalance">Current rider wallet balance.</param>
        /// <param name="errorMessage">Validation error message if invalid.</param>
        /// <returns>True if ride creation is allowed, otherwise false.</returns>
        public static bool CanCreateRide(double estimatedFare, double walletBalance, out string errorMessage)
        {
            if (estimatedFare > walletBalance)
            {
                errorMessage =
                    $"Ride cannot be created. Estimated fare ${estimatedFare:F2} exceeds rider wallet ${walletBalance:F2}.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Validates if pricing strategy can be changed for the current ride state.
        /// </summary>
        /// <param name="currentStatus">Current ride status.</param>
        /// <param name="isPaid">Whether the ride is already paid.</param>
        /// <param name="errorMessage">Validation error message if invalid.</param>
        /// <returns>True if strategy change is allowed, otherwise false.</returns>
        public static bool CanChangePricingStrategy(string currentStatus, bool isPaid, out string errorMessage)
        {
            if (
                string.Equals(currentStatus, Ride.InProgressStatus, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentStatus, Ride.CompletedStatus, StringComparison.OrdinalIgnoreCase)
            )
            {
                errorMessage = "Pricing strategy cannot be changed once a ride reaches In Progress.";
                return false;
            }

            if (isPaid)
            {
                errorMessage = "Pricing strategy cannot be changed after payment is completed.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Validates whether the updated projected fare is affordable for the rider wallet.
        /// </summary>
        /// <param name="projectedFare">Projected fare after strategy change.</param>
        /// <param name="walletBalance">Current rider wallet balance.</param>
        /// <param name="errorMessage">Validation error message if invalid.</param>
        /// <returns>True if update is allowed, otherwise false.</returns>
        public static bool CanApplyPricingUpdate(double projectedFare, double walletBalance, out string errorMessage)
        {
            if (projectedFare > walletBalance)
            {
                errorMessage =
                    $"Pricing strategy update rejected. New fare ${projectedFare:F2} exceeds rider wallet ${walletBalance:F2}.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Validates whether payment can be processed based on rider wallet affordability.
        /// </summary>
        /// <param name="payableFare">Final payable fare.</param>
        /// <param name="walletBalance">Current rider wallet balance.</param>
        /// <param name="errorMessage">Validation error message if invalid.</param>
        /// <returns>True if payment can proceed, otherwise false.</returns>
        public static bool CanProcessPayment(double payableFare, double walletBalance, out string errorMessage)
        {
            if (payableFare > walletBalance)
            {
                errorMessage =
                    $"Payment cannot be processed. Payable fare ${payableFare:F2} exceeds rider wallet ${walletBalance:F2}.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
