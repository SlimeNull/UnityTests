using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NinjaGame
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class SpiderWeb : MaskableGraphic
    {
        /// <summary>
        /// 蛛网大小
        /// </summary>
        [field: SerializeField]
        public float Size { get; set; } = 100;

        /// <summary>
        /// 顶点数量
        /// </summary>
        [field: SerializeField]
        public int VertexCount { get; set; } = 6;

        /// <summary>
        /// 线条数量
        /// </summary>
        [field: SerializeField]
        public int LineCount { get; set; } = 5;

        /// <summary>
        /// 线条厚度
        /// </summary>
        [field: SerializeField]
        public float LineThickness { get; set; } = 1;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var radius = Size / 2;                            // 半径
            var vertexCount = VertexCount;                    // 顶点数量
            var lineCount = LineCount;                        // 线条数量
            var lineThickness = LineThickness;                // 线条厚度
            var halfLineThickness = lineThickness / 2;        // 线条厚度的一半
            var radianGap = Mathf.PI * 2 / vertexCount;       // 弧度间隔

            vh.Clear();

            // 循环每一条线
            for (int i = 0; i < lineCount; i++)
            {
                var outerRadius = radius - (radius * i / lineCount);                     // 这条线外部半径
                var innerRadius = radius - (radius * i / lineCount) - lineThickness;     // 这条线内部半径
                var lineVertexIndexOffset = vh.currentVertCount;                         // 当前边顶点索引偏移量

                // 循环每一个顶点
                for (int j = 0; j < vertexCount; j++)
                {
                    var radian = radianGap * j;            // 当前顶点角度
                    var cos = Mathf.Cos(radian);           // COS 值
                    var sin = Mathf.Sin(radian);           // SIN 值

                    var outerX = cos * outerRadius;        // 外部顶点的 X 坐标
                    var outerY = sin * outerRadius;        // 外部顶点的 Y 坐标

                    var innerX = cos * innerRadius;        // 内部顶点的 X 坐标
                    var innerY = sin * innerRadius;        // 内部顶点的 Y 坐标

                    var currentOuterVertexIndex = lineVertexIndexOffset + j * 2;                          // 当前角外部顶点的索引
                    var currentInnerVertexIndex = lineVertexIndexOffset + j * 2 + 1;                      // 当前角内部顶点的索引
                    var nextOuterVertexIndex = lineVertexIndexOffset + (j + 1) % vertexCount * 2;         // 下一个角外部顶点的索引
                    var nextInnerVertexIndex = lineVertexIndexOffset + (j + 1) % vertexCount * 2 + 1;     // 下一个角内部顶点的索引

                    // 添加两个顶点
                    vh.AddVert(new Vector3(outerX, outerY), color, new Vector4());
                    vh.AddVert(new Vector3(innerX, innerY), color, new Vector4());

                    // 添加两个三角形
                    vh.AddTriangle(currentInnerVertexIndex, nextInnerVertexIndex, nextOuterVertexIndex);
                    vh.AddTriangle(currentInnerVertexIndex, nextOuterVertexIndex, currentOuterVertexIndex);
                }
            }


            for (int i = 0; i < vertexCount; i++)
            {
                var outerRadius = radius - lineThickness / 2;           // 外部半径
                var vertexIndexOffset = vh.currentVertCount;

                var radian = radianGap * i;                             // 当前方向角度
                var cos = Mathf.Cos(radian);                            // 当前角度 COS 值
                var sin = Mathf.Sin(radian);                            // 当前角度 SIN 值
                var tanCos = Mathf.Cos(radian - Mathf.PI / 2);          // 当前方向切线的 COS 值
                var tanSin = Mathf.Sin(radian - Mathf.PI / 2);          // 当前方向切线的 SIN 值
                var sideX = cos * outerRadius;
                var sideY = sin * outerRadius;

                var centerPoint0 = new Vector3(tanCos * halfLineThickness, tanSin * halfLineThickness);
                var centerPoint1 = -centerPoint0;

                var sidePoint0 = new Vector3(sideX + centerPoint0.x, sideY + centerPoint0.y);
                var sidePoint1 = new Vector3(sideX + centerPoint1.x, sideY + centerPoint1.y);

                vh.AddVert(centerPoint0, color, new Vector4());
                vh.AddVert(centerPoint1, color, new Vector4());
                vh.AddVert(sidePoint0, color, new Vector4());
                vh.AddVert(sidePoint1, color, new Vector4());

                vh.AddTriangle(vertexIndexOffset, vertexIndexOffset + 1, vertexIndexOffset + 3);
                vh.AddTriangle(vertexIndexOffset, vertexIndexOffset + 3, vertexIndexOffset + 2);
            }
        }
    }
}
