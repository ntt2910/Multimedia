namespace BW.Maths
{
    public class WeightDistribution
    {
        private float _min;
        private float _max;
        private float _weight;

        public float Min { get { return this._min; } }
        public float Max { get { return this._max; } }
        public float Weight { set { this._weight = value; } get { return this._weight; } }

        public WeightDistribution(float from, float to, float weight)
        {
            this._min = from;
            this._max = to;
            this._weight = weight;
        }
        
        /// <summary>
        /// Parsed from string (2 ; 2.3; 3.0  ; 3.2)
        /// </summary>
        /// <param name="value"></param>
        public WeightDistribution(string value)
        {
            // Pattern (min, max, weight)
            value = value.Replace("(", "").Replace(")", "").Replace(" ", string.Empty);
            var split = value.Split(';');
            float.TryParse(split[0], out this._min);
            float.TryParse(split[1], out this._max);
            float.TryParse(split[2], out this._weight);
        }
    }
}