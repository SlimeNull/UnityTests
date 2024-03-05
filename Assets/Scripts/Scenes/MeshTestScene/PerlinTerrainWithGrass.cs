using System.Collections.Generic;
using UnityEngine;

namespace UnityTests
{
    public class PerlinTerrainWithGrass : PerlinTerrain
    {
        /// <summary>
        /// 草的网格
        /// </summary>
        [field: SerializeField]
        public Mesh GrassMesh { get; set; }

        /// <summary>
        /// 草旋转
        /// </summary>
        [field: SerializeField]
        public Vector3 GrassRotation { get; set; }

        /// <summary>
        /// 草缩放
        /// </summary>
        [field: SerializeField]
        public Vector3 GrassScale { get; set; } = new Vector3(1, 1, 1);

        /// <summary>
        /// 草材质
        /// </summary>
        [field: SerializeField]
        public Material GrassMaterial { get; set; }

        /// <summary>
        /// 批处理大小
        /// </summary>
        [field: SerializeField]
        public int BatchSize { get; set; } = 1023;

        List<Matrix4x4> _grassMatrices;
        List<Vector4> _grassColors;
        MaterialPropertyBlock _materialPropertyBlock;
        Matrix4x4[] _grassMatricesBuffer;
        Vector4[] _grassColorBuffer;

        protected override void Awake()
        {
            _grassMatrices = new();
            _grassColors = new();
            _grassMatricesBuffer = new Matrix4x4[BatchSize];
            _grassColorBuffer = new Vector4[BatchSize];

            base.Awake();

            _materialPropertyBlock = new();
        }

        protected override void OnPopulateVertex(Vector3 vertex)
        {
            base.OnPopulateVertex(vertex);

            _grassMatrices.Add(Matrix4x4.TRS(transform.position + vertex, Quaternion.Euler(GrassRotation), GrassScale));
            _grassColors.Add(Random.ColorHSV(0, Random.Range(0, 1f)));
        }

        private void Update()
        {
            if (GrassMesh != null && GrassMaterial != null)
            {
                int i = 0;
                while (i < _grassMatrices.Count)
                {
                    int copyCount = Mathf.Min(_grassMatricesBuffer.Length, _grassMatrices.Count - i);
                    _grassMatrices.CopyTo(i, _grassMatricesBuffer, 0, copyCount);
                    _grassColors.CopyTo(i, _grassColorBuffer, 0, copyCount);

                    _materialPropertyBlock.SetVectorArray("_Color", _grassColorBuffer);
                    Graphics.DrawMeshInstanced(GrassMesh, 0, GrassMaterial, _grassMatricesBuffer, copyCount, _materialPropertyBlock);

                    i += copyCount;
                }
            }
        }
    }

}
