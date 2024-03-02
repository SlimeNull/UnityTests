using UnityEngine;

namespace UnityTests
{
    public class MinimapRenderTextureSource : MinimapTextureSource
    {
        [field: SerializeField]
        public Camera RenderCamera { get; set; }

        public override Texture Texture => RenderCamera?.targetTexture;

        public override float AreaSize => RenderCamera?.orthographicSize * 2 ?? 50;

        public override Vector2 Pivot
        {
            get
            {
                var worldPositionOffset = RenderCamera.transform.position;

                var pivot = new Vector2(0.5f, 0.5f);
                pivot.x -= worldPositionOffset.x / AreaSize;
                pivot.y -= worldPositionOffset.z / AreaSize;

                return pivot;
            }
        }

        private void Update()
        {
            if (RenderCamera == null)
            {
                Debug.LogWarning("No render camera");
            }
            else
            {
                var eulerAngles = RenderCamera.transform.eulerAngles;
                if (eulerAngles.x != 90 || eulerAngles.y != 0 || eulerAngles.z != 0)
                {
                    Debug.LogWarning("Invalid rotation of render camera, must be euler angles (90, 0, 0)");
                }
            }
        }
    }
}
