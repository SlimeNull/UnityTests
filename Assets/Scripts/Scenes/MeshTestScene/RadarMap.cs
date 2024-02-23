using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NinjaGame
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class RadarMap : MaskableGraphic, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        int _dragingVertexIndex = -1;

        [SerializeField]
        float _size = 200;

        [Range(0, 1)]
        [SerializeField]
        float[] _values = new float[]{ 1, 1, 1, 1, 1, 1 };

        public float Size
        {
            get => _size;
            set
            {
                _size = value;
                SetAllDirty();
            }
        }

        public float[] Values
        {
            get => _values;
            set
            {
                _values = value;
                SetAllDirty();
            }
        }

        [field: SerializeField]
        public bool AllowChange { get; set; } = false;

        [field: SerializeField]
        public float HandleSize { get; set; } = 15;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

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
            if (_dragingVertexIndex == -1)
                return;

            var size = Size;
            var radius = size / 2;
            var handleRadius = HandleSize / 2;
            var vertexCount = Values.Length;

            var radianGap = Mathf.PI * 2 / vertexCount;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out var mouseLocalPoint);

            var value = Mathf.Clamp01(Values[_dragingVertexIndex]);
            var cos = Mathf.Cos(_dragingVertexIndex * radianGap);
            var sin = Mathf.Sin(_dragingVertexIndex * radianGap);
            var x = cos * radius * value;
            var y = sin * radius * value;
            var distanceToCenter = Mathf.Clamp(Vector2.Dot(mouseLocalPoint, new Vector2(cos, sin)), 0, radius) / radius;
            

            Values[_dragingVertexIndex] = Mathf.Clamp01(distanceToCenter / radius);
            SetAllDirty();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!AllowChange)
                return;

            print("begin drag");

            var size = Size;
            var radius = size / 2;
            var handleRadius = HandleSize / 2;
            var vertexCount = Values.Length;

            var radianGap = Mathf.PI * 2 / vertexCount;
            var pointerDistance = eventData.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out var mouseLocalPoint);

            for (int i = 0; i < vertexCount; i++)
            {
                var value = Mathf.Clamp01(Values[i]);
                var cos = Mathf.Cos(i * radianGap);
                var sin = Mathf.Sin(i * radianGap);
                var x = cos * radius * value;
                var y = sin * radius * value;

                if (Vector2.Distance(mouseLocalPoint, new Vector2(x, y)) < HandleSize)
                {
                    _dragingVertexIndex = i;
                    return;
                }
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _dragingVertexIndex = -1;
        }
    }

}
