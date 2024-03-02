using UnityEngine;

namespace UnityTests
{
    public class Minimap : MonoBehaviour
    {
        /// <summary>
        /// 屏幕小地图显示大小
        /// </summary>
        [field: SerializeField]
        public float Size { get; set; } = 200;

        /// <summary>
        /// 小地图显示的区域大小
        /// </summary>
        [field: SerializeField]
        public float AreaSize { get; set; } = 10;

        /// <summary>
        /// 小地图原点位置 (跟随物体)
        /// </summary>
        [field: SerializeField]
        public GameObject Origin { get; set; }

        /// <summary>
        /// 纹理源
        /// </summary>
        [field: SerializeField]
        public MinimapTextureSource TextureSource { get; set; }

        /// <summary>
        /// 小地图相对地图纹理的缩放系数
        /// </summary>
        public float Scale => (TextureSource?.AreaSize ?? 50) / AreaSize;

        public void ViewportToWorldPoint(Vector2 point, out float x, out float z)
        {
            var localPoint = new Vector2(point.x * 2 - 1, point.y * 2 - 1);
            var relativePoint = localPoint / 2 / Scale * TextureSource.AreaSize;

            var centerPoint = new Vector3(0, 0, 0);
            if (Origin != null)
                centerPoint = Origin.transform.position;

            x = centerPoint.x + relativePoint.x;
            z = centerPoint.z + relativePoint.y;
        }

        public Vector2 WorldToViewportPoint(float x, float z)
        {
            if (TextureSource == null)
                return Vector2.zero;

            var centerPoint = new Vector3(0, 0, 0);
            if (Origin != null)
                centerPoint = Origin.transform.position;

            var relativePoint = new Vector2(x - centerPoint.x, z - centerPoint.z);
            var localPoint = relativePoint / TextureSource.AreaSize * Scale * 2;
            var viewportPoint = new Vector2((localPoint.x + 1) / 2, (localPoint.y + 1) / 2);

            return viewportPoint;
        }
    }
}
