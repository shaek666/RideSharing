namespace RideSharing.Users
{
    public abstract class User
    {
        public Guid Id { get; set; }
        public string Name { get; set;}
        public string Phone { get; set; }

        public abstract void DisplayInfo();
        public abstract string GetRole();
    }
}