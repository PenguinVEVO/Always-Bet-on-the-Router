using UnityEngine;
using UnityEngine.InputSystem;

namespace Mitchel.Player
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
        private Vector3 currentMoveVelocity;
        private float currentForwardSpeed;
        private float currentSidestepSpeed;

        private Vector2 currentInput;
        private CharacterController controller;
        
        // Accessor properties
        public Vector2 CurrentInput => currentInput;
        public Vector3 CurrentMoveVelocity => currentMoveVelocity;
        public float CurrentForwardSpeed => currentForwardSpeed;
    
        // Start is called before the first frame update
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            currentForwardSpeed = forwardSpeed;
            currentSidestepSpeed = sidestepSpeed;
        }

        // Update is called once per frame
        private void Update()
        {
            // Get move input
            Vector2 inputDir = new Vector2(currentInput.x, currentInput.y).normalized;
            Debug.Log(inputDir);

            // Handle movement of player
            if (inputDir.magnitude >= 0.01f)
            {
                // Set velocity
                float targetZ = inputDir.y * currentForwardSpeed; // Forward/backward
                float targetX = inputDir.x * currentSidestepSpeed; // Sidestep
            
                // Calculate directional velocity with the local transform of the player
                Vector3 targetMove = transform.right * targetX + 
                                     transform.forward * targetZ;

                // Calculate final velocity with acceleration applied
                currentMoveVelocity =
                    Vector3.MoveTowards(currentMoveVelocity, targetMove, accelerationRate * Time.deltaTime);
            }
            else
            {
                // Calculate final velocity with deceleration applied
                currentMoveVelocity =
                    Vector3.MoveTowards(currentMoveVelocity, Vector3.zero, decelerationRate * Time.deltaTime);
            }
            
            controller.Move(currentMoveVelocity * Time.deltaTime);
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
