using InspectorMathExpressions;

namespace BW
{
    public class Formula : MathExpressionSerialized<Formula>
    {
        public Formula()
        {
        }

        public Formula(string formular)
        {
            this.expression = formular;
        }

        [MathExpressionEvaluator]
        public float Calculate(float baseVal, int tier)
        {
            return (float) EvaluateMathExpression(baseVal, tier);
        }
    }
}