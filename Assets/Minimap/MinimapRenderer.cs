using UnityEngine;
using UnityEngine.UI;

namespace UnityTests
{

    [RequireComponent(typeof(Minimap))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class MinimapRenderer : MaskableGraphic
    {
        Minimap _minimap;

        protected override void Awake()
        {
            _minimap = GetComponent<Minimap>();
        }

        private void Update()
        {
            SetAllDirty();
        }

        public override Texture mainTexture => _minimap?.TextureSource?.Texture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (_minimap == null || _minimap.TextureSource == null)
                return;

            var worldPoint = Vector3.zero;
            if (_minimap.Origin != null)
                worldPoint = _minimap.Origin.transform.position;

            var minimapAreaSize = _minimap.TextureSource.AreaSize;
            var offsetXInTexture = worldPoint.x / minimapAreaSize;
            var offsetYInTexture = worldPoint.z / minimapAreaSize;

            var uvCenter = _minimap.TextureSource.Pivot + new Vector2(offsetXInTexture, offsetYInTexture);
            var uvOffset = 0.5f / _minimap.Scale;

            var vert1uv = new Vector2(uvCenter.x - uvOffset, uvCenter.y - uvOffset);
            var vert2uv = new Vector2(uvCenter.x - uvOffset, uvCenter.y + uvOffset);
            var vert3uv = new Vector2(uvCenter.x + uvOffset, uvCenter.y + uvOffset);
            var vert4uv = new Vector2(uvCenter.x + uvOffset, uvCenter.y - uvOffset);

            var radius = _minimap.Size / 2;

            vh.AddVert(new Vector3(-radius, -radius, 0), color, vert1uv);
            vh.AddVert(new Vector3(-radius, radius, 0), color, vert2uv);
            vh.AddVert(new Vector3(radius, radius, 0), color, vert3uv);
            vh.AddVert(new Vector3(radius, -radius, 0), color, vert4uv);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
        }
    }
}
