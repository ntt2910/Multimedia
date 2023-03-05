using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace InspectorMathExpressions {

/* 
 * MathExpression usage example.
 * 
 * public class ExperienceFormula : MathExpression<ExperienceFormula>
 * {
 *		[MathExpressionEvaluator]
 *		public double GetExp(int level, int paramMin, int paramMax, float modifier)
 *		{
 *			return EvaluateMathExpression(level, paramMin, paramMax, modifier);
 *		}
 *	}
 *	
 */

	public abstract class MathExpression : ScriptableObject
	{
		[SerializeField]
		protected string expression;
		public string Expression { get { return this.expression; } }
		
		public abstract IList<string> ParameterNames { get; }
		public abstract IList<string> ParameterWarnings { get; }
		public abstract bool IsValid { get; }
		
		public abstract void ParseExpression(out string error);

		protected virtual Consts Constatns { get { return Utils.BaseConstants; } }
		protected virtual Funcs Functions { get { return Utils.BaseFunctions; } }
	}
	
	public abstract class MathExpression<T> :
		MathExpression
			where T : MathExpression<T>
	{
		private static readonly List<string> parameterNames;
		private static readonly List<string> parameterWarnings;
		public override IList<string> ParameterNames { get { return parameterNames.AsReadOnly(); } }
		public override IList<string> ParameterWarnings { get { return parameterWarnings.AsReadOnly(); } }
		
		private static bool isValid;
		public override bool IsValid { get { return isValid; } }
		
		static MathExpression()
		{
			parameterNames = new List<string>();
			parameterWarnings = new List<string>();
			var evaluatorMethodInfo = GetEvaluatorMethodInfo();
			if (evaluatorMethodInfo == null)
			{
				return;
			}
			
			foreach (var parameter in evaluatorMethodInfo.GetParameters())
			{
				var t = parameter.ParameterType;
				if (!(t == typeof(int) || t == typeof(double) ||  t == typeof(float))) {
					parameterWarnings.Add(parameter.Name);
				}
				parameterNames.Add(parameter.Name);
			}
			
			isValid = true;
		}
		
		protected MathExpression()
		{
			CRTPChecker.Check<T>(this);
		}
		
		private static MethodInfo GetEvaluatorMethodInfo()
		{
			MethodInfo evaluatorMethod = null;
			
			foreach (var method in typeof(T).GetMethods())
			{
				if (method.GetCustomAttributes(typeof(MathExpressionEvaluatorAttribute), false).Length != 0)
				{
					if (evaluatorMethod != null)
					{
						Debug.LogErrorFormat("Found more than one MathExpressionEvaluator method for type {0}", typeof(T).FullName);
						return null;
					}
					evaluatorMethod = method;
				}
			}
			
			if (evaluatorMethod == null)
			{
				Debug.LogErrorFormat("MathExpressionEvaluator method for type {0} not found", typeof(T).FullName);
			}
			
			return evaluatorMethod;
		}
		
		private Expression e;
		
		public override void ParseExpression(out string error)
		{
			try
			{
				if (this.expression == null)
				{
					throw new System.ArgumentNullException("Expression is not set");
				}

				this.e = InspectorMathExpressions.Expression.Parse(this.expression, Constatns, Functions);
				
				foreach (var p in this.e.Parameters)
				{
					if (parameterNames.Find((string obj) => obj == p.Key) == null)
					{
						throw new System.ArgumentNullException(p.Key + " not found in Available parameters");
					}
				}
				
				error = null;
			}
			catch (System.Exception ex)
			{
				error = ex.Message;
			}
		}
		
		protected double EvaluateMathExpression(params double[] parameters)
		{
			if (this.e == null)
			{
				string error;
				ParseExpression(out error);
				if (error != null)
				{
					throw new System.ApplicationException("MathExpression evaluate error: " + error);
				}
			}
			
			if (parameters.Length != parameterNames.Count)
			{
				throw new System.ArgumentNullException(
					string.Format("Incorrect parameters count passed: {0} but need {1}",
				              parameters.Length, parameterNames.Count));
			}
			
			int parameterIndex = 0;
			foreach (var parameter in parameters)
			{
				Parameter param;
				if (this.e.Parameters.TryGetValue(parameterNames[parameterIndex], out param))
				{
					param.Value = parameter;
				}
				parameterIndex++;
			}
			
			return this.e.Value;
		}
	}

}
