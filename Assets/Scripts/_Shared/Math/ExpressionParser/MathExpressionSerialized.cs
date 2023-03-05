using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace InspectorMathExpressions
{
/* 
 * MathExpression usage example.
 * [System.Serializable]
 * public class ExperienceFormula : MathExpressionSerialized<ExperienceFormula>
 * {
 *		[MathExpressionEvaluator]
 *		public double GetExp(int level, int paramMin, int paramMax, float modifier)
 *		{
 *			return EvaluateMathExpression(level, paramMin, paramMax, modifier);
 *		}
 *	}
 *	
 */
    public abstract class MathExpressionSerialized
    {
        [SerializeField] protected string expression;

        public string Expression => this.expression;

        public abstract IList<string> ParameterNames { get; }
        public abstract IList<string> ParameterWarnings { get; }
        public abstract bool IsValid { get; }

        public abstract void ParseExpression(out string error);
        public abstract string TryParseExpression();

        protected virtual Consts Constants => Utils.BaseConstants;

        protected virtual Funcs Functions => Utils.BaseFunctions;
    }

    public abstract class MathExpressionSerialized<T> : MathExpressionSerialized where T : class, new()
    {
        private static readonly List<string> parameterNames;
        private static readonly List<string> parameterWarnings;

        public override IList<string> ParameterNames => parameterNames;

        public override IList<string> ParameterWarnings => parameterWarnings;

        private static bool isValid;

        public override bool IsValid => isValid;

        static MathExpressionSerialized()
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
                if (!(t == typeof(int) || t == typeof(double) || t == typeof(float)))
                {
                    parameterWarnings.Add(parameter.Name);
                }

                parameterNames.Add(parameter.Name);
            }

            isValid = true;
        }

        protected MathExpressionSerialized()
        {
            CRTPChecker.Check<T>(this);
        }

        private static MethodInfo GetEvaluatorMethodInfo()
        {
            MethodInfo evaluatorMethod = null;

            foreach (var method in typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
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

        public override string TryParseExpression()
        {
            ParseExpression(out var error);
            return error;
        }

        public override void ParseExpression(out string error)
        {
            try
            {
                if (this.expression == null)
                {
                    throw new ArgumentNullException("Expression is not set");
                }

                this.e = InspectorMathExpressions.Expression.Parse(this.expression, Constants, Functions);

                foreach (var p in this.e.Parameters)
                {
                    if (parameterNames.Find((string obj) => obj == p.Key) == null)
                    {
                        throw new ArgumentNullException(p.Key + " not found in Available parameters");
                    }
                }

                error = null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        protected double EvaluateMathExpression(params double[] parameters)
        {
            if (this.e == null)
            {
                ParseExpression(out var error);
                if (error != null)
                {
                    throw new ApplicationException("MathExpression evaluate error: " + error);
                }
            }

            if (parameters.Length != parameterNames.Count)
            {
                throw new ArgumentNullException(
                    $"Incorrect parameters count passed: {parameters.Length} but need {parameterNames.Count}");
            }

            var parameterIndex = 0;
            foreach (var parameter in parameters)
            {
                if (this.e.Parameters.TryGetValue(parameterNames[parameterIndex], out var param))
                {
                    param.Value = parameter;
                }

                parameterIndex++;
            }

            return this.e.Value;
        }

        [ContextMenu("Validate")]
        private void Validate()
        {
            ParseExpression(out var parseError);
            if (!string.IsNullOrEmpty(parseError))
            {
                Debug.LogError(parseError);
            }
        }
    }
}