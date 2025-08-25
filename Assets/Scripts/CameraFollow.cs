using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Player to follow
    public float smoothSpeed = 5f;  // Higher = snappier, lower = smoother
    public Vector3 offset;          // Optional offset from the player

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position (player + offset)
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move camera towards the target
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Keep camera’s Z unchanged (important in 2D)
        smoothedPosition.z = transform.position.z;

        transform.position = smoothedPosition;
    }
}