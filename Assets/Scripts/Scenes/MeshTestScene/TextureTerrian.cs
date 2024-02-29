using UnityEngine;

namespace NinjaGame
{

    public class TextureTerrian : SampleTerrian
    {
        /// <summary>
        /// 高度纹理
        /// </summary>
        [field: SerializeField]
        public Texture2D Texture { get; set; }

        protected override int GetXEnd() => Texture.width;
        protected override int GetXStart() => 0;
        protected override int GetZEnd() => Texture.height;
        protected override int GetZStart() => 0;
        protected override float Sample(int x, int z) => Texture.GetPixel(x, Texture.height - z - 1).grayscale;
    }
}
