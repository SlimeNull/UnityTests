using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTests
{
    public class FollowObject : MonoBehaviour
    {
        [field: SerializeField]
        public GameObject Target { get; set; }

        [field: SerializeField]
        public Vector3 Offset { get; set; } = new Vector3();

        private void LateUpdate()
        {
            if (Target != null)
                transform.position = Target.transform.position + Offset;
        }
    }
}
