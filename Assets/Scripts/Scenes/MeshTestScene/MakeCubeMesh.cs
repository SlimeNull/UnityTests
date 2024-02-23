using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace NinjaGame
{
    [RequireComponent(typeof(MeshFilter))]
    public class MakeCubeMesh : MonoBehaviour
    {
        [field: SerializeField]
        public float Radius { get; set; } = 0.5f;

        MeshFilter _meshFilter;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        // Start is called before the first frame update
        void Start()
        {
            float radius = Radius;

            _meshFilter.mesh = new Mesh()
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

        // Update is called once per frame
        void Update()
        {

        }
    }
}
