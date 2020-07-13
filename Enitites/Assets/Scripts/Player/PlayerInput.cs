using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool IsFiring => Input.GetButton("Fire1");

    // return input vector in camera space
    public Vector3 GetCameraSpaceInputDirection(Camera cam)
    {
        // "classic" Input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // xz-vector from input values
        Vector3 inputDirection = new Vector3(h, 0, v);

        if (cam == null)
        {
            return inputDirection;
        }

        // multiply input by camera axes to convert into camera space (but still along the xz-plane)
        Vector3 cameraRight = cam.transform.right;
        Vector3 cameraForward = cam.transform.forward;

        return cameraRight * inputDirection.x + cameraForward * inputDirection.z;
    }


}
