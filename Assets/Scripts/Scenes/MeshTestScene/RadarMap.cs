using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NinjaGame
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class RadarMap : MaskableGraphic, IDragHandler, IEndDragHandler
    {
        [field: SerializeField]
        public float Size { get; set; } = 100;

        [field: Range(0, 1)]
        [field: SerializeField]
        public float[] Values { get; set; } = Array.Empty<float>();

        [field: SerializeField]
        public bool AllowChange { get; set; } = false;

        [field: SerializeField]
        public float HandleSize { get; set; }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var size = Size;
            var radius = size / 2;
            var vertexCount = Values.Length;

            var radianGap = Mathf.PI * 2 / vertexCount;

            vh.AddVert(new Vector3(0, 0, 0), color, new Vector4());

            for (int i = 0; i < vertexCount; i++)
            {
                var value = Mathf.Clamp01(Values[i]);
                var cos = Mathf.Cos(i * radianGap);
                var sin = Mathf.Sin(i * radianGap);
                var x = cos * radius * value;
                var y = sin * radius * value;

                vh.AddVert(new Vector3(x, y, 0), color, new Vector4());

                var currentVertexIndex = 1 + i;
                var nextVertexIndex = 1 + (i + 1) % vertexCount;

                vh.AddTriangle(0, nextVertexIndex, currentVertexIndex);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            var size = Size;
            var radius = size / 2;
            var handleRadius = HandleSize / 2;
            var vertexCount = Values.Length;

            var radianGap = Mathf.PI * 2 / vertexCount;
            var pointerDistance = eventData.position;

            for (int i = 0; i < vertexCount; i++)
            {
                var value = Mathf.Clamp01(Values[i]);
                var cos = Mathf.Cos(i * radianGap);
                var sin = Mathf.Sin(i * radianGap);
                var x = cos * radius * value;
                var y = sin * radius * value;

                //if (eventData.)
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            
        }
    }

}
