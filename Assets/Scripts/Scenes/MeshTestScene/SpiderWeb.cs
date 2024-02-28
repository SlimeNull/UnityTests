using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NinjaGame
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class SpiderWeb : MaskableGraphic
    {
        [field: SerializeField]
        public float Size { get; set; } = 100;

        [field: SerializeField]
        public int VertexCount { get; set; } = 6;

        [field: SerializeField]
        public int LineCount { get; set; } = 5;

        [field: SerializeField]
        public float LineThickness { get; set; } = 1;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var radius = Size / 2;
            var vertexCount = VertexCount;
            var lineCount = LineCount;
            var lineThickness = LineThickness;
            var halfLineThickness = lineThickness / 2;
            var radianGap = Mathf.PI * 2 / vertexCount;

            vh.Clear();

            for (int i = 0; i < lineCount; i++)
            {
                var outerRadius = radius - (radius * i / lineCount);
                var innerRadius = radius - (radius * i / lineCount) - lineThickness;
                var lineVertexIndexOffset = vh.currentVertCount;

                for (int j = 0; j < vertexCount; j++)
                {
                    var radian = radianGap * j;
                    var cos = Mathf.Cos(radian);
                    var sin = Mathf.Sin(radian);

                    var outerX = cos * outerRadius;
                    var outerY = sin * outerRadius;

                    var innerX = cos * innerRadius;
                    var innerY = sin * innerRadius;

                    var currentOuterVertexIndex = lineVertexIndexOffset + j * 2;
                    var currentInnerVertexIndex = lineVertexIndexOffset + j * 2 + 1;
                    var nextOuterVertexIndex = lineVertexIndexOffset + (j + 1) % vertexCount * 2;
                    var nextInnerVertexIndex = lineVertexIndexOffset + (j + 1) % vertexCount * 2 + 1;

                    vh.AddVert(new Vector3(outerX, outerY), color, new Vector4());
                    vh.AddVert(new Vector3(innerX, innerY), color, new Vector4());

                    vh.AddTriangle(currentInnerVertexIndex, nextInnerVertexIndex, nextOuterVertexIndex);
                    vh.AddTriangle(currentInnerVertexIndex, nextOuterVertexIndex, currentOuterVertexIndex);
                }
            }


            for (int i = 0; i < vertexCount; i++)
            {
                var outerRadius = radius - lineThickness / 2;
                var vertexIndexOffset = vh.currentVertCount;

                var radian = radianGap * i;
                var cos = Mathf.Cos(radian);
                var sin = Mathf.Sin(radian);
                var tanCos = Mathf.Cos(radian - Mathf.PI / 2);
                var tanSin = Mathf.Sin(radian - Mathf.PI / 2);
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
