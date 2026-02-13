namespace RideSharing.Users
{
    public abstract class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public abstract void DisplayInfo();
        public abstract string GetRole();
    }
}
