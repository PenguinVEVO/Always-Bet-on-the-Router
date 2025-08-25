using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mitchel.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerHeadBob : MonoBehaviour
    {
        private enum HeadBobTypeEnum { Procedural, CustomCurve }
        [Tooltip("Note that you only need to set the values for the specific type of head bob you need.")]
        [SerializeField] private HeadBobTypeEnum HeadBobType;
        
        [Header("Curve Head Bob")] 
        [SerializeField] private AnimationCurve headBobCurve;
        private float stepTimeElapsed = 0;
        private float stepLength;
        
        [Header("Procedural Head Bob")]
        [SerializeField] private float headBobAmplitude;
        [SerializeField] private float headBobFrequency;
        private float initialCameraOffset;
        
        [Header("Object References")]
        [SerializeField] private Camera playerCamera;
        private PlayerMovement playerMove;
        
        // Start is called before the first frame update
        void Start()
        {
            playerMove = GetComponent<PlayerMovement>();
            stepLength = headBobCurve.keys[headBobCurve.length - 1].time;
            initialCameraOffset = playerCamera.transform.localPosition.y;
        }

        // Update is called once per frame
        void Update()
        {
            switch (HeadBobType)
            {
                case HeadBobTypeEnum.Procedural:
                    ProceduralHeadBob();
                    break;
                case HeadBobTypeEnum.CustomCurve:
                    CurveHeadBob();
                    break;
            }
        }

        private void ProceduralHeadBob()
        {
            float movementAmplitude = playerMove.CurrentMoveVelocity.magnitude / playerMove.CurrentForwardSpeed;
            float finalAmplitude = Mathf.Clamp01(movementAmplitude) * headBobAmplitude;
            
            Vector3 pos = playerCamera.transform.position;
            pos.y = initialCameraOffset + Mathf.Sin(Time.time * headBobFrequency) * finalAmplitude;
            playerCamera.transform.position = pos;
        }

        private void CurveHeadBob()
        {
            
        }
    }
}