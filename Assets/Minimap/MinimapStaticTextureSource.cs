using UnityEngine;

namespace UnityTests
{
    public class MinimapStaticTextureSource : MinimapTextureSource
    {
        public override Texture Texture => StaticTexture;
        public override float AreaSize => StaticAreaSize;
        public override Vector2 Pivot => StaticPivot;


        [field: SerializeField]
        public Texture StaticTexture { get; set; }

        [field: SerializeField]
        public float StaticAreaSize { get; set; } = 10;

        [field: SerializeField]
        public Vector2 StaticPivot { get; set; } = new Vector2(0.5f, 0.5f);
    }
}
