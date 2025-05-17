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
        [Range(0.1f, 1f)] public float slopeSpeedModifier;
        [HideInInspector] public Vector3 velocity;

        [Header("Gravity/Ground Settings")]
        public float gravity = -9.81f;
        public float mass = 2;
        [Space(5)]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckDistance = 0.4f;
        [SerializeField] private float slopeCheckDistance = 0.5f;
        [SerializeField] private LayerMask groundMask;
        private bool _isGrounded;

        [Header("Vertical Head Bob")] 
        [SerializeField] private AnimationCurve headBobCurve;
        private float stepLength;
        
        private CharacterController controller;
    
        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();
            stepLength = headBobCurve.keys[headBobCurve.length - 1].time;
        }

        // Update is called once per frame
        void Update()
        {
            // Get forward and side axis inputs
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Calculate gravity depending on if the player is grounded and how heavy they are
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
            if (_isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            // Calculate sprint speed
            float sprintMultiply = Input.GetKey(KeyCode.LeftShift) 
                ? sprintModifier 
                : 1;

            // Set velocity
            velocity.x = x * sidestepSpeed * sprintMultiply;
            velocity.y += mass * gravity * Time.deltaTime;
            velocity.z = z * forwardSpeed * sprintMultiply;
            
            // Slope detection to ensure the player slides down a slope rather than "falling" off of it
            Vector3 horizontalMove = new Vector3(velocity.x, 0, velocity.z);
            if (_isGrounded)
            {
                Vector3 slopeNormal = GetSlopeNormal();
                if (slopeNormal != Vector3.up)
                {
                    horizontalMove = Vector3.ProjectOnPlane(horizontalMove, slopeNormal);
                }
                else velocity.y = 0;
            }

            // Calculate final velocity with the local transform of the player
            Vector3 move = transform.right * horizontalMove.x + 
                           transform.up * velocity.y + 
                           transform.forward * horizontalMove.z;
            
            controller.Move(move * Time.deltaTime);
        }
        
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
