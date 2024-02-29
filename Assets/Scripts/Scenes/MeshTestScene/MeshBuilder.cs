using UnityEngine;

namespace NinjaGame
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshBuilder : MonoBehaviour
    {
        private MeshFilter _meshFilter;

        public MeshFilter MeshFilter => _meshFilter ??= GetComponent<MeshFilter>();

        protected virtual void Awake()
        {
            var mesh = CreateMesh();

            MeshFilter.mesh = mesh;
            if (GetComponent<MeshCollider>() is { } collider)
                collider.sharedMesh = mesh;
        }

        protected virtual Mesh CreateMesh()
        {
            return new Mesh();
        }
    }
}
