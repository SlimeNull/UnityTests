using UnityEngine;

namespace UnityTests
{
    public abstract class SampleTerrian : MeshBuilder
    {
        /// <summary>
        /// 地形位置偏移
        /// </summary>
        [field: SerializeField]
        public Vector3 Offset { get; set; } = new Vector3(0, 0, 0);

        /// <summary>
        /// 地形大小缩放
        /// </summary>
        [field: SerializeField]
        public float SizeScale { get; set; } = 1;

        /// <summary>
        /// 地形高度缩放
        /// </summary>
        [field: SerializeField]
        public float HeightScale { get; set; } = 1;

        protected abstract int GetXStart();
        protected abstract int GetXEnd();
        protected abstract int GetZStart();
        protected abstract int GetZEnd();

        protected abstract float Sample(int x, int z);

        protected override Mesh CreateMesh()
        {
            int xEnd = GetXEnd();
            int zEnd = GetZEnd();
            int xStart = GetXStart();
            int zStart = GetZStart();
            int xOffset = xEnd - xStart;
            int zOffset = zEnd - zStart;

            if (xEnd == 1 || zEnd == 1)
                return new Mesh();

            Vector3 offset = new Vector3((float)(xStart - xEnd) / 2, 0, (float)(zStart - zEnd) / 2);
            Vector3 userDefinedOffset = Offset;

            Vector3[] vertices = new Vector3[xOffset * zOffset];
            Vector2[] uvs = new Vector2[xOffset * zOffset];
            int[] triangles = new int[(xOffset - 1) * (zOffset - 1) * 2 * 3];

            for (int z = 0; z < xOffset; z++)
            {
                for (int x = 0; x < xOffset; x++)
                {
                    // 采样
                    var sample = Sample(xStart + x, zStart + z);

                    // 顶点坐标
                    // 由于图像使用的是屏幕坐标系, 所以需要把 y 反转赋值给 z, 以保证形状看起来和图像一致
                    Vector3 vertex = new Vector3(
                        (offset.x + x) * SizeScale,
                        sample * HeightScale,
                        (offset.z + z) * SizeScale) + userDefinedOffset;

                    // 顶点以及 UV
                    vertices[xOffset * z + x] = vertex;
                    uvs[xOffset * z + x] = new Vector2(x / (xOffset - 1), z / (xOffset - 1));
                }
            }

            for (int y = 0; y < zOffset - 1; y++)
            {
                for (int x = 0; x < xOffset - 1; x++)
                {
                    var triangleIndexOffset = (xOffset - 1) * 2 * 3 * y + x * 2 * 3;

                    // 顶点的坐标, 当前顶点, 当前顶点旁边(右侧), 以及前面两个顶点 y + 1 之后的两个顶点
                    var vertexX0Y0 = xOffset * y + x;
                    var vertexX1Y0 = xOffset * y + x + 1;
                    var vertexX0Y1 = xOffset * (y + 1) + x;
                    var vertexX1Y1 = xOffset * (y + 1) + x + 1;

                    // 三角形
                    triangles[triangleIndexOffset] = vertexX0Y0;
                    triangles[triangleIndexOffset + 1] = vertexX0Y1;
                    triangles[triangleIndexOffset + 2] = vertexX1Y1;

                    triangles[triangleIndexOffset + 3] = vertexX0Y0;
                    triangles[triangleIndexOffset + 4] = vertexX1Y1;
                    triangles[triangleIndexOffset + 5] = vertexX1Y0;
                }
            }

            var newMesh = new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs,
            };

            // 计算法线
            newMesh.RecalculateNormals();

            return newMesh;
        }
    }
}
