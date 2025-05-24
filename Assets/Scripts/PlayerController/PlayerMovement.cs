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
        [HideInInspector] public Vector3 velocity;

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
            
            //if (isGrounded)
                //HeadBob(x, z);
            
            // Calculate sprint speed
            float sprintMultiply = Input.GetKey(KeyCode.LeftShift) 
                ? sprintModifier 
                : 1;

            // Set velocity
            velocity.x = x * sidestepSpeed * sprintMultiply;
            velocity.z = z * forwardSpeed * sprintMultiply;

            // Calculate final velocity with the local transform of the player
            Vector3 move = transform.right * velocity.x + 
                           transform.forward * velocity.z;
            
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
    }
}
