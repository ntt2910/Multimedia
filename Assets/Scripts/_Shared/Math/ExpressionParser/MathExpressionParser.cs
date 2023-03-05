using System.Collections.Generic;
using UniLinq;

namespace InspectorMathExpressions
{
	public interface IValue
	{
		double Value { get; }
	}
	public class Number : IValue
	{
		private double m_Value;
		public double Value
		{
			get { return this.m_Value; }
			set { this.m_Value = value; }
		}
		public Number(double aValue)
		{
			this.m_Value = aValue;
		}
		public override string ToString()
		{
			return "" + this.m_Value + "";
		}
	}
	public class OperationSum : IValue
	{
		private IValue[] m_Values;
		public double Value
		{
			get { 
				double sum = 0;
				foreach(var v in this.m_Values) {
					sum += v.Value;
				}
				return sum;
			}
		}
		public OperationSum(params IValue[] aValues)
		{
			// collapse unnecessary nested sum operations.
			List<IValue> v = new List<IValue>(aValues.Length);
			foreach (var I in aValues)
			{
				var sum = I as OperationSum;
				if (sum == null)
					v.Add(I);
				else
					v.AddRange(sum.m_Values);
			}

			this.m_Values = v.ToArray();
		}
		public override string ToString()
		{
			var list = new List<string>();
			foreach(var v in this.m_Values) {
				list.Add(v.ToString());
			}
			return "( " + string.Join(" + ", list.ToArray() ) + " )";
		}
	}
	public class OperationProduct : IValue
	{
		private IValue[] m_Values;
		public double Value
		{
			get { 
				var prod = 1.0;
				foreach(var v in this.m_Values) {
					prod*= v.Value;
				}
				return prod;
			}
		}
		public OperationProduct(params IValue[] aValues)
		{
			this.m_Values = aValues;
		}
		public override string ToString()
		{
			var list = new List<string>();
			foreach(var v in this.m_Values) {
				list.Add(v.ToString());
			}
			return "( " + string.Join(" * ", list.ToArray()) + " )";
		}
		
	}
	public class OperationPower : IValue
	{
		private IValue m_Value;
		private IValue m_Power;
		public double Value
		{
			get { return System.Math.Pow(this.m_Value.Value, this.m_Power.Value); }
		}
		public OperationPower(IValue aValue, IValue aPower)
		{
			this.m_Value = aValue;
			this.m_Power = aPower;
		}
		public override string ToString()
		{
			return "( " + this.m_Value + "^" + this.m_Power + " )";
		}
		
	}
	public class OperationDiceRoll : IValue
	{
		private IValue m_DiceCount;
		private IValue m_DiceFaces;
		static System.Random rnd = new System.Random();
		public double Value
		{
			get { 

				double result = 0;
				var count = this.m_DiceCount.Value;
				var faces = (int) this.m_DiceFaces.Value;
				for(var i = 0; i < count; i++) {
					result += rnd.Next(1,faces+1);
				}
				return result;
			}
		}
		public OperationDiceRoll(IValue aValue, IValue aPower)
		{
			this.m_DiceCount = aValue;
			this.m_DiceFaces = aPower;
		}
		public override string ToString()
		{
			return "( " + this.m_DiceCount + Utils.kDiceNotationOperator + this.m_DiceFaces + " )";
		}
		
	}
	
	public class OperationZeroBiasRoll : IValue
	{
		private IValue m_DiceCount;
		private IValue m_DiceFaces;
		static System.Random rnd = new System.Random();
		public double Value
		{
			get {
				double result = 0;
				var count = this.m_DiceCount.Value;
				var faces = (int) this.m_DiceFaces.Value;
				for(var i = 0; i < count; i++) {
					result += rnd.Next(0,faces);
				}
				return result;
			}
		}
		public OperationZeroBiasRoll(IValue aValue, IValue aPower)
		{
			this.m_DiceCount = aValue;
			this.m_DiceFaces = aPower;
		}
		public override string ToString()
		{
			return "( " + this.m_DiceCount + Utils.kZeroBiasNotationOperator + this.m_DiceFaces + " )";
		}
		
	}


	public class OperationNegate : IValue
	{
		private IValue m_Value;
		public double Value
		{
			get { return -this.m_Value.Value; }
		}
		public OperationNegate(IValue aValue)
		{
			this.m_Value = aValue;
		}
		public override string ToString()
		{
			return "( -" + this.m_Value + " )";
		}
		
	}
	public class OperationReciprocal : IValue
	{
		private IValue m_Value;
		public double Value
		{
			get { return 1.0 / this.m_Value.Value; }
		}
		public OperationReciprocal(IValue aValue)
		{
			this.m_Value = aValue;
		}
		public override string ToString()
		{
			return "( 1/" + this.m_Value + " )";
		}
		
	}
	
	public class MultiParameterList : IValue
	{
		private IValue[] m_Values;
		public IValue[] Parameters { get { return this.m_Values; } }
		public double Value
		{
			get { 
				if (this.m_Values.Length == 0) {
					return 0;
				}
				return this.m_Values[0].Value;
			}
		}
		public MultiParameterList(params IValue[] aValues)
		{
			this.m_Values = aValues;
		}
		public override string ToString()
		{
			var list = new List<string>();
			foreach(var v in this.m_Values) {
				list.Add(v.ToString());
			}
			return string.Join(", ", list.ToArray());
		}
	}
	
	public class CustomFunction : IValue
	{
		private IValue[] m_Params;
		private System.Func<double[], double> m_Delegate;
		private string m_Name;
		public double Value
		{
			get
			{
				if (this.m_Params == null)
					return this.m_Delegate(null);
				var list = new List<double>();
				foreach(var p in this.m_Params) {
					list.Add(p.Value);
				}
				return this.m_Delegate(list.ToArray());
			}
		}
		public CustomFunction(string aName, System.Func<double[], double> aDelegate, params IValue[] aValues)
		{
			this.m_Delegate = aDelegate;
			this.m_Params = aValues;
			this.m_Name = aName;
		}
		public override string ToString()
		{
			if (this.m_Params == null)
				return this.m_Name;
			var list = new List<string>();
			foreach(var p in this.m_Params) {
				list.Add(p.ToString());
			}
			return this.m_Name + "( " + string.Join(", ", list.ToArray()) + " )";
		}
	}
	public class Parameter : Number
	{
		public string Name { get; private set; }
		public override string ToString()
		{
			return Name+"["+base.ToString()+"]";
		}
		public Parameter(string aName) : base(0)
		{
			Name = aName;
		}
	}
	
	public class Expression : IValue
	{
		public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
		public IValue ExpressionTree { get; set; }
		public double Value
		{
			get { return ExpressionTree.Value; }
		}
		public double[] MultiValue
		{
			get {
				var t = ExpressionTree as MultiParameterList;
				if (t != null)
				{
					double[] res = new double[t.Parameters.Length];
					for (int i = 0; i < res.Length; i++)
						res[i] = t.Parameters[i].Value;
					return res;
				}
				return null;
			}
		}
		public override string ToString()
		{
			return ExpressionTree.ToString();
		}
		public ExpressionDelegate ToDelegate(params string[] aParamOrder)
		{
			var parameters = new List<Parameter>(aParamOrder.Length);
			for(int i = 0; i < aParamOrder.Length; i++)
			{
				if (this.Parameters.ContainsKey(aParamOrder[i]))
					parameters.Add(this.Parameters[aParamOrder[i]]);
				else
					parameters.Add(null);
			}
			var parameters2 = parameters.ToArray();
			
			return (p) => Invoke(p, parameters2);
		}
		public MultiResultDelegate ToMultiResultDelegate(params string[] aParamOrder)
		{
			var parameters = new List<Parameter>(aParamOrder.Length);
			for (int i = 0; i < aParamOrder.Length; i++)
			{
				if (this.Parameters.ContainsKey(aParamOrder[i]))
					parameters.Add(this.Parameters[aParamOrder[i]]);
				else
					parameters.Add(null);
			}
			var parameters2 = parameters.ToArray();
			
			
			return (p) => InvokeMultiResult(p, parameters2);
		}
		double Invoke(double[] aParams, Parameter[] aParamList)
		{
			int count = System.Math.Min(aParamList.Length, aParams.Length);
			for (int i = 0; i < count; i++ )
			{
				if (aParamList[i] != null)
					aParamList[i].Value = aParams[i];
			}
			return Value;
		}
		double[] InvokeMultiResult(double[] aParams, Parameter[] aParamList)
		{
			int count = System.Math.Min(aParamList.Length, aParams.Length);
			for (int i = 0; i < count; i++)
			{
				if (aParamList[i] != null)
					aParamList[i].Value = aParams[i];
			}
			return MultiValue;
		}
		public static Expression Parse(string aExpression, Consts consts, Funcs funcs)
		{
			return new ExpressionParser(consts, funcs).EvaluateExpression(aExpression);
		}
		
		public class ParameterException : System.Exception { public ParameterException(string aMessage) : base(aMessage) { } }
	}
	public delegate double ExpressionDelegate(params double[] aParams);
	public delegate double[] MultiResultDelegate(params double[] aParams);

	public class Funcs : Dictionary<string, System.Func<double[], double>> {}
	public class Consts : Dictionary<string, System.Func<double>> {}
	
	public class ExpressionParser
	{
		private List<string> m_BracketHeap = new List<string>();
		private Consts m_Consts;
		private Funcs m_Funcs;
		private Expression m_Context;
		
		public ExpressionParser(Consts consts, Funcs funcs)
		{
			this.m_Consts = consts;
			this.m_Funcs = funcs;
		}
		
		public void AddFunc(string aName, System.Func<double[],double> aMethod)
		{
			this.m_Funcs[aName] = aMethod;
		}
		
		public void AddConst(string aName, System.Func<double> aMethod)
		{
			this.m_Consts[aName] = aMethod;
		}
		public void RemoveFunc(string aName)
		{
			if (this.m_Funcs.ContainsKey(aName)) this.m_Funcs.Remove(aName);
		}
		public void RemoveConst(string aName)
		{
			if (this.m_Consts.ContainsKey(aName)) this.m_Consts.Remove(aName);
		}
		
		int FindClosingBracket(ref string aText, int aStart, char aOpen, char aClose)
		{
			int counter = 0;
			for (int i = aStart; i < aText.Length; i++)
			{
				if (aText[i] == aOpen)
					counter++;
				if (aText[i] == aClose)
					counter--;
				if (counter == 0)
					return i;
			}
			return -1;
		}
		
		void SubstitudeBracket(ref string aExpression, int aIndex)
		{
			int closing = FindClosingBracket(ref aExpression, aIndex, '(', ')');
			if (closing > aIndex + 1)
			{
				string inner = aExpression.Substring(aIndex + 1, closing - aIndex - 1);
				this.m_BracketHeap.Add(inner);
				string sub = "&" + (this.m_BracketHeap.Count - 1) + ";";
				aExpression = aExpression.Substring(0, aIndex) + sub + aExpression.Substring(closing + 1);
			}
			else {
				throw new ParseException("Bracket not closed!");
			}
		}
		
		IValue Parse(string aExpression)
		{
			aExpression = aExpression.Trim();
			int index = aExpression.IndexOf('(');
			while (index >= 0)
			{
				SubstitudeBracket(ref aExpression, index);
				index = aExpression.IndexOf('(');
			}
			if (aExpression.Contains(','))
			{
				string[] parts = aExpression.Split(',');
				List<IValue> exp = new List<IValue>(parts.Length);
				for (int i = 0; i < parts.Length; i++)
				{
					string s = parts[i].Trim();
					if (!string.IsNullOrEmpty(s))
						exp.Add(Parse(s));
				}
				return new MultiParameterList(exp.ToArray());
			}
			else if (aExpression.Contains('+'))
			{
				string[] parts = aExpression.Split('+');
				List<IValue> exp = new List<IValue>(parts.Length);
				for (int i = 0; i < parts.Length; i++)
				{
					string s = parts[i].Trim();
					if (!string.IsNullOrEmpty(s))
						exp.Add(Parse(s));
				}
				if (exp.Count == 1)
					return exp[0];
				return new OperationSum(exp.ToArray());
			}
			else if (aExpression.Contains('-'))
			{
				string[] parts = aExpression.Split('-');
				List<IValue> exp = new List<IValue>(parts.Length);
				if (!string.IsNullOrEmpty(parts[0].Trim()))
					exp.Add(Parse(parts[0]));
				for (int i = 1; i < parts.Length; i++)
				{
					string s = parts[i].Trim();
					if (!string.IsNullOrEmpty(s))
						exp.Add(new OperationNegate(Parse(s)));
				}
				if (exp.Count == 1)
					return exp[0];
				return new OperationSum(exp.ToArray());
			}
			else if (aExpression.Contains('*'))
			{
				string[] parts = aExpression.Split('*');
				List<IValue> exp = new List<IValue>(parts.Length);
				for (int i = 0; i < parts.Length; i++)
				{
					exp.Add(Parse(parts[i]));
				}
				if (exp.Count == 1)
					return exp[0];
				return new OperationProduct(exp.ToArray());
			}
			else if (aExpression.Contains('/'))
			{
				string[] parts = aExpression.Split('/');
				List<IValue> exp = new List<IValue>(parts.Length);
				if (!string.IsNullOrEmpty(parts[0].Trim()))
					exp.Add(Parse(parts[0]));
				for (int i = 1; i < parts.Length; i++)
				{
					string s = parts[i].Trim();
					if (!string.IsNullOrEmpty(s))
						exp.Add(new OperationReciprocal(Parse(s)));
				}
				return new OperationProduct(exp.ToArray());
			}
			else if (aExpression.Contains('^'))
			{
				int pos = aExpression.IndexOf('^');
				var val = Parse(aExpression.Substring(0, pos));
				var pow = Parse(aExpression.Substring(pos + 1));
				return new OperationPower(val, pow);
			}
			else if (aExpression.Contains(Utils.kDiceNotationOperator))
			{
				int pos = aExpression.IndexOf(Utils.kDiceNotationOperator);
				var val = Parse(aExpression.Substring(0, pos));
				var pow = Parse(aExpression.Substring(pos + 1));
				return new OperationDiceRoll(val, pow);
			}
			else if (aExpression.Contains(Utils.kZeroBiasNotationOperator))
			{
				int pos = aExpression.IndexOf(Utils.kZeroBiasNotationOperator);
				var val = Parse(aExpression.Substring(0, pos));
				var pow = Parse(aExpression.Substring(pos + 1));
				return new OperationZeroBiasRoll(val, pow);
			}
			foreach (var M in this.m_Funcs)
			{
				if (aExpression.StartsWith(M.Key) && StartWithDelimiter(aExpression.Substring(M.Key.Length)))
				{
					var inner = aExpression.Substring(M.Key.Length);
					var param = Parse(inner);
					var multiParams = param as MultiParameterList;
					IValue[] parameters;
					if (multiParams != null)
						parameters = multiParams.Parameters;
					else
					parameters = new IValue[] { param };
					return new CustomFunction(M.Key, M.Value, parameters);
				}
			}
			foreach (var C in this.m_Consts)
			{
				if (aExpression.StartsWith(C.Key))
				{
					return new CustomFunction(C.Key,(p)=>C.Value(),null);
				}
			}
			int index2a = aExpression.IndexOf('&');
			int index2b = aExpression.IndexOf(';');
			if (index2a >= 0 && index2b >= 2)
			{
				var inner = aExpression.Substring(index2a + 1, index2b - index2a - 1);
				int bracketIndex;
				if (int.TryParse(inner, out bracketIndex) && bracketIndex >= 0 && bracketIndex < this.m_BracketHeap.Count)
				{
					return Parse(this.m_BracketHeap[bracketIndex]);
				}
				else {
					throw new ParseException("Can't parse substitude token");
				}
			}
			double doubleValue;
			if (double.TryParse(aExpression, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out doubleValue))
			{
				return new Number(doubleValue);
			}
			if (ValidIdentifier(aExpression))
			{
				if (this.m_Context.Parameters.ContainsKey(aExpression))
					return this.m_Context.Parameters[aExpression];
				var val = new Parameter(aExpression);
				this.m_Context.Parameters.Add(aExpression, val);
				return val;
			}
			throw new ParseException("Reached unexpected end within the parsing tree");
		}

		bool StartWithDelimiter(string aExpression) {
			if (string.IsNullOrEmpty(aExpression))
				return true;
			if (aExpression.Length < 1)
				return true;
			return !"abcdefghijklmnopqrstuvwxyz§$_0123456789".Contains(char.ToLower(aExpression[0]));
		}
		
		private bool ValidIdentifier(string aExpression)
		{
			aExpression = aExpression.Trim();
			if (string.IsNullOrEmpty(aExpression))
				return false;
			if (aExpression.Length < 1)
				return false;
			if (aExpression.Contains(" "))
				return false;
			if (!"abcdefghijklmnopqrstuvwxyz§$".Contains(char.ToLower(aExpression[0])))
				return false;
			if (this.m_Consts.ContainsKey(aExpression))
				return false;
			if (this.m_Funcs.ContainsKey(aExpression))
				return false;
			return true;
		}
		
		public Expression EvaluateExpression(string aExpression)
		{
			var val = new Expression();
			this.m_Context = val;
			val.ExpressionTree = Parse(aExpression);
			this.m_Context = null;
			this.m_BracketHeap.Clear();
			return val;
		}
		
		public double Evaluate(string aExpression)
		{
			return EvaluateExpression(aExpression).Value;
		}
		public static double Eval(string aExpression)
		{
			return new ExpressionParser(Utils.BaseConstants, Utils.BaseFunctions).Evaluate(aExpression);
		}
		
		public class ParseException : System.Exception { public ParseException(string aMessage) : base(aMessage) { } }
	}
}
