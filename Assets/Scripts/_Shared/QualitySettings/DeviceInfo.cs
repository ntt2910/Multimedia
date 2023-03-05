using UnityEngine;

namespace BW.Devices
{
    public class DeviceInfo
    {
        public DeviceClass DeviceClass { get; }
        public int SystemMemorySize => SystemInfo.systemMemorySize;
        public RuntimePlatform RuntimePlatform => Application.platform;

        private const int LowEndMemoryMb = 1024;
        private const int MidEndMemoryMb = 2048;

        public DeviceInfo()
        {
            if (SystemMemorySize <= LowEndMemoryMb)
            {
                DeviceClass = DeviceClass.LowEnd;
            }
            else if (LowEndMemoryMb <= SystemMemorySize && SystemMemorySize <= MidEndMemoryMb)
            {
                DeviceClass = DeviceClass.MidEnd;
            }
            else
            {
                DeviceClass = DeviceClass.HighEnd;
            }
        }
    }
}