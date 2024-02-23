using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NinjaGame
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class TestRadar : MaskableGraphic, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Range(0, 1)]
        public List<float> value = new List<float>();

        Vector3[] maxPos, pos;
        Vector3 delPos;
        int choseIndex = -1;
        float r;

        public void OnBeginDrag(PointerEventData eventData)
        {

            delPos = GetLocalPos(transform);
            Vector3 v3 = Input.mousePosition - delPos - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            for (int i = 0; i < pos.Length; i++)
            {
                if (Vector3.Distance(pos[i], v3) <= 50)
                {
                    choseIndex = i;
                    return;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (choseIndex != -1)
            {
                Vector3 v3 = Input.mousePosition - delPos - new Vector3(Screen.width / 2, Screen.height / 2, 0);
                value[choseIndex] = Vector3.Dot(maxPos[choseIndex], v3) / Vector3.Dot(maxPos[choseIndex], maxPos[choseIndex]);
                if (value[choseIndex] > 1)
                    value[choseIndex] = 1;
                if (value[choseIndex] < 0)
                    value[choseIndex] = 0;
                SetAllDirty();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            choseIndex = -1;
        }

        public Vector3 GetLocalPos(Transform tr)
        {
            if (tr.GetComponent<Canvas>() != null)
            {
                return Vector3.zero;
            }
            else
            { return tr.localPosition + GetLocalPos(tr.parent); }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();
            maxPos = new Vector3[value.Count];
            pos = new Vector3[value.Count];
            float rad = 2 * Mathf.PI / value.Count;
            RectTransform rec = GetComponent<RectTransform>();
            r = Mathf.Min(rec.sizeDelta.x, rec.sizeDelta.y) / 2;
            for (int i = 0; i < value.Count; i++)
            {
                float x = Mathf.Cos(i * rad + Mathf.PI / 2) * r;
                float y = Mathf.Sin(i * rad + Mathf.PI / 2) * r;
                maxPos[i] = new Vector3(x, y);
                pos[i] = maxPos[i] * value[i];
                vh.AddVert(pos[i], color, Vector2.zero);
            }
            vh.AddVert(Vector3.zero, color, Vector2.zero);

            for (int i = 0; i < value.Count; i++)
            {
                vh.AddTriangle(value.Count, (i + 1) % value.Count, i);
            }
        }
    }

}
