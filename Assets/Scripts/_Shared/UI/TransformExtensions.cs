using UnityEngine;

namespace BW
{
    public static partial class TransformExtensions
    {
        public static void SetParentUI(this Transform transform, RectTransform parent)
        {
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
        }

        public static void SetParentUI(this Transform transform, Transform parent)
        {
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
        }

        public static RectTransform rectTransform(this Transform transform)
        {
            return transform as RectTransform;
        }
    }
}