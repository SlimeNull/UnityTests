using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NinjaGame
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCtrl : MonoBehaviour
    {
        Vector3 _velocity;
        CharacterController _characterController;
        NavMeshAgent _navMeshAgent;

        [field: SerializeField]
        public float ForwordSpeed { get; set; } = 6;

        [field: SerializeField]
        public float ForwordRunSpeed { get; set; } = 10;

        [field: SerializeField]
        public float BackwordSpeed { get; set; } = 3;

        [field: SerializeField]
        public float SideSpeed { get; set; } = 4;

        [field: SerializeField]
        public float RotateSpeed { get; set; } = 1;

        [field: SerializeField]
        public float JumpSpeed { get; set; } = 5;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            var xAxis = Input.GetAxis("Horizontal");
            var zAxis = Input.GetAxis("Vertical");
            var mouseX = Input.GetAxis("Mouse X");

            if (_navMeshAgent != null)
            {
                if (xAxis != 0 || zAxis != 0)
                {
                    // 当玩家尝试手动控制时, 关闭导航组件

                    _navMeshAgent.ResetPath();
                    _navMeshAgent.isStopped = true;
                }

                if (xAxis == 0 && zAxis == 0 && _velocity.y == 0 && _navMeshAgent.hasPath)
                    return;
            }

            _velocity.x = 0;
            _velocity.z = 0;

            _velocity.x += xAxis * SideSpeed;
            if (zAxis > 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    _velocity.z += zAxis * ForwordRunSpeed;
                else
                    _velocity.z += zAxis * ForwordSpeed;
            }
            else
            {
                _velocity.z += zAxis * BackwordSpeed;
            }

            if (_characterController.isGrounded)
            {
                _velocity.y = 0;

                if (Input.GetKeyDown(KeyCode.Space))
                    _velocity.y += JumpSpeed;
            }
            else
            {
                _velocity.y += Physics.gravity.y * Time.deltaTime;
            }

            _characterController.Move(transform.rotation * _velocity * Time.deltaTime);

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                transform.localEulerAngles += new Vector3(0, mouseX * RotateSpeed, 0);
            }
        }
    }
}
