
using UnityEngine;

namespace NinjaGame.Assets.Scripts
{
    static class RectTransformUtilityEx
    {
        public static Vector2 GetSize(RectTransform rect)
        {
            if (rect.parent is not RectTransform parentRect)
                return rect.sizeDelta;

            var parentSize = GetSize(parentRect);

            return new Vector2(
                parentSize.x * (rect.anchorMax.x - rect.anchorMin.x) + rect.sizeDelta.x,
                parentSize.y * (rect.anchorMax.y - rect.anchorMin.y) + rect.sizeDelta.y);
        }
    }
}
