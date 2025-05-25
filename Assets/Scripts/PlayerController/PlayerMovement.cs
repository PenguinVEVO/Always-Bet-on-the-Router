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
        private Vector3 currentMoveVelocity;
        private float currentForwardSpeed;
        private float currentSidestepSpeed;
        private float inputGraceTimer = 0.1f;
        private float gracePeriod = 0.1f;

        [Header("Vertical Head Bob")] 
        [SerializeField] private AnimationCurve headBobCurve;
        [SerializeField] private float cameraRollAmount;
        [SerializeField] private float cameraRollSpeed;
        private float stepLength;

        private Vector2 currentInput;
        private Vector3 lastInputMoveDirection = Vector3.zero;
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
            Vector2 inputDir = new Vector2(currentInput.x, currentInput.y).normalized;

            if (inputDir.magnitude >= 0.01f)
            {
                inputDir.Normalize();
                
                // Set velocity
                float targetX = inputDir.x * currentForwardSpeed; // Forward/backward
                float targetZ = inputDir.y * currentSidestepSpeed; // Sidestep
            
                // Calculate directional velocity with the local transform of the player
                Vector3 targetMove = transform.right * targetX + 
                                     transform.forward * targetZ;
                
                // Check how similar new input is to the last input direction
                float alignment = Vector3.Dot(lastInputMoveDirection.normalized, targetMove.normalized);
                
                // If input is clearly in a new direction, reset grace timer
                if (alignment < 0.99f)
                {
                    inputGraceTimer += Time.deltaTime;

                    if (inputGraceTimer >= gracePeriod)
                    {
                        // Accept the new input direction after grace period
                        lastInputMoveDirection = targetMove;
                        inputGraceTimer = 0f;
                    }
                }
                else
                {
                    // Input is similar, update direction and reset timer
                    lastInputMoveDirection = targetMove;
                    inputGraceTimer = 0f;
                }

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
