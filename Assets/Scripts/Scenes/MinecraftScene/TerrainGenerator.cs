using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTests.Minecraft
{
    public class TerrainGenerator : MonoBehaviour
    {
        [field: SerializeField]
        public int Seed { get; set; }

        [field: SerializeField]
        public int ChunkSize { get; set; } = 16;

        [field: SerializeField]
        public int SeaLeavel { get; set; } = 102;

        [field: SerializeField]
        public int Octaves { get; set; } = 3;

        [field: SerializeField]
        public float TerrainUndulationFactor { get; set; } = 20;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private float Perlin2D(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
        }

        private float Perlin3D(float x, float y, float z)
        {
            return Mathf.PerlinNoise(x, Mathf.PerlinNoise(y, z));
        }

        public BlockKind GetBlockKind(int x, int y, int z)
        {
            return BlockKind.Air;
        }


        public GameObject CreateBlock(
            Texture2D topTexture, 
            Texture2D bottomTexture, 
            Texture2D frontTexture, 
            Texture2D backTexture, 
            Texture leftTexture, 
            Texture rightTexture,
            Vector3 colliderCenter,
            Vector3 colliderSize)
        {
            GameObject newBlock = new();

            var collider = newBlock.AddComponent<BoxCollider>();
            collider.center = colliderCenter;
            collider.size = colliderSize;

            return newBlock;
        }
    }

    public class TerrianChunk : MonoBehaviour
    {

    }
}
