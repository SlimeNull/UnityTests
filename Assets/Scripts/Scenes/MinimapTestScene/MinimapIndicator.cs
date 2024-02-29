using UnityEngine;

namespace NinjaGame
{
    public class MinimapIndicator : MonoBehaviour
    {
        [field: SerializeField]
        public Camera MinimapCamera { get; set; }

        [field: SerializeField]
        public GameObject Player { get; set; }

        [field: SerializeField]
        public GameObject PlayerImage { get; set; }

        [field: SerializeField]
        public float Size { get; set; } = 100;

        private void Update()
        {
            if (Player != null && PlayerImage != null)
            {
                var radius = Size / 2;
                var viewportPosition = MinimapCamera.WorldToViewportPoint(Player.transform.position);
                var localPosition = new Vector2(viewportPosition.x * 2 - 1, viewportPosition.y * 2 - 1) * radius;

                PlayerImage.transform.localPosition = localPosition;
            }
        }
    }
}
