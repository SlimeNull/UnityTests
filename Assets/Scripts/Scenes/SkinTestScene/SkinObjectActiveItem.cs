using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTests
{
    public class SkinObjectActiveItem : CarouselItem
    {
        [field: SerializeField]
        public PlayerSkinTest SkinTest { get; set; }

        [field: SerializeField]
        public GameObject TargetGameObject { get; set; }

        public bool SelfSelected { get; private set; }

        public override void OnItemDeselected()
        {
            SelfSelected = false;
            base.OnItemDeselected();
            if (TargetGameObject != null)
                TargetGameObject.SetActive(false);
        }

        public override void OnItemSelected()
        {
            SelfSelected = true;
            base.OnItemSelected();

            // 当被选择时, 激活目标物体
            if (TargetGameObject != null)
                TargetGameObject.SetActive(true);

            // 触发皮肤测试脚本的方法
            if (SkinTest != null)
                SkinTest.OnSkinChanged();
        }
    }
}
