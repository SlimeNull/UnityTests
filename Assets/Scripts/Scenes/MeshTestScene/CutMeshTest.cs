using UnityEngine;

namespace UnityTests
{
    [RequireComponent(typeof(MeshFilter))]
    public class CutMeshTest : MonoBehaviour
    {
        MeshFilter _meshFilter;

        [field: SerializeField]
        public Vector3 StartPoint { get; set; }

        [field: SerializeField]
        public Vector3 NormalVector { get; set; } = Vector3.up;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            if (_meshFilter.mesh is Mesh mesh)
            {
                MeshUtils.Cut(mesh, StartPoint, NormalVector, out var mesh1, out var mesh2);

                _meshFilter.mesh = mesh1;

                var newObject = new GameObject();
                newObject.transform.SetParent(transform);
                newObject.transform.localPosition = new Vector3();

                var newMeshFilter = newObject.AddComponent<MeshFilter>();
                var newMeshRenderer = newObject.AddComponent<MeshRenderer>();

                newMeshFilter.mesh = mesh2;

                if (GetComponent<MeshRenderer>() is { } selfMeshRenderer)
                    newMeshRenderer.material = selfMeshRenderer.material;
            }
        }
    }
}
