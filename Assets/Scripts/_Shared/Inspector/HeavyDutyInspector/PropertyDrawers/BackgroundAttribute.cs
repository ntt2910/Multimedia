//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
using UnityEngine;

namespace BW.Inspector
{

	public class BackgroundAttribute : PropertyAttribute {

		public Color color
		{
			get;
			private set;
		}

		/// <summary>
		/// Add a solid color background to the variable it is applied to.
		/// </summary>
		/// <param name="color">Color.</param>
		public BackgroundAttribute(ColorEnum color)
		{
			this.color = ColorEx.GetColorByEnum(color);

			// Always display last to make sure it is applied to the variable, not another DecoratorDrawer
			order = int.MaxValue;
		}

		/// <summary>
		/// Add a solid color background to the variable it is applied to.
		/// </summary>
		/// <param name="r">The color's red component.</param>
		/// <param name="g">The color's  green component.</param>
		/// <param name="b">The color's  blue component.</param>
		public BackgroundAttribute(float r, float g, float b)
		{
			color = new Color(r, g, b);
		}
	}
	
}
#endif
	