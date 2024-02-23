using UnityEngine;

namespace NinjaGame
{
    [RequireComponent(typeof(MeshFilter))]
    public class MakeCylinderMesh : MonoBehaviour
    {
        MeshFilter _meshFilter;

        [field: SerializeField]
        public float Radius { get; set; } = 0.5f;

        [field: SerializeField]
        public float Height { get; set; } = 2f;

        [field: SerializeField]
        public int VertexCountInCircle { get; set; } = 10;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            //var halfPi = Mathf.PI / 2;
            var radius = Radius;
            var halfHeight = Height / 2;
            var negativeHalfHeight = -halfHeight;
            var vertexCountInCircle = VertexCountInCircle;
            var radianGap = Mathf.PI * 2 / vertexCountInCircle;

            Vector3[] vertices = new Vector3[2 + vertexCountInCircle * 4];
            Vector2[] uv = new Vector2[2 + vertexCountInCircle * 4];
            int[] triangles = new int[vertexCountInCircle * 3 * 4];

            vertices[0] = new Vector3(0, halfHeight, 0);     // top center
            vertices[1] = new Vector3(0, -halfHeight, 0);    // bottom center
            uv[0] = new Vector2(0.125f, 0.5f);
            uv[1] = new Vector2(0.375f, 0.5f);

            int topTrianglesStart = 0;
            int bottomTrianglesStart = vertexCountInCircle * 3;
            int sideTriangles1Start = vertexCountInCircle * 3 * 2;
            int sideTriangles2Start = vertexCountInCircle * 3 * 3;
            for (int i = 0; i < vertexCountInCircle; i++)
            {
                var cos = Mathf.Cos(radianGap * i);
                var sin = Mathf.Sin(radianGap * i);
                var x = cos * radius;
                var z = sin * radius;

                // 上下面顶点
                vertices[2 + i] = new Vector3(x, halfHeight, z);
                vertices[2 + i + vertexCountInCircle] = new Vector3(x, negativeHalfHeight, z);

                // 侧边顶点
                vertices[2 + i + vertexCountInCircle * 2] = new Vector3(x, halfHeight, z);
                vertices[2 + i + vertexCountInCircle * 3] = new Vector3(x, negativeHalfHeight, z);

                var currentTopVertexIndex = 2 + i;
                var nextTopVertexIndex = 2 + (i + 1) % vertexCountInCircle;

                var currentBottomVertexIndex = 2 + i + vertexCountInCircle;
                var nextBottomVertexIndex = 2 + (i + 1) % vertexCountInCircle + vertexCountInCircle;

                var sideCurrentTopVertexIndex = 2 + i + vertexCountInCircle * 2;
                var sideNextTopVertexIndex = 2 + (i + 1) % vertexCountInCircle + vertexCountInCircle * 2;

                var sideCurrentBottomVertexIndex = 2 + i + vertexCountInCircle * 3;
                var sideNextBottomVertexIndex = 2 + (i + 1) % vertexCountInCircle + vertexCountInCircle * 3;

                // uv
                uv[currentTopVertexIndex] = new Vector2((cos + 1) / 2 / 4, (sin + 1) / 2);
                uv[currentBottomVertexIndex] = new Vector2(0.25f + (cos + 1) / 2 / 4, (sin + 1) / 2);
                uv[sideCurrentTopVertexIndex] = new Vector2(0.5f + 0.5f * ((float)i / (vertexCountInCircle - 1)), 1);
                uv[sideCurrentBottomVertexIndex] = new Vector2(0.5f + 0.5f * ((float)i / (vertexCountInCircle - 1)), 0);

                // top triangle
                triangles[topTrianglesStart + i * 3] = 0;
                triangles[topTrianglesStart + i * 3 + 1] = nextTopVertexIndex;
                triangles[topTrianglesStart + i * 3 + 2] = currentTopVertexIndex;

                // bottom triangle
                triangles[bottomTrianglesStart + i * 3] = 1;
                triangles[bottomTrianglesStart + i * 3 + 1] = currentBottomVertexIndex;
                triangles[bottomTrianglesStart + i * 3 + 2] = nextBottomVertexIndex;

                // side triangles
                triangles[sideTriangles1Start + i * 3] = sideCurrentBottomVertexIndex;
                triangles[sideTriangles1Start + i * 3 + 1] = sideCurrentTopVertexIndex;
                triangles[sideTriangles1Start + i * 3 + 2] = sideNextTopVertexIndex;

                triangles[sideTriangles2Start + i * 3] = sideCurrentBottomVertexIndex;
                triangles[sideTriangles2Start + i * 3 + 1] = sideNextTopVertexIndex;
                triangles[sideTriangles2Start + i * 3 + 2] = sideNextBottomVertexIndex;
            }

            var newMesh = new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
                uv = uv
            };

            newMesh.RecalculateNormals();

            _meshFilter.mesh = newMesh;
        }
    }
}
