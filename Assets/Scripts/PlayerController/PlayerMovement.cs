using UnityEngine;
using UnityEngine.InputSystem;

namespace Mitchel.PlayerController
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")] 
        public float forwardSpeed = 12f;
        public float sidestepSpeed = 10f;
        [Range(1.1f, 4f)] public float sprintModifier;
        [SerializeField] private float accelerationRate = 10f;
        [SerializeField] private float decelerationRate = 8f;
        [HideInInspector] public Vector3 velocity;
        private float currentForwardSpeed;
        private float currentSidestepSpeed;

        [Header("Vertical Head Bob")] 
        [SerializeField] private AnimationCurve headBobCurve;
        [SerializeField] private float cameraRollAmount;
        [SerializeField] private float cameraRollSpeed;
        private float stepLength;

        private Vector2 currentInput;
        private CharacterController controller;
    
        // Start is called before the first frame update
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            stepLength = headBobCurve.keys[headBobCurve.length - 1].time;
            currentForwardSpeed = forwardSpeed;
            currentSidestepSpeed = sidestepSpeed;
        }

        // Update is called once per frame
        private void Update()
        {
            // Get forward and side axis inputs
            float x = currentInput.x;
            float z = currentInput.y;

            // Set velocity
            velocity.x = x * currentForwardSpeed;
            velocity.z = z * currentSidestepSpeed;

            // Calculate final velocity with the local transform of the player
            Vector3 move = transform.right * velocity.x + 
                           transform.forward * velocity.z;
            
            controller.Move(move * Time.deltaTime);
        }
        
        #region INPUT DETECTION
        /// <summary>
        /// Get the move input action value from the player action map.
        /// Only to be used by the PlayerInput component.
        /// </summary>
        /// <param name="context">Vector2 value of buttons pressed.</param>
        public void OnMove(InputAction.CallbackContext context)
        {
            currentInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Get the sprinting button state from the player action map and adjust player move speed.
        /// Only to be used by the PlayerInput component.
        /// </summary>
        /// <param name="context">The button press state.</param>
        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                currentForwardSpeed = forwardSpeed * sprintModifier;
                currentSidestepSpeed = sidestepSpeed * sprintModifier;
            }
            
            if (context.canceled)
            {
                currentForwardSpeed = forwardSpeed;
                currentSidestepSpeed = sidestepSpeed;
            }
        }
        #endregion
    }
}
