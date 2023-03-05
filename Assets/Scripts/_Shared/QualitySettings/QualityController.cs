using System.Collections.Generic;
using UnityEngine;

namespace BW.Devices
{
    public class QualityController
    {
        private readonly List<QualityPreset> _qualityPresets;

        public QualityController()
        {
        }

        public void AddQualityPreset(QualityPreset qualityPreset)
        {
            if (!this._qualityPresets.Contains(qualityPreset)) this._qualityPresets.Add(qualityPreset);
        }

        public void AutoDetectQualityPreset()
        {
            int memory = SystemInfo.systemMemorySize;

            for (int i = 0; i < this._qualityPresets.Count - 1; i++)
            {
                var currentPreset = this._qualityPresets[i];
                var nextPreset = this._qualityPresets[i + 1];
            }
        }
    }
}