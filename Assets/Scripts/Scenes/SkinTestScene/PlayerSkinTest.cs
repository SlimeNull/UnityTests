using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTests
{
    public class PlayerSkinTest : MonoBehaviour
    {
        [field: SerializeField]
        public bool Combine { get; set; }

        [field: SerializeField]
        public GameObject CombineTarget { get; set; }

        [field: SerializeField]
        public SkinObjectActiveItem[] CombineParts { get; set; }


        public void OnSkinChanged()
        {
            // 如果启用了合并
            if (!Combine)
                return;

            // 获取需要合并的身体部分
            GameObject[] activeCombineParts = CombineParts
                .Where(part => part.TargetGameObject != null)
                .Where(part => part.SelfSelected)
                .Select(part => part.TargetGameObject)
                .ToArray();

            // 隐藏它们
            foreach (var wearCombinePart in activeCombineParts)
                wearCombinePart.SetActive(false);

            // 合并
            SkinnedMeshUtils.Combine(CombineTarget, activeCombineParts);
        }
    }
}
