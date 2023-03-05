using System;
using UnityEngine;

namespace BW.Maths
{
    [Serializable]
    public class Bound2D
    {
        public static readonly Bound2D zero = new Bound2D();

        public float XLow;
        public float XHigh;
        public float YLow;
        public float YHigh;

        public float Width
        {
            get { return this.XHigh - this.XLow; }
        }

        public float Height
        {
            get { return this.YHigh - this.YLow; }
        }

        public Vector2 Center
        {
            get { return new Vector2((this.XLow + this.XHigh) / 2f, (this.YLow + this.YHigh) / 2f); }
        }

        public float CenterX => (this.XLow + this.XHigh) / 2f;

        public Bound2D()
        {
            this.XLow = 0f;
            this.XHigh = 0f;
            this.YLow = 0f;
            this.YHigh = 0f;
        }

        public Bound2D(float xLow, float xHigh, float yLow, float yHigh)
        {
            this.XLow = xLow;
            this.XHigh = xHigh;
            this.YLow = yLow;
            this.YHigh = yHigh;
        }

        public void Set(float xLow, float xHigh, float yLow, float yHigh)
        {
            this.XLow = xLow;
            this.XHigh = xHigh;
            this.YLow = yLow;
            this.YHigh = yHigh;
        }

        public bool Contains(Vector3 position)
        {
            return position.x >= this.XLow && position.x <= this.XHigh && position.y >= this.YLow && position.y <= this.YHigh;
        }

        public void Copy(Bound2D bounds)
        {
            this.XLow = bounds.XLow;
            this.XHigh = bounds.XHigh;
            this.YLow = bounds.YLow;
            this.YHigh = bounds.YHigh;
        }
    }
}