using UnityEngine;

namespace NinjaGame.Minecraft
{
    public class BlockConfig : ScriptableObject
    {
        [field: SerializeField]
        public BlockData[] Blocks { get; set; }

        public struct BlockData
        {
            public BlockData(
                BlockKind kind,
                Texture2D topTexture,
                Texture2D bottomTexture,
                Texture2D frontTexture,
                Texture2D backTexture,
                Texture leftTexture,
                Texture rightTexture,
                Vector3 colliderCenter,
                Vector3 colliderSize)
            {
                Kind = kind;
                TopTexture = topTexture;
                BottomTexture = bottomTexture;
                FrontTexture = frontTexture;
                BackTexture = backTexture;
                LeftTexture = leftTexture;
                RightTexture = rightTexture;
                ColliderCenter = colliderCenter;
                ColliderSize = colliderSize;
            }

            public BlockKind Kind { get; }
            public Texture2D TopTexture { get; }
            public Texture2D BottomTexture { get; }
            public Texture2D FrontTexture { get; }
            public Texture2D BackTexture { get; }
            public Texture LeftTexture { get; }
            public Texture RightTexture { get; }
            public Vector3 ColliderCenter { get; }
            public Vector3 ColliderSize { get; }
        }
    }
}
