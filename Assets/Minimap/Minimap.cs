using UnityEngine;

namespace UnityTests
{
    public class Minimap : MonoBehaviour
    {
        /// <summary>
        /// 屏幕小地图显示大小
        /// </summary>
        [field: SerializeField]
        public float Size { get; set; } = 100;

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
        public float Scale
        {
            get
            {
                if (TextureSource == null)
                    return 50 / AreaSize;

                return TextureSource.AreaSize / AreaSize;
            }
        }

        /// <summary>
        /// 小地图旋转
        /// </summary>
        public float Rotation
        {
            get
            {
                if (Origin == null)
                    return 0;

                float rotation = -Origin.transform.eulerAngles.y * Mathf.Deg2Rad;
                return rotation;
            }
        }

        public void ViewportToWorldPoint(Vector2 point, out float x, out float z)
        {
            var localPoint = new Vector2(point.x * 2 - 1, point.y * 2 - 1);
            var localPointLength = localPoint.magnitude;

            var angle = Mathf.Atan2(localPoint.y, localPoint.x);
            var rotatedAngle = angle + Rotation;

            var rotatedLocalPoint = new Vector2(Mathf.Cos(rotatedAngle) * localPointLength, Mathf.Sin(rotatedAngle) * localPointLength);
            var relativePoint = rotatedLocalPoint / 2 / Scale * TextureSource.AreaSize;

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
            var rotatedLocalPoint = relativePoint / TextureSource.AreaSize * Scale * 2;
            var localPointLength = rotatedLocalPoint.magnitude;

            var rotatedAngle = Mathf.Atan2(rotatedLocalPoint.y, rotatedLocalPoint.x);
            var angle = rotatedAngle - Rotation;

            var localPoint = new Vector2(Mathf.Cos(angle) * localPointLength, Mathf.Sin(angle) * localPointLength);
            var viewportPoint = new Vector2((localPoint.x + 1) / 2, (localPoint.y + 1) / 2);

            return viewportPoint;
        }
    }
}
