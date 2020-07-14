using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    /// <summary>
    /// Handles player movement (turret faces the mouse, chassis/body 
    /// smoothly rotates to face the input direction)  
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMover : MonoBehaviour
    {
        // reference to tank body/chassis; should be untransformed relative to player transform
        [SerializeField] private Transform chassisTransform;

        // translation speed in units per second 
        [SerializeField] private float moveSpeed = 3f;

        // percentage to turn between current chassis angle and input angle (smaller values work best here)
        [Range(0.05f, 0.3f)] [SerializeField] private float turnSpeed = 0.1f;

        // uses Rigidbody for smoother interpolated movement
        private Rigidbody playerRigidbody;

        // layer mask to detect mouse position
        public LayerMask groundLayerMask;

        public void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        public void Start()
        {
            // disconnect the chassis transform for easier rotation
            if (chassisTransform != null)
            {
                chassisTransform.parent = null;
            }
        }

        // top down player movement mechanic
        public void MovePlayer(Vector3 direction)
        {
            // restrict player to xz plane
            Vector3 moveDirection = new Vector3(direction.x, 0f, direction.z);

            // normalize and scale vector per moveSpeed
            moveDirection = moveDirection.normalized * moveSpeed * Time.deltaTime;

            // add to rigidbody
            playerRigidbody.MovePosition(transform.position + moveDirection);

            // align the tank body/chassis 
            AlignChassis(moveDirection);
        }


        // return a rotation to face a specific world space position (using y rotation)
        private Quaternion GetRotationToTarget(Transform xform, Vector3 targetPosition)
        {
            // get a normalized vector to the target on the xz-plane
            Vector3 direction = targetPosition - xform.position;
            direction.y = 0f;
            direction.Normalize();

            // convert Vector3 to Quaternion and return
            return Quaternion.LookRotation(direction);
        }

        // returns correction rotation to face the mouse pointer (using y rotation)
        private Quaternion GetRotationToMouse(Transform xform, Camera cam)
        {
            if (cam == null)
            {
                Debug.Log("PLAYERMOVER GetRotationToMouse: no camera");
                return xform.rotation;
            }

            // use Raycast and GroundLayer mask to calculate mouse position in world space
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //if (Physics.Raycast(ray, out hit, groundLayerMask))
            if (Physics.Raycast(ray, out hit, groundLayerMask))
            {
                return GetRotationToTarget(xform, hit.point);
            }

            return xform.rotation;
        }

        // turn the player/turret to face the mouse position
        public void AimAtMousePosition(Camera cam)
        {
            if (cam != null)
            {
                transform.rotation = GetRotationToMouse(transform, cam);
            }
        }

        // sync the chassis position and smoothly rotate to aim at the input direction
        public void AlignChassis(Vector3 direction)
        {
            // update the chassis position 
            chassisTransform.position = playerRigidbody.transform.position;

            // if we have some keyboard input...
            if (direction.magnitude > 0.001f)
            {
                // ...calculate the desired rotation
                Quaternion targetRot = Quaternion.LookRotation(direction);

                // slerp to blend rotations
                chassisTransform.rotation = Quaternion.Slerp(chassisTransform.rotation, targetRot, turnSpeed);

                // note: cheating here.  we never reach the target rotation but looks good with smaller turnSpeed values
            }
        }

    }
}