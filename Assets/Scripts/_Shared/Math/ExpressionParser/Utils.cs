using UniLinq;

namespace InspectorMathExpressions {

	public static class Utils {

		public const char kDiceNotationOperator = '@';
		public const char kZeroBiasNotationOperator = '#';

		public static Funcs BaseFunctions {
			get {
				var funcs = new Funcs();
				funcs.Add("f_sqrt", (p) => System.Math.Sqrt(p.FirstOrDefault()));
				funcs.Add("f_abs", (p) => System.Math.Abs(p.FirstOrDefault()));
				funcs.Add("f_ln", (p) => System.Math.Log(p.FirstOrDefault()));
				funcs.Add("f_floor", (p) => System.Math.Floor(p.FirstOrDefault()));
				funcs.Add("f_ceiling", (p) => System.Math.Ceiling(p.FirstOrDefault()));
				funcs.Add("f_round", (p) => System.Math.Round(p.FirstOrDefault()));
				
				funcs.Add("f_sin", (p) => System.Math.Sin(p.FirstOrDefault()));
				funcs.Add("f_cos", (p) => System.Math.Cos(p.FirstOrDefault()));
				funcs.Add("f_tan", (p) => System.Math.Tan(p.FirstOrDefault()));
				
				funcs.Add("f_asin", (p) => System.Math.Asin(p.FirstOrDefault()));
				funcs.Add("f_acos", (p) => System.Math.Acos(p.FirstOrDefault()));
				funcs.Add("f_atan", (p) => System.Math.Atan(p.FirstOrDefault()));
				funcs.Add("f_atan2", (p) => System.Math.Atan2(p.FirstOrDefault(),p.ElementAtOrDefault(1)));
				
				funcs.Add("f_min", (p) => System.Math.Min(p.FirstOrDefault(), p.ElementAtOrDefault(1)));
				funcs.Add("f_max", (p) => System.Math.Max(p.FirstOrDefault(), p.ElementAtOrDefault(1)));
				funcs.Add("f_clamp", (p) => System.Math.Min(System.Math.Max(p.FirstOrDefault(), p.ElementAtOrDefault(1)), p.ElementAtOrDefault(2)));
				funcs.Add("f_clamp01", (p) => System.Math.Min(System.Math.Max(p.FirstOrDefault(),0),1));
				var rnd = new System.Random();
				funcs.Add("f_rnd", (p) => {
					if (p.Length == 2)
						return p[0] + rnd.NextDouble() * (p[1] - p[0]);
					if (p.Length == 1)
						return rnd.NextDouble() * p[0];
					return rnd.NextDouble();
				});
				return funcs;
			}
		}
		public static Consts BaseConstants {
			get {
				var c = new Consts();
				c.Add("k_pi", () => System.Math.PI);
				c.Add("k_e", () => System.Math.E);
				return c;
			}
		}
	}

}
