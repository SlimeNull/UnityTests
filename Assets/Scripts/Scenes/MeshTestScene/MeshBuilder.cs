using UnityEngine;

namespace NinjaGame
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshBuilder : MonoBehaviour
    {
        protected MeshFilter meshFilter;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            meshFilter.mesh = CreateMesh();
        }

        protected virtual Mesh CreateMesh()
        {
            return new Mesh();
        }
    }
}
