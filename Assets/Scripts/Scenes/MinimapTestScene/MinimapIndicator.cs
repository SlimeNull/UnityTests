using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityTests
{
    /// <summary>
    /// 小地图的标记显示
    /// </summary>
    public class MinimapIndicator : MonoBehaviour, IScrollHandler
    {
        [field: SerializeField]
        public Camera MinimapCamera { get; set; }

        [field: SerializeField]
        public GameObject Player { get; set; }

        [field: SerializeField]
        public GameObject PlayerImage { get; set; }

        [field: SerializeField]
        public float Size { get; set; } = 100;

        [field: SerializeField]
        public bool AllowScale = true;

        [field: SerializeField]
        public float MinScale = 0.1f;

        [field: SerializeField]
        public float MaxScale = 10f;

        private void Update()
        {
            if (Player != null && PlayerImage != null)
            {
                var radius = Size / 2;

                // 计算玩家图标位置
                var viewportPosition = MinimapCamera.WorldToViewportPoint(Player.transform.position);
                var localPosition = new Vector2(viewportPosition.x * 2 - 1, viewportPosition.y * 2 - 1) * radius;

                // 判断是否超出范围
                var outOfBounds =
                    localPosition.x > radius || localPosition.x < -radius ||
                    localPosition.y > radius || localPosition.y < -radius;

                // 判断坐标是否有效
                var available = !outOfBounds &&
                    !float.IsNaN(localPosition.x) &&
                    !float.IsNaN(localPosition.y);

                PlayerImage.SetActive(available);

                if (available)
                {
                    // 玩家图标位置以及旋转
                    PlayerImage.transform.localPosition = localPosition;
                    PlayerImage.transform.localEulerAngles = new Vector3(0, 0, -(Player.transform.eulerAngles.y - MinimapCamera.transform.eulerAngles.y));
                }
            }
        }

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (!AllowScale || MinimapCamera == null)
                return;

            // 地图缩放
            var newSize = Mathf.Clamp(MinimapCamera.orthographicSize + eventData.scrollDelta.y, MinScale, MaxScale);
            MinimapCamera.orthographicSize = newSize;
        }
    }
}
