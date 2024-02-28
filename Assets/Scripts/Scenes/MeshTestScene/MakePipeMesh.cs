using UnityEngine;

namespace NinjaGame
{
    [RequireComponent(typeof(MeshFilter))]
    public class MakePipeMesh : MeshBuilder
    {
        /// <summary>
        /// 管道半径
        /// </summary>
        [field: SerializeField]
        public float Radius { get; set; } = 0.5f;

        /// <summary>
        /// 管道高度
        /// </summary>
        [field: SerializeField]
        public float Height { get; set; } = 2f;

        /// <summary>
        /// 管道臂厚度
        /// </summary>
        [field: SerializeField]
        public float Thickness { get; set; } = 0.2f;

        /// <summary>
        /// 圆上顶点数
        /// </summary>
        [field: SerializeField]
        public int VertexCountInCircle { get; set; } = 10;

        protected override Mesh CreateMesh()
        {
            //var halfPi = Mathf.PI / 2;
            var radius = Radius;                                              // 半径
            var innerRadius = Radius - Thickness;                             // 内圆半径
            var halfHeight = Height / 2;                                      // 高度的一半
            var negativeHalfHeight = -halfHeight;                             // 高度一半的负值
            var vertexCountInCircle = VertexCountInCircle;                    // 圆上的顶代数
            var radianGap = Mathf.PI * 2 / vertexCountInCircle;               // 圆上每两个顶点, 弧度值的间隔

            // 内壁外壁上下共 "圆上定点数 * 4" 个顶点 (不考虑法线问题
            Vector3[] vertices = new Vector3[vertexCountInCircle * 4];

            // 三角形, 内壁, 上圆, 外壁, 下圆, 每一个顶点, 两个三角形, 共 8 个三角形, *3 顶点数
            int[] triangles = new int[vertexCountInCircle * 3 * 8];

            // 各个部分的三角形在数组中的索引起始位置
            int topTriangles1Start = 0;                                        // 顶部第一个三角形
            int topTriangles2Start = vertexCountInCircle * 3;                  // 顶部第二个三角形
            int bottomTriangles1Start = vertexCountInCircle * 3 * 2;           // 底部第一个三角形
            int bottomTriangles2Start = vertexCountInCircle * 3 * 3;           // 底部第二个三角形
            int sideTriangles1Start = vertexCountInCircle * 3 * 4;             // 侧边外侧第一个三角形
            int sideTriangles2Start = vertexCountInCircle * 3 * 5;             // 侧边外测第二个三角形
            int sideTriangles3Start = vertexCountInCircle * 3 * 6;             // 侧边内测第一个三角形
            int sideTriangles4Start = vertexCountInCircle * 3 * 7;             // 侧边内测第二个三角形

            // 为圆上每个顶点角度循环
            for (int i = 0; i < vertexCountInCircle; i++)
            {
                // 三角函数值
                var cos = Mathf.Cos(radianGap * i);
                var sin = Mathf.Sin(radianGap * i);

                // 外侧顶点坐标值
                var x = cos * radius;
                var z = sin * radius;

                // 内侧顶点坐标值
                var innerX = cos * innerRadius;
                var innerZ = sin * innerRadius;

                // 当前角度, 顶部顶点, 底部顶点, 内圆顶部顶点, 内圆下部顶点, 在顶点数组中的索引
                var currentTopVertexIndex = i;
                var currentBottomVertexIndex = i + vertexCountInCircle;
                var innerCurrentTopVertexIndex = i + vertexCountInCircle * 2;
                var innerCurrentBottomVertexIndex = i + vertexCountInCircle * 3;

                // 下一个角度, 顶部顶点, 底部顶点, 内圆顶部顶点, 内圆下部顶点, 在顶点数组中的索引
                var nextTopVertexIndex = (i + 1) % vertexCountInCircle;
                var nextBottomVertexIndex = (i + 1) % vertexCountInCircle + vertexCountInCircle;
                var innerNextTopVertexIndex = (i + 1) % vertexCountInCircle + vertexCountInCircle * 2;
                var innerNextBottomVertexIndex = (i + 1) % vertexCountInCircle + vertexCountInCircle * 3;

                // 存储顶点坐标
                vertices[currentTopVertexIndex] = new Vector3(x, halfHeight, z);
                vertices[currentBottomVertexIndex] = new Vector3(x, negativeHalfHeight, z);
                vertices[innerCurrentTopVertexIndex] = new Vector3(innerX, halfHeight, innerZ);
                vertices[innerCurrentBottomVertexIndex] = new Vector3(innerX, negativeHalfHeight, innerZ);


                // 顶部三角形
                triangles[topTriangles1Start + i * 3] = currentTopVertexIndex;
                triangles[topTriangles1Start + i * 3 + 1] = innerCurrentTopVertexIndex;
                triangles[topTriangles1Start + i * 3 + 2] = innerNextTopVertexIndex;

                triangles[topTriangles2Start + i * 3] = currentTopVertexIndex;
                triangles[topTriangles2Start + i * 3 + 1] = innerNextTopVertexIndex;
                triangles[topTriangles2Start + i * 3 + 2] = nextTopVertexIndex;

                // 底部三角形
                triangles[bottomTriangles1Start + i * 3] = currentBottomVertexIndex;
                triangles[bottomTriangles1Start + i * 3 + 1] = innerNextBottomVertexIndex;
                triangles[bottomTriangles1Start + i * 3 + 2] = innerCurrentBottomVertexIndex;

                triangles[bottomTriangles2Start + i * 3] = currentBottomVertexIndex;
                triangles[bottomTriangles2Start + i * 3 + 1] = nextBottomVertexIndex;
                triangles[bottomTriangles2Start + i * 3 + 2] = innerNextBottomVertexIndex;


                // 外侧三角形
                triangles[sideTriangles1Start + i * 3] = currentBottomVertexIndex;
                triangles[sideTriangles1Start + i * 3 + 1] = currentTopVertexIndex;
                triangles[sideTriangles1Start + i * 3 + 2] = nextTopVertexIndex;

                triangles[sideTriangles2Start + i * 3] = currentBottomVertexIndex;
                triangles[sideTriangles2Start + i * 3 + 1] = nextTopVertexIndex;
                triangles[sideTriangles2Start + i * 3 + 2] = nextBottomVertexIndex;

                // 内侧三角形
                triangles[sideTriangles3Start + i * 3] = innerCurrentBottomVertexIndex;
                triangles[sideTriangles3Start + i * 3 + 1] = innerNextTopVertexIndex;
                triangles[sideTriangles3Start + i * 3 + 2] = innerCurrentTopVertexIndex;

                triangles[sideTriangles4Start + i * 3] = innerCurrentBottomVertexIndex;
                triangles[sideTriangles4Start + i * 3 + 1] = innerNextBottomVertexIndex;
                triangles[sideTriangles4Start + i * 3 + 2] = innerNextTopVertexIndex;
            }

            // 创建网格
            var newMesh = new Mesh()
            {
                vertices = vertices,
                triangles = triangles
            };

            // 重新计算发现 (但对于当前的顶点, 这方法并没什么好的作用)
            newMesh.RecalculateNormals();

            return newMesh;
        }
    }
}
