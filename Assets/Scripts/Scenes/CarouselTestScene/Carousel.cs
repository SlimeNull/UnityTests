using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityTests
{
    public class Carousel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        float _radianOffset;                     // 弧度偏移量
        float _radianVelocity;                   // 旋转速度

        float _cameraDistance;
        Vector3 _lastMouseWorldPosition;

        bool _dragging;

        CarouselItem _lastSelectedItem;

        /// <summary>
        /// 大小
        /// </summary>
        [field: SerializeField]
        public float Size { get; set; } = 5;

        /// <summary>
        /// 旋转阻力
        /// </summary>
        [field: SerializeField]
        public float RadianDrag { get; set; } = 10;

        /// <summary>
        /// 允许点击选择
        /// </summary>
        [field: SerializeField]
        public bool AllowClickSelection { get; set; } = false;

        /// <summary>
        /// 启用惯性
        /// </summary>
        [field: SerializeField]
        public bool EnableInertia { get; set; } = true;

        /// <summary>
        /// 启用自动回正
        /// </summary>
        [field: SerializeField]
        public bool EnableAutoCorrection { get; set; } = true;

        /// <summary>
        /// 矫正时长
        /// </summary>
        [field: SerializeField]
        public float AutoCorrectionDuration { get; set; } = 0.2f;

        /// <summary>
        /// 已选择的索引
        /// </summary>
        public int SelectedIndex { get; private set; }

        /// <summary>
        /// 已选择的图像
        /// </summary>
        public GameObject SelectedGameObject { get; private set; }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            UpdateObjectStatus();
            Select(0);
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        // Update is called once per frame
        protected virtual void Update()
        {
            // 拖拽惯性
            if (EnableInertia && !_dragging && _radianVelocity != 0)
            {
                _radianOffset += _radianVelocity * Time.deltaTime;
                UpdateObjectStatus();

                var radianVelocitySign = Mathf.Sign(_radianVelocity);
                var radianVelocitySize = Mathf.Abs(_radianVelocity);

                radianVelocitySize -= RadianDrag * Time.deltaTime;
                if (radianVelocitySize < 0)
                    radianVelocitySize = 0;

                _radianVelocity = radianVelocitySign * radianVelocitySize;

                if (EnableAutoCorrection && _radianVelocity == 0)
                {
                    Select(SelectedIndex);
                }
            }
        }

        /// <summary>
        /// 更新物体状态
        /// </summary>
        protected void UpdateObjectStatus()
        {
            var radius = Size / 2;
            var childCount = transform.childCount;
            var radianGap = Mathf.PI * 2 / childCount;

            var minSin = 1.0f;
            var selectedIndex = -1;
            GameObject selectedGameObject = null;

            _radianOffset %= Mathf.PI * 2;
            for (int i = 0; i < childCount; i++)
            {
                var radian = radianGap * i + _radianOffset - Mathf.PI / 2;
                var child = transform.GetChild(i);

                var cos = Mathf.Cos(radian);
                var sin = Mathf.Sin(radian);

                var x = cos * radius;
                var z = sin * radius;

                child.localPosition = new Vector3(x, 0, z);

                if (sin < minSin)
                {
                    selectedIndex = i;
                    selectedGameObject = child.gameObject;
                    minSin = sin;
                }
            }

            SelectedIndex = selectedIndex;
            SelectedGameObject = selectedGameObject;
        }

        /// <summary>
        /// 矫正目标旋转量
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        private void CorrectRotationTarget(float origin, ref float target)
        {
            const float doublePi = Mathf.PI * 2;

            float diff = (target - origin) % doublePi;
            if (diff > Mathf.PI)
                diff -= doublePi;
            else if (diff < -Mathf.PI)
                diff += doublePi;

            target = origin + diff;
        }

        /// <summary>
        /// 从索引获取旋转量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private float GetRadianFromItemIndex(int index)
        {
            var childCount = transform.childCount;
            var radianGap = Mathf.PI * 2 / childCount;

            return -radianGap * index;
        }

        public void Select(int index)
        {
            var originRadian = _radianOffset;
            var targetRadian = GetRadianFromItemIndex(index);
            CorrectRotationTarget(originRadian, ref targetRadian);

            DOTween
                .To(radian =>
                {
                    _radianOffset = radian;
                    UpdateObjectStatus();
                }, originRadian, targetRadian, AutoCorrectionDuration)
                .OnComplete(() =>
                {
                    _lastSelectedItem?.OnItemDeselected();
                    _lastSelectedItem = null;

                    if (SelectedGameObject.GetComponent<CarouselItem>() is CarouselItem carouselItem)
                    {
                        carouselItem.OnItemSelected();
                        _lastSelectedItem = carouselItem;
                    }
                });
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            var radius = Size / 2;

            var mousePosition = Input.mousePosition;
            mousePosition.z = _cameraDistance;
            var newMouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var mouseWorldOffset = newMouseWorldPosition - _lastMouseWorldPosition;

            _lastMouseWorldPosition = newMouseWorldPosition;

            var radianChange = mouseWorldOffset.x / radius;

            _radianVelocity = radianChange / Time.deltaTime;
            _radianOffset += radianChange;

            UpdateObjectStatus();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            var selfScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            _cameraDistance = selfScreenPosition.z;

            var mousePosition = Input.mousePosition;
            mousePosition.z = _cameraDistance;
            _lastMouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            _dragging = true;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;

            if (EnableAutoCorrection && !EnableInertia)
            {
                Select(SelectedIndex);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!AllowClickSelection)
                return;

            var childCount = transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var renderer = transform.GetChild(i);

                if (eventData.pointerCurrentRaycast.gameObject == renderer.gameObject)
                {
                    Select(i);
                    break;
                }
            }
        }
    }
}
