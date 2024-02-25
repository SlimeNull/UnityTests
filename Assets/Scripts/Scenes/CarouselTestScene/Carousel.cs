using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NinjaGame
{
    public class Carousel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        float _radianOffset;
        float _cameraDistance;
        Vector3 _dragPosition;

        [field: SerializeField]
        public float Size { get; set; } = 10;

        public int SelectedIndex { get; private set; }
        public GameObject SelectedGameObject { get; private set; }

        protected virtual void Start()
        {
            UpdateObjectStatus();
        }

        private void UpdateObjectStatus()
        {
            int childCount = transform.childCount;

            float radius = Size / 2;
            float radianGap = Mathf.PI * 2 / childCount;

            float selectedItemSin = 1;
            int selectedIndex = -1;
            GameObject selectedGameObject = null;

            _radianOffset %= (Mathf.PI * 2);
            for (int i = 0; i < childCount; i++)
            {
                float radian = radianGap * i + _radianOffset - Mathf.PI / 2;
                var cos = Mathf.Cos(radian);
                var sin = Mathf.Sin(radian);
                var x = cos * radius;
                var z = sin * radius;

                var child = transform.GetChild(i);

                child.transform.localPosition = new Vector3(x, 0, z);

                if (sin < selectedItemSin)
                {
                    selectedIndex = i;
                    selectedGameObject = child.gameObject;
                    selectedItemSin = sin;
                }
            }

            SelectedIndex = selectedIndex;
            SelectedGameObject = selectedGameObject;
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
            var radianGap = Mathf.PI * 2 / transform.childCount;

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
                UpdateObjectStatus();
            }, endValue, .1f)
                .SetEase(Ease.OutCirc);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = _cameraDistance;

            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var mouseWorldOffset = mouseWorldPosition - _dragPosition;

            float normalizedOffset = mouseWorldOffset.x / (Size / 2);
            float angleOffset = Mathf.Asin(normalizedOffset % 1);

            _radianOffset += angleOffset;
            _dragPosition = mouseWorldPosition;
            UpdateObjectStatus();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            var selfScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
            _cameraDistance = selfScreenPoint.z;

            var mousePosition = Input.mousePosition;
            mousePosition.z = _cameraDistance;

            _dragPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            Select(SelectedIndex);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var imageCount = transform.childCount;

            for (int i = 0; i < imageCount; i++)
            {
                var child = transform.GetChild(i);

                if (eventData.pointerCurrentRaycast.gameObject == child.gameObject)
                {
                    Select(i);
                    break;
                }
            }
        }
    }
}
