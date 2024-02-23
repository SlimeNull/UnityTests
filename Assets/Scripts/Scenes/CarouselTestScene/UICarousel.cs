using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NinjaGame
{
    [RequireComponent(typeof(RectTransform))]
    public class UICarousel : UIBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        RectTransform _rectTransform;

        float _radianOffset;

        [SerializeField]
        Sprite[] _images;
        List<UnityEngine.UI.Image> _imageComponents;

        public Sprite[] Images { get => _images; set => SetImages(value); }

        [field: SerializeField]
        public Vector3 ImageSize { get; set; } = new Vector3(100, 100);

        [field: SerializeField]
        public bool ScaleImages { get; set; } = true;

        [field: SerializeField]
        public float MinScale { get; set; } = 0.3f;

        public int SelectedIndex { get; private set; }
        public Sprite SelectedImage { get; private set; }


        protected override void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            Initialize();
        }

        protected override void Start()
        {
            UpdateImagesStatus();
        }

        private void Initialize()
        {
            if (_images is null)
                _images = Array.Empty<Sprite>();

            UpdateRenderers();
        }

        private void SetImages(Sprite[] images)
        {
            UpdateRenderers();
        }

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

        private void UpdateRenderers()
        {
            if (_imageComponents is null)
            {
                _imageComponents = _images
                    .Select(image => CreateRendererFor(image))
                    .ToList();
            }
            else
            {
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
        }

        private void UpdateImagesStatus()
        {
            var scaleGap = 1 - MinScale;
            var radianGap = Mathf.PI * 2 / _images.Length;
            var selfSizeDelta = _rectTransform.sizeDelta;
            var imageCount = _images.Length;
            var halfPi = Mathf.PI / 2;

            var selectedImageIndex = -1;
            var selectedImageScale = 0.0f;

            _radianOffset %= (Mathf.PI * 2);
            for (int i = 0; i < imageCount; i++)
            {
                var scaleShrink = scaleGap * i / (imageCount - 1);
                var renderer = _imageComponents[i];

                float cos = Mathf.Cos(radianGap * i + _radianOffset - halfPi);
                float sin = Mathf.Sin(radianGap * i + _radianOffset - halfPi);
                var x = cos * selfSizeDelta.x;
                var z = sin * selfSizeDelta.x;
                var scale = Mathf.Lerp(MinScale, 1, ((-sin) + 1) / 2);

                if (scale > selectedImageScale)
                {
                    selectedImageIndex = i;
                    selectedImageScale = scale;
                }

                renderer.transform.localPosition = new Vector3(x, 0, z);
                renderer.transform.localScale = new Vector3(scale, scale, scale);
            }

            foreach (var com in _imageComponents.OrderBy(com => com.rectTransform.localScale.x))
                com.transform.SetAsLastSibling();

            SelectedIndex = selectedImageIndex;
            SelectedImage = _images[selectedImageIndex];
        }

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

        private float GetRadianOffsetFromIndex(int index)
        {
            var radianGap = Mathf.PI * 2 / _images.Length;

            return radianGap * -index;
        }

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
                .SetEase(Ease.OutCirc);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _radianOffset += eventData.delta.x / 180;
            UpdateImagesStatus();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var imageCount = _images.Length;

            for (int i = 0; i < imageCount; i++)
            {
                var renderer = _imageComponents[i];

                if (eventData.pointerCurrentRaycast.gameObject == renderer.gameObject)
                {
                    Select(i);
                    break;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Select(SelectedIndex);
        }
    }
}
