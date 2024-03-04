using System.Collections.Generic;
using UnityEngine;

namespace UnityTests
{
    public class PerlinTerrainWithGrass : PerlinTerrain
    {
        [field: SerializeField]
        public Mesh GrassMesh { get; set; }

        [field: SerializeField]
        public Vector3 GrassRotation { get; set; }

        [field: SerializeField]
        public Vector3 GrassScale { get; set; }

        [field: SerializeField]
        public Material GrassMaterial { get; set; }

        List<Matrix4x4> _grassMatrices;

        protected override void Awake()
        {
            _grassMatrices = new();

            base.Awake();
        }

        protected override void OnPopulateVertex(Vector3 vertex)
        {
            base.OnPopulateVertex(vertex);

            _grassMatrices.Add(Matrix4x4.TRS(transform.position + vertex, Quaternion.Euler(GrassRotation), GrassScale));
        }

        private void Update()
        {
            if (GrassMesh != null && GrassMaterial != null)
                Graphics.DrawMeshInstanced(GrassMesh, 0, GrassMaterial, _grassMatrices);
        }
    }

}
