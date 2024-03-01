using UnityTests.Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace UnityTests
{
    /// <summary>
    /// 小地图导航器
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class MinimapNavigator : MonoBehaviour, IPointerClickHandler
    {
        RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();

        [field: SerializeField]
        public Camera MinimapCamera { get; set; }

        [field: SerializeField]
        public NavMeshAgent NavMeshAgent { get; set; }

        [field: SerializeField]
        public MinimapNavigationIndicator NavigationIndicator { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, GetComponentInParent<Canvas>()?.worldCamera, out var pointerLocalPoint);
            var selfSize = RectTransformUtilityEx.GetSize(RectTransform);

            var viewportPoint = new Vector2(
                pointerLocalPoint.x / selfSize.x + RectTransform.pivot.x,
                pointerLocalPoint.y / selfSize.y + RectTransform.pivot.y);

            var ray = MinimapCamera.ViewportPointToRay(viewportPoint);

            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, MinimapCamera.cullingMask))
            {
                // 启用 NavMeshAgent 并设置目标点
                NavMeshAgent.enabled = true;
                NavMeshAgent.destination = hit.point;

                if (NavigationIndicator is { } indicator)
                    indicator.SetAllDirty();
            }
        }
    }
}
