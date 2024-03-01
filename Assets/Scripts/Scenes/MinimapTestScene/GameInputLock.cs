using UnityEngine;

namespace UnityTests
{
    public class GameInputLock : MonoBehaviour
    {
        bool _released = false;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                _released = true;

            if (_released && Input.GetMouseButtonDown(0))
                _released = false;

            if (_released || Input.GetKey(KeyCode.LeftAlt))
                Cursor.lockState = CursorLockMode.None;
            else if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
