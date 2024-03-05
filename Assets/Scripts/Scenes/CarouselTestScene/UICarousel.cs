using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UnityTests
{
    [RequireComponent(typeof(RectTransform))]
    public class UICarousel : UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        RectTransform _rectTransform;

        // 旋转偏移量 (弧度制)
        float _radianOffset;
        float _radianVelocity;

        bool _dragging;
        CarouselItem _lastSelectedItem;


        [SerializeField]
        Sprite[] _images;

        [SerializeField]
        private bool _scaleImages = true;

        [SerializeField]
        private float _minScale = 0.3f;

        List<UnityEngine.UI.Image> _imageComponents;

        protected RectTransform rectTransform => _rectTransform ??= GetComponent<RectTransform>();

        /// <summary>
        /// 要展示的图片
        /// </summary>
        public Sprite[] Images { get => _images; set => SetImages(value); }

        /// <summary>
        /// 图片尺寸 (用于生成 Image 物体设置大小)
        /// </summary>
        [field: SerializeField]
        public Vector2 ImageSize { get; set; } = new Vector2(100, 100);

        /// <summary>
        /// 旋转阻力
        /// </summary>
        [field: SerializeField]
        public float RadianDrag { get; set; } = 10;

        /// <summary>
        ///是否根据图像的前后关系调整图像大小  <br/>
        /// 如果是 Overlap, 不存在近大远小, 则需要开启这个, 但是如果是 WorldSpace 的 Canvas 并且设置了缩放使其在场景内, 则不需要启用这个
        /// </summary>
        public bool ScaleImages
        {
            get => _scaleImages;
            set
            {
                _scaleImages = value;
                UpdateImagesStatus();
            }
        }

        /// <summary>
        /// 最小缩放比例 (最后方的图像的缩放系数会是这个值)
        /// </summary>
        public float MinScale { get => _minScale; set => _minScale = value; }

        /// <summary>
        /// 允许点击选择
        /// </summary>
        [field: SerializeField]
        public bool AllowClickSelection { get; set; } = false;

        [field: SerializeField]
        public bool EnableInertia { get; set; } = true;

        [field: SerializeField]
        public bool EnableAutoCorrection { get; set; } = true;

        /// <summary>
        /// 矫正时间
        /// </summary>
        [field: SerializeField]
        public float TransitionTime { get; set; } = 0.2f;

        /// <summary>
        /// 已选择的索引
        /// </summary>
        public int SelectedIndex { get; private set; }

        /// <summary>
        /// 已选择的图像
        /// </summary>
        public Sprite SelectedImage { get; private set; }


        protected override void Awake()
        {
            Initialize();
        }

        protected override void Start()
        {
            UpdateImagesStatus();
            Select(0);
        }

        protected virtual void Update()
        {
            // 拖拽惯性
            if (EnableInertia && !_dragging && _radianVelocity != 0)
            {
                _radianOffset += _radianVelocity * Time.deltaTime;
                UpdateImagesStatus();

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
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            if (_images is null)
                _images = Array.Empty<Sprite>();

            UpdateRenderers();
        }

        /// <summary>
        /// 在设置图像时, 同时更新渲染器 (创建或删除 Image 对象)
        /// </summary>
        /// <param name="images"></param>
        private void SetImages(Sprite[] images)
        {
            UpdateRenderers();
        }

        /// <summary>
        /// 根据图像创建 Image 对象, 并自动设置属性
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private UnityEngine.UI.Image CreateRendererFor(Sprite image)
        {
            GameObject gameObject = new("Image");
            gameObject.transform.SetParent(transform);

            var rectTransform = gameObject.AddComponent<RectTransform>();
            var renderer = gameObject.AddComponent<UnityEngine.UI.Image>();

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ImageSize.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ImageSize.y);
            renderer.sprite = image;

            return renderer;
        }

        /// <summary>
        /// 更新 Sprite 的渲染器
        /// </summary>
        private void UpdateRenderers()
        {
            if (_imageComponents is null)
                _imageComponents = new List<UnityEngine.UI.Image>(GetComponentsInChildren<UnityEngine.UI.Image>());

            if (_imageComponents.Count < _images.Length)
            {
                for (int i = 0; i < _imageComponents.Count; i++)
                    _imageComponents[i].sprite = _images[i];
                while (_imageComponents.Count < _images.Length)
                    _imageComponents.Add(CreateRendererFor(_images[_imageComponents.Count]));
            }
            else
            {
                for (int i = 0; i < _images.Length; i++)
                    _imageComponents[i].sprite = _images[i];
                while (_imageComponents.Count > _images.Length)
                {
                    int lastIndex = _imageComponents.Count - 1;

                    // destroy the last
                    Destroy(_imageComponents[lastIndex].gameObject);

                    // remove the last
                    _imageComponents.RemoveAt(lastIndex);
                }
            }
        }

        /// <summary>
        /// 旋转主逻辑
        /// 根据 "旋转偏移量" 设置所有图像的位置, 大小, 以及前后关系
        /// </summary>
        private void UpdateImagesStatus()
        {
            UpdateRenderers();

            var scaleGap = 1 - MinScale;
            var radianGap = Mathf.PI * 2 / _images.Length;
            var selfSizeDelta = rectTransform.sizeDelta;
            var radius = selfSizeDelta.x / 2;
            var imageCount = _images.Length;
            var halfPi = Mathf.PI / 2;

            var minSin = 1f;
            var selectedImageIndex = -1;

            _radianOffset %= (Mathf.PI * 2);
            for (int i = 0; i < imageCount; i++)
            {
                var scaleShrink = scaleGap * i / (imageCount - 1);
                var renderer = _imageComponents[i];

                float cos = Mathf.Cos(radianGap * i + _radianOffset - halfPi);
                float sin = Mathf.Sin(radianGap * i + _radianOffset - halfPi);
                var x = cos * radius;
                var z = sin * radius;

                if (sin <= minSin)
                {
                    selectedImageIndex = i;
                    minSin = sin;
                }

                renderer.transform.localPosition = new Vector3(x, 0, z);

                if (ScaleImages)
                {
                    var scale = Mathf.Lerp(MinScale, 1, ((-sin) + 1) / 2);
                    renderer.transform.localScale = new Vector3(scale, scale, scale);
                }
                else
                {
                    renderer.transform.localScale = new Vector3(1, 1, 1);
                }
            }

            // 根据大小, 调整顺序, 因为小的在后面, 所以直接根据大小, 调用调整顺序方法
            foreach (var com in _imageComponents.OrderBy(com => com.rectTransform.localScale.x))
                com.transform.SetAsLastSibling();

            // 记录当前选择索引以及图像
            SelectedIndex = selectedImageIndex;
            SelectedImage = _images[selectedImageIndex];
        }

        /// <summary>
        /// 矫正目标的旋转量
        /// 用于圆形旋转时, 当需要从 350 旋转到 -10 时, 不让它从 350 过渡到 -10, 而是从 350 过渡到 370
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private float CorrectRadianTarget(float current, float target)
        {
            if (target > current)
            {
                if (target - current > Mathf.PI)
                    target -= Mathf.PI * 2;
            }
            else
            {
                if (current - target > Math.PI)
                    target += Mathf.PI * 2;
            }

            return target;
        }

        /// <summary>
        /// 根据对象索引, 获取对应旋转偏移量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private float GetRadianOffsetFromIndex(int index)
        {
            var radianGap = Mathf.PI * 2 / _images.Length;

            return radianGap * -index;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            UpdateImagesStatus();
        }

        /// <summary>
        /// 旋转目标索引的对象
        /// </summary>
        /// <param name="index"></param>
        public void Select(int index)
        {
            var startValue = _radianOffset;
            var endValue = GetRadianOffsetFromIndex(index);

            endValue = CorrectRadianTarget(startValue, endValue);

            DOTween.To(() => startValue, (newValue) =>
                {
                    _radianOffset = newValue;
                    UpdateImagesStatus();
                }, endValue, .1f)
                .SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    var selectedGameObject = _imageComponents[index].gameObject;

                    _lastSelectedItem?.OnItemDeselected();
                    _lastSelectedItem = null;

                    if (selectedGameObject.GetComponent<CarouselItem>() is CarouselItem carouselItem)
                    {
                        carouselItem.OnItemSelected();
                        _lastSelectedItem = carouselItem;
                    }
                });
        }

        /// <summary>
        /// 拖拽时, 计算实际角度偏移量, 并更新图像状态
        /// </summary>
        /// <param name="eventData"></param>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            var selfSizeDelta = _rectTransform.sizeDelta;
            float normalizedOffset = eventData.delta.x / (selfSizeDelta.x / 2);
            float radianChange = Mathf.Asin(normalizedOffset % 1);

            _radianOffset += radianChange;
            _radianVelocity = radianChange / Time.deltaTime;

            UpdateImagesStatus();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
        }

        /// <summary>
        /// 旋转结束时, 也选择最前方的对象
        /// </summary>
        /// <param name="eventData"></param>
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;

            if (!EnableInertia && EnableAutoCorrection)
            {
                Select(SelectedIndex);
            }
        }

        /// <summary>
        /// 当点击时, 如果点击了某个图像, 则将它旋转到最前方
        /// </summary>
        /// <param name="eventData"></param>
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!AllowClickSelection)
                return;

            var imageCount = _images.Length;

            for (int i = 0; i < imageCount; i++)
            {
                var renderer = _imageComponents[i];

                if (eventData.pointerPressRaycast.gameObject == renderer.gameObject)
                {
                    Select(i);
                    break;
                }
            }
        }
    }
}
