using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTests
{
    public class MinimapTools : EditorWindow
    {
        [MenuItem("Tools/Minimap")]
        static void Open()
        {
            var window = EditorWindow.GetWindow<MinimapTools>();
            window.Show();
        }

        string _textureSizeStr = "1024";
        string _cameraHeightStr = "5";
        string _minimapAreaSizeStr = "50";

        private void OnGUI()
        {
            GUILayout.Label("Texture size: ");
            _textureSizeStr = GUILayout.TextField(_textureSizeStr);

            GUILayout.Label("Camera height: ");
            _cameraHeightStr = GUILayout.TextField(_cameraHeightStr);

            GUILayout.Label("Minimap area size: ");
            _minimapAreaSizeStr = GUILayout.TextField(_minimapAreaSizeStr);

            GUILayout.Space(10);

            if (!int.TryParse(_textureSizeStr, out var textureSize) ||
                !float.TryParse(_cameraHeightStr, out var cameraHeight) ||
                !float.TryParse(_minimapAreaSizeStr, out var minimapAreaSize))
            {
                GUILayout.Label("Error input!");
                return;
            }

            if (GUILayout.Button("Generate minimap texture"))
            {
                string filename = EditorUtility.SaveFilePanel("Save texture", Application.dataPath, "Minimap", "png");

                RenderTexture rt = new RenderTexture(textureSize, textureSize, 32);

                GameObject cameraGameObject = new GameObject();
                Camera camera = cameraGameObject.AddComponent<Camera>();

                cameraGameObject.transform.eulerAngles = new Vector3(90, 0, 0);
                cameraGameObject.transform.position = new Vector3(0, cameraHeight, 0);

                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0, 0, 0, 0);
                camera.orthographic = true;
                camera.orthographicSize = minimapAreaSize / 2;

                camera.targetTexture = rt;
                camera.Render();

                var activeRenderTextureBefore = RenderTexture.active;
                RenderTexture.active = rt;

                Texture2D png = new Texture2D(textureSize, textureSize);
                png.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
                png.Apply();

                RenderTexture.active = activeRenderTextureBefore;

                var bytes = png.EncodeToPNG();
                System.IO.File.WriteAllBytes(filename, bytes);

                DestroyImmediate(cameraGameObject);
                DestroyImmediate(rt);
                DestroyImmediate(png);
            }
        }
    }
}
