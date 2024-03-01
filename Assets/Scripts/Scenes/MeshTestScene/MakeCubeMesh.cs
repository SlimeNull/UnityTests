using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTests
{

    [RequireComponent(typeof(MeshFilter))]
    public class MakeCubeMesh : MeshBuilder
    {
        [field: SerializeField]
        public float Radius { get; set; } = 0.5f;

        protected override Mesh CreateMesh()
        {
            float radius = Radius;

            return new Mesh()
            {
                vertices = new Vector3[]
                {
                    new Vector3(-radius, -radius, -radius),
                    new Vector3(radius, -radius, -radius),
                    new Vector3(radius, radius, -radius),
                    new Vector3(-radius, radius, -radius),
                    new Vector3(-radius, -radius, radius),
                    new Vector3(radius, -radius, radius),
                    new Vector3(radius, radius, radius),
                    new Vector3(-radius, radius, radius),
                },

                triangles = new int[]
                {
                    0, 2, 1,
                    0, 3, 2,
                    3, 6, 2,
                    3, 7, 6,
                    1, 6, 5,
                    1, 2, 6,
                    4, 3, 0,
                    4, 7, 3,
                    5, 6, 4,
                    4, 6, 7,
                    4, 1, 5,
                    4, 0, 1
                }
            };
        }
    }
}
