using UnityEngine;

namespace NinjaGame
{
    public class TextureTerrian : MeshBuilder
    {
        [field: SerializeField]
        public Texture2D Texture { get; set; }

        [field: SerializeField]
        public Vector3 Offset { get; set; } = new Vector3(0, 0, 0);

        [field: SerializeField]
        public float SizeScale { get; set; } = 1;

        [field: SerializeField]
        public float HeightScale { get; set; } = 1;

        protected override Mesh CreateMesh()
        {
            int height = Texture.height;
            int width = Texture.width;

            if (width == 1 || height == 1)
                return new Mesh();

            Vector3 offset = new Vector3(-(float)width / 2, 0, -(float)height / 2);
            Vector3 userDefinedOffset = Offset;

            Vector3[] vertices = new Vector3[width * height];
            Vector2[] uvs = new Vector2[width * height];
            int[] triangles = new int[(width - 1) * (height - 1) * 2 * 3];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var vertexHeight = Texture.GetPixel(x, y).grayscale * HeightScale;
                    Vector3 vertex = new Vector3(
                        (offset.x + x) * SizeScale, 
                        vertexHeight, 
                        (offset.z + (height - y)) * SizeScale) + userDefinedOffset;

                    vertices[width * y + x] = vertex;
                    uvs[width * y + x] = new Vector2(x / (width - 1), 1 - y / (height - 1));
                }
            }

            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    var triangleIndexOffset = (width - 1) * 2 * 3 * y + x * 2 * 3;

                    var vertexX0Y0 = width * y + x;
                    var vertexX1Y0 = width * y + x + 1;
                    var vertexX0Y1 = width * (y + 1) + x;
                    var vertexX1Y1 = width * (y + 1) + x + 1;

                    triangles[triangleIndexOffset] = vertexX0Y0;
                    triangles[triangleIndexOffset + 1] = vertexX1Y0;
                    triangles[triangleIndexOffset + 2] = vertexX1Y1;

                    triangles[triangleIndexOffset + 3] = vertexX0Y0;
                    triangles[triangleIndexOffset + 4] = vertexX1Y1;
                    triangles[triangleIndexOffset + 5] = vertexX0Y1;
                }
            }

            var newMesh = new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs,
            };

            newMesh.RecalculateNormals();

            return newMesh;
        }
    }
}
