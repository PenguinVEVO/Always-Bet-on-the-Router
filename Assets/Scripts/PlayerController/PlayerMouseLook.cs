using UnityEngine;

namespace Mitchel.PlayerController
{
    public class PlayerMouseLook : MonoBehaviour
    {
        [Header("Mouse Look Settings")] 
        public float mouseSensitivityX = 100f;
        public float mouseSensitivityY = 100f;
        [SerializeField] private Vector2 minMaxPitch = new(-90f, 90f);
        private float _rotationX = 0f;
        
        [Header("Object References")]
        public Camera playerCamera;
        public Transform playerBody;
        
        // Start is called before the first frame update
        void Start()
        {
            // Lock the cursor at the start of the game
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            // Get mouse input and calculate with mouse sensitivity
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, minMaxPitch.x, minMaxPitch.y);

            // Rotate both the camera and the player model together (model is restricted to X axis only, obviously)
            playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}