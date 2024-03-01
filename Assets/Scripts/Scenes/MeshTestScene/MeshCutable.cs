using UnityEngine;

namespace UnityTests
{

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(LineRenderer))]
    public class MeshCutable : MonoBehaviour
    {
        [field: SerializeField]
        public LineRenderer LineRenderer { get; set; }

        float _cameraDistance;
        Vector3 _startWorldPoint;

        private void Awake()
        {
            
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                LineRenderer.SetPosition(1, Input.mousePosition);
            }
            else
            {
                if (LineRenderer.positionCount != 0)
                    LineRenderer.SetPositions(new Vector3[0]);
            }

            if (Input.GetMouseButtonDown(0))
            {
                var selfScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
                var mousePosition = Input.mousePosition;

                LineRenderer.positionCount = 2;
                LineRenderer.SetPosition(0, mousePosition);

                _cameraDistance = selfScreenPosition.z;

                mousePosition.z = _cameraDistance;
                _startWorldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {

            }
        }
    }
}
