using UnityEngine;

namespace BW.Devices
{
    public class DeviceProfile
    {
        public RuntimePlatform Platform { get; }
        public int PerformanceLevel { get; }

        public DeviceProfile(RuntimePlatform platform, int performanceLevel)
        {
            
        }
    }
}