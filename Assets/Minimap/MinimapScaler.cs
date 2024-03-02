using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityTests
{
    [RequireComponent(typeof(Minimap))]
    public class MinimapScaler : MonoBehaviour, IScrollHandler
    {
        Minimap _minimap;

        [field: SerializeField]
        public float MinSize = 5f;

        [field: SerializeField]
        public float MaxSize = 30f;

        private void Awake()
        {
            _minimap = GetComponent<Minimap>();
        }

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            // 地图缩放
            var newSize = Mathf.Clamp(_minimap.AreaSize + eventData.scrollDelta.y, MinSize, MaxSize);
            _minimap.AreaSize = newSize;
        }
    }
}
