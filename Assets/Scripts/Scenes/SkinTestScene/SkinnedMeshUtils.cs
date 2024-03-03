using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTests
{
    public static class SkinnedMeshUtils
    {
        public static void Combine(GameObject target, GameObject[] gameObjects)
        {
            // 材质
            Material material = null;

            // 合并实例
            List<CombineInstance> combineInstances = new();

            // 需要合并的纹理
            List<Texture2D> textures = new();

            // 所有网格的 UV
            List<Vector2[]> meshUvs = new();

            // 获取已经存在的骨骼
            var existBones = target
                .GetComponentsInChildren<Transform>()
                .ToDictionary(tr => tr.name, tr => tr);

            // 结果骨骼
            var resultBones = new List<Transform>();

            int textureMaxWidth = 0;
            int textureMaxHeight = 0;

            // 遍历所有游戏对象
            foreach (var gameObject in gameObjects)
            {
                // 如果没有 SkinnedMeshRenderer, 或者网格为空则跳过
                if (gameObject.GetComponent<SkinnedMeshRenderer>() is not SkinnedMeshRenderer smr ||
                    smr.sharedMesh == null)
                    continue;

                // 复制材质
                if (material == null &&
                    smr.sharedMaterial != null)
                    material = UnityEngine.Object.Instantiate(smr.sharedMaterial);

                // 添加合并任务实例
                for (int i = 0; i < smr.sharedMesh.subMeshCount; i++)
                {
                    combineInstances.Add(
                        new CombineInstance()
                        {
                            mesh = smr.sharedMesh,
                            subMeshIndex = i
                        });
                }

                // 保存 UV
                meshUvs.Add(smr.sharedMesh.uv);

                // 提取主纹理
                var texture = smr.material?.mainTexture as Texture2D;

                // 保存入要合并的纹理列表中
                textures.Add(texture);
                if (texture != null)
                {
                    // 保存纹理的最大宽度和高度
                    textureMaxWidth = Math.Max(textureMaxWidth, texture.width);
                    textureMaxHeight = Math.Max(textureMaxHeight, texture.height);
                }

                foreach (var bone in smr.bones)
                {
                    //if (resultBoneNames.Contains(bone.name))
                    //    continue;
                    if (!existBones.ContainsKey(bone.name))
                        continue;

                    resultBones.Add(bone);
                }
            }

            // 计算合并纹理的结果大小
            int combinedTextureColumn = (int)Math.Ceiling(Mathf.Log(textures.Count, 2));
            int combinedTextureRow = (int)Math.Ceiling((float)textures.Count / combinedTextureColumn);
            int combinedTextureWidth = textureMaxWidth * combinedTextureColumn;
            int combinedTextureHeight = textureMaxHeight * combinedTextureRow;

            // 将大小限制在 1024 (也可以不限制)
            float scale = (float)1024 / Mathf.Max(combinedTextureWidth, combinedTextureHeight);
            if (scale < 1)
            {
                combinedTextureWidth = (int)(combinedTextureWidth * scale);
                combinedTextureHeight = (int)(combinedTextureHeight * scale);
            }

            int combinedTextureSize = Mathf.Max(combinedTextureWidth, combinedTextureHeight);

            Texture2D combinedTexture = new Texture2D(combinedTextureSize, combinedTextureSize);
            var textureCombineResult = combinedTexture.PackTextures(textures.ToArray(), 0);

            Mesh combinedMesh = new();
            combinedMesh.CombineMeshes(combineInstances.ToArray(), true, false);

            // 修正 UV 坐标
            Vector2[] resultUv = new Vector2[combinedMesh.uv.Length];
            for (int i = 0, j = 0; i < meshUvs.Count; i++)
            {
                foreach (var uv in meshUvs[i])
                {
                    resultUv[j].x = Mathf.Lerp(textureCombineResult[i].xMin, textureCombineResult[i].xMax, uv.x);
                    resultUv[j].y = Mathf.Lerp(textureCombineResult[i].yMin, textureCombineResult[i].yMax, uv.y);
                    j++;
                }
            }

            combinedMesh.uv = resultUv;

            // 更新合并目标的 SkinnedMeshRenderer
            SkinnedMeshRenderer targetSmr = target.GetComponent<SkinnedMeshRenderer>();
            if (targetSmr == null)
                targetSmr = target.AddComponent<SkinnedMeshRenderer>();

            material.mainTexture = combinedTexture;

            targetSmr.sharedMesh = combinedMesh;
            targetSmr.sharedMaterial = material;
            targetSmr.bones = resultBones.ToArray();
        }
    }
}
