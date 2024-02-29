using UnityEngine;

namespace NinjaGame
{
    public class PerlinNoise
    {

    }

    public class PerlinTerrain : SampleTerrian
    {
        [field: SerializeField]
        public int Size { get; set; } = 32;

        [field: SerializeField]
        public int Octaves { get; set; }

        [field: SerializeField]
        public Vector2 PerlinOffset { get; set; } = new();

        [field: SerializeField]
        public Vector2 PerlinScale { get; set; } = new Vector2(1, 1);

        protected override int GetXEnd() => Size;
        protected override int GetXStart() => 0;
        protected override int GetZEnd() => Size;
        protected override int GetZStart() => 0;
        protected override float Sample(int x, int z)
        {
            float value = Mathf.PerlinNoise((PerlinOffset.x + (float)x * PerlinScale.x) / Size, (PerlinOffset.x + (float)z * PerlinScale.y) / Size);
            float total = 1;

            float nowPerlinSize = 1;
            for (int i = 0; i < Octaves; i++)
            {
                nowPerlinSize /= 2;
                value += Mathf.PerlinNoise(PerlinOffset.x + x, PerlinOffset.y + z) / nowPerlinSize;
                total += nowPerlinSize;
            }

            float finalValue = value / total;

            return finalValue;
        }
    }
}
