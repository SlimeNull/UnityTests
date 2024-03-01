using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityTests
{
    /// <summary>
    /// 雷达图
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class RadarMap2 : MaskableGraphic, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        int _dragingVertexIndex = -1;

        [SerializeField]
        float _size = 200;

        [Range(0, 1)]
        [SerializeField]
        float[] _values = new float[]{ 1, 1, 1, 1, 1, 1 };

        List<GameObject> _handles;

        /// <summary>
        /// 雷达图大小
        /// </summary>
        public float Size
        {
            get => _size;
            set
            {
                _size = value;
                SetAllDirty();
            }
        }

        /// <summary>
        /// 雷达图值
        /// </summary>
        public float[] Values
        {
            get => _values;
            set
            {
                _values = value;
                SetAllDirty();
            }
        }

        /// <summary>
        /// 允许用户拖拽变更值
        /// </summary>
        [field: SerializeField]
        public bool AllowChange { get; set; } = false;


        [field: SerializeField]
        public GameObject HandlePrefab { get; set; }

        /// <summary>
        /// 拖拽手柄大小
        /// </summary>
        [field: SerializeField]
        public float HandleSize { get; set; } = 15;

        private void UpdateHandles()
        {
            while (_handles.Count < _values.Length)
            {
                var handle = GameObject.Instantiate(HandlePrefab, transform);

                handle.SetActive(true);
                _handles.Add(handle);
            }

            while (_handles.Count > _values.Length)
            {
                var handle = _handles[_values.Length];

                Destroy(handle);
                _handles.RemoveAt(_values.Length);
            }
        }

        protected override void Awake()
        {
            _handles = new();
        }

        protected override void Start() 
        {
            // 因为 Graphics 的 Start 在编辑器中也可能执行, 从而导致手柄重复创建
            // 所以在这里判断是否在 Playing

            if (Application.isPlaying)
                UpdateHandles();
        }

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

                if (i < _handles.Count)
                    _handles[i].transform.localPosition = new Vector3(x, y, 0);

                vh.AddVert(new Vector3(x, y, 0), color, new Vector4());

                var currentVertexIndex = 1 + i;
                var nextVertexIndex = 1 + (i + 1) % vertexCount;

                vh.AddTriangle(0, nextVertexIndex, currentVertexIndex);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            // 仅在有索引的时候进行拖拽
            if (_dragingVertexIndex == -1)
                return;

             var size = Size;
            var radius = size / 2;
            var handleRadius = HandleSize / 2;
            var vertexCount = Values.Length;

            var radianGap = Mathf.PI * 2 / vertexCount;

            // 使用内置的工具类方法将鼠标坐标从屏幕坐标转到 RectTransform 的局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out var mouseLocalPoint);

            var value = Mathf.Clamp01(Values[_dragingVertexIndex]);
            var cos = Mathf.Cos(_dragingVertexIndex * radianGap);
            var sin = Mathf.Sin(_dragingVertexIndex * radianGap);
            var x = cos * radius * value;
            var y = sin * radius * value;

            // 点积取鼠标向量在当前方向上的投影, 并使用 Clamp 限制取值
            var distanceToCenter = Mathf.Clamp(Vector2.Dot(mouseLocalPoint, new Vector2(cos, sin)), 0, radius);

            // 最终取值也限制下, 因为只能是 0~1
            float newValue = Mathf.Clamp01(distanceToCenter / radius);

            var newX = cos * radius * newValue;
            var newY = sin * radius * newValue;

            Values[_dragingVertexIndex] = newValue;
            SetAllDirty();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!AllowChange)
                return;

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

                // 如果鼠标位置与雷达图一角端点的距离小于手柄大小
                if (Vector2.Distance(mouseLocalPoint, new Vector2(x, y)) < HandleSize)
                {
                    // 记录当前顶点索引
                    _dragingVertexIndex = i;
                    return;
                }
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            // 停止拖拽, 重置拖拽顶点索引
            _dragingVertexIndex = -1;
        }
    }
}
