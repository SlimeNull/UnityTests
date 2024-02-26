using UnityEngine;
using UnityEngine.UI;

namespace NinjaGame
{
    [RequireComponent(typeof(Button))]
    public class CarouselSelectButton : MonoBehaviour
    {
        Button _button;

        [field: SerializeField]
        public Carousel TargetCarousel { get; set; }

        [field: SerializeField]
        public int TargetItemIndex { get; set; }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (TargetCarousel is Carousel carousel)
                carousel.Select(TargetItemIndex);
        }
    }
}