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
            MeshFilter.mesh = CreateMesh();
        }

        protected virtual Mesh CreateMesh()
        {
            return new Mesh();
        }
    }
}
