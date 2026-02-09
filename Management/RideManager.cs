using System.Runtime.CompilerServices;

namespace RideSharing.Management
{
    public class RideManager
    {
        private static RideManager _instance = null;
        private RideManager()
        {
            
        }

        public static RideManager GetInstance()
        {
            if ( _instance == null )
            {
                _instance = new RideManager();
            }
            return _instance;
        }
    }
}