using UnityEngine;

namespace BW.Devices
{
    public class QualityPreset
    {
        public string Name { get; }
        public int MemoryThreshold { private set; get; }
        public TextureQuality TextureQualityLevel { set; get; }

        public QualityPreset(string name)
        {
        }

        public void SetMemoryThreshold(int memoryMb)
        {
            if (memoryMb < 0)
            {
                Debug.LogError("Memory cannot be less than zero");
                return;
            }

            // memoryMb = memoryMb;
        }
    }
}