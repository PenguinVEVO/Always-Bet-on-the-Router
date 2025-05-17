using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mitchel.PlayerController
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")] 
        public float forwardSpeed = 12f;
        public float sidestepSpeed = 10f;
        [Range(1.1f, 4f)] public float sprintModifier;
        [Range(0.01f, 1f)] public float slopeSpeedModifier;
        [HideInInspector] public Vector3 velocity;

        [Header("Gravity/Ground Settings")]
        public float gravity = -9.81f;
        public float mass = 2;
        [Space(5)] 
        [Tooltip("The minimum angle a slope needs to be at in order to slow the player's move speed")] 
        [Range(1f, 40f)] [SerializeField] private float minSlopeMovementAngle;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckDistance = 0.4f;
        [SerializeField] private float slopeCheckDistance = 0.5f;
        [SerializeField] private LayerMask groundMask;
        private bool isGrounded;

        [Header("Vertical Head Bob")] 
        [SerializeField] private AnimationCurve headBobCurve;
        [SerializeField] private float cameraRollAmount;
        [SerializeField] private float cameraRollSpeed;
        private float stepLength;
        
        private CharacterController controller;
        private PlayerMouseLook mouseLook;
    
        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();
            mouseLook = GetComponent<PlayerMouseLook>();
            stepLength = headBobCurve.keys[headBobCurve.length - 1].time;
        }

        // Update is called once per frame
        void Update()
        {
            // Get forward and side axis inputs
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            

            // Calculate gravity depending on if the player is grounded and how heavy they are
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -20f;
            }
            
            //if (isGrounded)
                //HeadBob(x, z);
            
            // Calculate sprint speed
            float sprintMultiply = Input.GetKey(KeyCode.LeftShift) 
                ? sprintModifier 
                : 1;

            // Set velocity
            velocity.x = x * sidestepSpeed * sprintMultiply;
            velocity.y += mass * gravity * Time.deltaTime;
            velocity.z = z * forwardSpeed * sprintMultiply;
            
            // Slope detection to ensure the player slides down a slope rather than "falling" off of it
            // Also slow the player if the slope is above a minimum angle
            // Code help by ChatGPT: https://chatgpt.com/share/6828c7a9-8744-8002-a57a-d810a79ba04a
            Vector3 horizontalMove = new Vector3(velocity.x, 0, velocity.z);
            float slopeMultiply = 1;
            if (isGrounded)
            {
                Vector3 slopeNormal = GetSlopeNormal();
                if (slopeNormal != Vector3.up)
                {
                    horizontalMove = Vector3.ProjectOnPlane(horizontalMove, slopeNormal);
                    float slopeAngle = Vector3.Angle(slopeNormal, Vector3.up);
                    Debug.Log(slopeAngle);
                    if (slopeAngle > minSlopeMovementAngle)
                        slopeMultiply = slopeSpeedModifier;
                }
                else
                {
                    velocity.y = 0;
                }
            }
            
            

            // Calculate final velocity with the local transform of the player
            Vector3 move = transform.right * horizontalMove.x + 
                           transform.up * (slopeMultiply * velocity.y) + 
                           transform.forward * horizontalMove.z;
            
            controller.Move(move * Time.deltaTime);
        }

        // TODO: Set this up properly
        /*private void HeadBob(float x, float z)
        {
            Quaternion currentRotation = mouseLook.playerCamera.transform.rotation;
            Quaternion endRotation = new Quaternion(currentRotation.x,
                                                    currentRotation.y, 
                                                    x * cameraRollAmount, 
                                                    currentRotation.w);
            float rollStep = cameraRollSpeed * Time.deltaTime;
            Quaternion roll = Quaternion.RotateTowards(currentRotation, endRotation, rollStep);
            mouseLook.playerCamera.transform.rotation = roll;
        }*/
        
        #region =====  UTILITY FUNCTIONS  =====
        Vector3 GetSlopeNormal()
        {
            Vector3 origin = transform.position + controller.center + Vector3.down * (controller.height / 2);
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, slopeCheckDistance + 0.1f, groundMask))
            {
                return hit.normal;
            }
            return Vector3.up;
        }
        
        void OnDrawGizmos()
        {
            if (controller != null)
            {
                Vector3 slopeRayOrigin = transform.position + controller.center + Vector3.down * (controller.height / 2);
                float slopeRayDistance = slopeCheckDistance + 0.1f;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(slopeRayOrigin, slopeRayOrigin + Vector3.down * slopeRayDistance);
            }
        }
        #endregion
    }
}
