using UnityEngine;

namespace UnityTests
{
    public class QuadsMeshTest : MeshBuilder
    {
        [field: SerializeField]
        public float Size { get; set; } = 1;

        protected override Mesh CreateMesh()
        {
            var radius = Size / 2;

            Mesh mesh = new Mesh()
            {
                vertices = new Vector3[]
                {
                    new Vector3(-radius, -radius, 0),
                    new Vector3(-radius, radius, 0),
                    new Vector3(radius, radius, 0),
                    new Vector3(radius, -radius, 0),
                }
            };

            var indices = new int[]
            {
                0, 1, 2, 3
            };

            mesh.SetIndices(indices, MeshTopology.Quads, 0);

            return mesh;
        }
    }
}
