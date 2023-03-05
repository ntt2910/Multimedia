using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace BW.Maths
{
    public static class MathUtils
    {
        public static Random Random = new Random();

        // private static readonly Random SystemRandom = new Random();

        /// <summary>
        /// Return array of random int numbers which are different each other from min(inclusive) to max(inclusive) using Fisher–Yates shuffle
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="amount">Number of result number</param>
        /// <returns></returns>
        public static int[] RandomDistinctNumber(int min, int max, int amount)
        {
            if (amount > max - min + 1 || amount <= 0) return null;

            int[] domainArray = new int[max - min + 1];
            int[] resultArray = new int[amount];

            for (int i = 0; i < domainArray.Length - 1; i++)
            {
                domainArray[i] = min + i;
            }

            int tmpLength = domainArray.Length;
            for (int k = 0; k < amount; k++)
            {
                int domainIndex = UnityEngine.Random.Range(0, tmpLength);
                resultArray[k] = domainArray[domainIndex];

                int tmp = domainArray[domainIndex];
                domainArray[domainIndex] = domainArray[tmpLength - 1];
                domainArray[tmpLength - 1] = tmp;

                tmpLength--;
            }

            return resultArray;
        }

        /// <summary>
        /// Non allocation random using Fisher–Yates shuffle
        /// </summary>
        /// <param name="domainArray"></param>
        /// <param name="resultArray"></param>
        /// <returns></returns>
        public static int[] RandomDistinctNumberNonAlloc(int[] domainArray, int[] resultArray)
        {
            int tmpLength = domainArray.Length;
            for (int k = 0; k < resultArray.Length; k++)
            {
                int domainIndex = UnityEngine.Random.Range(0, tmpLength);
                resultArray[k] = domainArray[domainIndex];

                int tmp = domainArray[domainIndex];
                domainArray[domainIndex] = domainArray[tmpLength - 1];
                domainArray[tmpLength - 1] = tmp;

                tmpLength--;
            }

            return resultArray;
        }

        public static int[] RandomSubIntArray(int[] arr, int subLength)
        {
            return RandomDistinctNumber(0, arr.Length - 1, subLength);
        }

        /// <summary>
        /// Random a subset of Component[] that elements are different each other.
        /// </summary>
        /// <param name="components"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Transform[] RandomSubComponentArray(Transform[] components, int amount)
        {
            if (amount <= 0 || components == null || amount > components.Length) return null;

            int[] randomIndexs = RandomDistinctNumber(0, components.Length - 1, amount);

            Transform[] resultArray = new Transform[amount];
            for (int i = 0; i < amount; i++)
            {
                resultArray[i] = components[randomIndexs[i]];
            }

            return resultArray;
        }

        /// <summary>
        /// Random positive or negative sign
        /// </summary>
        /// <param name="chancePositive">Chance of random positive sign. 0 equals 0%, 1 equals 100%.</param>
        /// <returns>1 if positive, -1 if negative</returns>
        public static int RandomSign(float chancePositive)
        {
            return UnityEngine.Random.value <= chancePositive ? 1 : -1;
        }

        public static float RandomSignFloat(float chancePositive)
        {
            return UnityEngine.Random.value <= chancePositive ? 1f : -1f;
        }

        public static bool RandomBoolean(float trueChance)
        {
            return UnityEngine.Random.value <= trueChance;
        }

        public static bool RandomBoolean()
        {
            return UnityEngine.Random.value <= 0.5f ? true : false;
        }

        public static int[] GetChanceArray(float[] chanceArray)
        {
            int[] indexArray = new int[10];

            int tmpIndex = 0;

            for (int i = 0; i < chanceArray.Length; i++)
            {
                for (int k = 0; k < chanceArray[i] * 10f; k++)
                {
                    indexArray[tmpIndex] = i;
                    tmpIndex++;
                }
            }

            return indexArray;
        }

        public static int RandomIndexWeighted(IList<float> chances)
        {
            // Calculate sum of chance
            var sumChance = 0f;

            for (var i = 0; i < chances.Count; i++)
            {
                sumChance += chances[i];
            }

            var rand = UnityEngine.Random.Range(0f, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int i = 0; i < chances.Count; ++i)
            {
                if (chances[i] <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + chances[i];

                if (rand >= minRange && rand <= maxRange)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        public static int RandomIndexWeighted(IList<int> chances)
        {
            // Calculate sum of chance
            var sumChance = 0f;

            for (var i = 0; i < chances.Count; i++)
            {
                sumChance += chances[i];
            }

            var rand = UnityEngine.Random.Range(0f, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int i = 0; i < chances.Count; ++i)
            {
                if (chances[i] <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + chances[i];

                if (rand >= minRange && rand <= maxRange)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        public static int RandomWeighted<T>(IList<T> weightables) where T : IWeightable
        {
            // Calculate sum of chance
            var sumChance = 0f;

            foreach (var weightable in weightables)
            {
                sumChance += weightable.Weight;
            }

            var rand = UnityEngine.Random.Range(0, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int index = 0; index < weightables.Count; ++index)
            {
                if (weightables[index].Weight <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + weightables[index].Weight;

                if (rand >= minRange && rand <= maxRange)
                {
                    result = index;
                    break;
                }
            }

            return result;
        }

        public static float RandomWeightedValue(IList<float> weightables)
        {
            // Calculate sum of chance
            var sumChance = 0f;

            foreach (var weightable in weightables)
            {
                sumChance += weightable;
            }

            var rand = UnityEngine.Random.Range(0, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int index = 0; index < weightables.Count; ++index)
            {
                if (weightables[index] <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + weightables[index];

                if (rand >= minRange && rand <= maxRange)
                {
                    result = index;
                    break;
                }
            }

            return weightables[result];
        }

        public static int RandomWeightedValue(IList<int> weightables)
        {
            // Calculate sum of chance
            var sumChance = 0f;

            foreach (var weightable in weightables)
            {
                sumChance += weightable;
            }

            var rand = UnityEngine.Random.Range(0, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int index = 0; index < weightables.Count; ++index)
            {
                if (weightables[index] <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + weightables[index];

                if (rand >= minRange && rand <= maxRange)
                {
                    result = index;
                    break;
                }
            }

            return weightables[result];
        }

        public static int RandomWeighted<T>(T[] weightables) where T : IWeightable
        {
            // Calculate sum of chance
            var sumChance = 0f;

            foreach (var weightable in weightables)
            {
                sumChance += weightable.Weight;
            }

            var rand = UnityEngine.Random.Range(0, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            var length = weightables.Length;
            for (int index = 0; index < length; ++index)
            {
                var w = weightables[index];
                if (w.Weight <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + w.Weight;

                if (rand >= minRange && rand <= maxRange)
                {
                    result = index;
                    break;
                }
            }

            return result;
        }

        public static int RandomWeighted(IWeightable[] weightables)
        {
            // Calculate sum of chance
            var sumChance = 0f;

            foreach (var weightable in weightables)
            {
                sumChance += weightable.Weight;
            }

            var rand = UnityEngine.Random.Range(0, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            var length = weightables.Length;
            for (int index = 0; index < length; ++index)
            {
                var w = weightables[index];
                if (w.Weight <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + w.Weight;

                if (rand >= minRange && rand <= maxRange)
                {
                    result = index;
                    break;
                }
            }

            return result;
        }

        public static float RandomWeightedRange(WeightDistribution[] distribution)
        {
            // Calculate sum of chance
            var sumChance = 0f;
            foreach (var d in distribution)
            {
                if (d.Weight <= 0f) continue;

                sumChance += d.Weight;
            }

            var rand = UnityEngine.Random.Range(0f, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int i = 0; i < distribution.Length; ++i)
            {
                if (distribution[i].Weight <= 0f) continue;

                minRange += maxRange;
                maxRange = minRange + distribution[i].Weight;

                if (rand >= minRange && rand <= maxRange)
                {
                    result = i;
                    break;
                }
            }

            var dis = distribution[result];
            return UnityEngine.Random.Range(dis.Min, dis.Max);
        }

        public static Object GetRandomArrayElement(Object[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length - 1)];
        }

        public static Vector3 RandomPointInBound2D(Bound2D bound)
        {
            Vector3 v = Vector3.zero;
            float randX = UnityEngine.Random.Range(bound.XLow, bound.XHigh);
            float randY = UnityEngine.Random.Range(bound.YLow, bound.YHigh);
            v.x = randX;
            v.y = randY;
            return v;
        }

        public static Vector3 RandomPointInBound2D(Bound2D bound, float offsetXLow = 0f, float offsetXHigh = 0f,
            float offsetYLow = 0f, float offsetYHigh = 0f)
        {
            Vector3 v = Vector3.zero;
            float randX = UnityEngine.Random.Range(bound.XLow + offsetXLow, bound.XHigh + offsetXHigh);
            float randY = UnityEngine.Random.Range(bound.YLow + offsetYLow, bound.YHigh + offsetYHigh);
            v.x = randX;
            v.y = randY;
            return v;
        }

        public static Vector3 Bound2D(Vector3 vec, Bound2D bound, float offsetXLow = 0f, float offsetXHigh = 0f,
            float offsetYLow = 0f, float offsetYHigh = 0f)
        {
            vec.x = Mathf.Clamp(vec.x, bound.XLow + offsetXLow, bound.XHigh + offsetXHigh);
            vec.y = Mathf.Clamp(vec.y, bound.YLow + offsetYLow, bound.YHigh + offsetYHigh);
            return vec;
        }

        public static Vector2 RandomInsideCircle(Vector2 center, float radius)
        {
            return RandomInsideCircle(center, 0.01f, radius);
        }

        public static Vector2 RandomInsideCircle(Vector2 center, float innerRadius, float outerRadius)
        {
            // Ignore center
            if (innerRadius < 0.01f) innerRadius = 0.01f;

            Vector2 point = Vector2.zero;
            float radius = UnityEngine.Random.Range(innerRadius, outerRadius);
            float angle = UnityEngine.Random.value * 2f * Mathf.PI;
            point.x = Mathf.Cos(angle) * radius;
            point.y = Mathf.Sin(angle) * radius;
            Vector2 dest = center + point;
            return dest;
        }

        public static bool IsNegativeSign(float a)
        {
            return a < 0f;
        }

        public static bool IsPositiveSign(float a)
        {
            return a > 0f;
        }

        public static bool IsSameSign(float a, float b)
        {
            return a * b >= 0f;
        }

        public static bool IsSameSignPositive(float a, float b)
        {
            return a >= 0f && b >= 0f;
        }

        public static bool IsSameSignNegative(float a, float b)
        {
            return a < 0f && b < 0f;
        }

        public static Vector3 AddNoiseDirectionVector(Vector3 v, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * v;
        }

        public static float RandomStep(float min, float max, float step)
        {
            if (min > max)
            {
                Debug.LogError("Min cannot be greater than max");
                return min;
            }

            int numberOfStep = (int) ((max - min) / step);
            int randomStep = UnityEngine.Random.Range(0, numberOfStep + 1);
            return min + randomStep * step;
        }

        /// <summary>
        /// if value == 0.2f mean 20% to get return success.
        /// </summary>
        public static bool RollChance(float value, float min = 0, float max = 1)
        {
            var chance = UnityEngine.Random.Range(0, 1);
            return chance < value;
        }

        /// <summary>
        /// if value == 0.2f mean 20% to get return success.
        /// </summary>
        public static bool RollSuccessChance(this float value, float min = 0, float max = 1)
        {
            var chance = UnityEngine.Random.Range(0, 1);
            return chance < value;
        }

        private static readonly int[] digitsValues = {1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000};

        private static readonly string[] romanDigits =
            {"I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M"};

        public static string ToRomanNumber(int number)
        {
            StringBuilder result = new StringBuilder();

            while (number > 0)
            {
                for (int i = digitsValues.Length - 1; i >= 0; i--)
                    if (number / digitsValues[i] >= 1)
                    {
                        number -= digitsValues[i];
                        result.Append(romanDigits[i]);
                        break;
                    }
            }

            return result.ToString();
        }

        // /// <summary>
        // /// Random integer from min to max + 1
        // /// </summary>
        // /// <param name="min"></param>
        // /// <param name="max"></param>
        // /// <returns></returns>
        // [Obsolete("use System.Random not Unity.random. The result is not revertable")]
        // public static int RandomInt(int min, int max)
        // {
        //     return SystemRandom.Next(min, max);
        // }

        public static Vector3[] GetPositionsInCircle(Vector3 center, float radius, int length)
        {
            Vector3[] positions = new Vector3[length];
            float angleGap = 360f / length;

            for (int i = 0; i < length; ++i)
            {
                float angle = i * angleGap;
                float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = center.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                positions[i] = new Vector3(x, y, center.z);
            }

            return positions;
        }

        public static int GreatestCommonDivisor(int a, int b)
        {
            int Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }

        /// <summary> GreatestCommonDivisor </summary>
        public static int GCD(int a, int b)
        {
            int Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }

        public static int RoundUp(float value, int rightDecimalPoints = 1)
        {
            int digit = GetNumberOfDigits(value);
            int trueDigit = (int) Mathf.Pow(10, digit - (digit - rightDecimalPoints));
            return (int) (((int) Mathf.Ceil(value / trueDigit)) * trueDigit);
        }

        public static int GetNumberOfDigits(float d)
        {
            var abs = Mathf.Abs(d);
            return abs < 1 ? 0 : (int) (Mathf.Log10((abs)) + 1);
        }
    }
}