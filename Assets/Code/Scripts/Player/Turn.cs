using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Turn : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0f)] private float turnSpeed = 10f;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [ShowIf("_debug"), ReadOnly, ShowInInspector] private float currentAngle = 0f;

    private void Start()
    {
        currentAngle = transform.eulerAngles.y;
    }

    /// <summary>
    /// Rotates the player based on the caller's rotation and the player's current rotation.
    /// </summary>
    /// <param name="caller">The transform of the object that triggered this turn.</param>
    public void TurnPlayer(Transform caller)
    {
        // Get the current rotation of the player and the caller
        float playerYRotation = NormalizeAngle(transform.eulerAngles.y);
        float callerYRotation = NormalizeAngle(caller.eulerAngles.y);

        // Calculate the delta (difference) between the player and the caller rotation
        float delta = Mathf.DeltaAngle(playerYRotation, callerYRotation);

        // Determine whether to turn left or right, based on the delta value
        float relativeAngle = 0f;

        // This condition ensures that when both rotations are the same (0,0) or (180,180) etc,
        // it will return a -90 degree turn. Otherwise, it will return 90 or -90 based on direction.
        if (delta == 0)
            relativeAngle = -90f;  // If no difference, turn left (by default)

        else if (delta > 0)
            relativeAngle = -90f;  // Right turns are negative (90 in this context)

        else if (delta < 0)
            relativeAngle = 90f;   // Left turns are positive (90 in this context)

        // Update the target angle for rotation
        currentAngle = transform.rotation.eulerAngles.y + relativeAngle;

        if (_debug)
            Debug.Log($"Caller rotation: {callerYRotation}, Player rotation: {playerYRotation}, Delta: {delta}, Relative angle: {relativeAngle}, Target angle: {currentAngle}");

        // Stop any ongoing rotation and start a new one
        StopAllCoroutines();
        StartCoroutine(TurnCoroutine(currentAngle));
    }

    /// <summary>
    /// Rotates the player by a relative angle (e.g., 90 or -90).
    /// </summary>
    /// <param name="relativeAngle">The angle to turn, relative to the current rotation.</param>
    public void TurnPlayer(float relativeAngle)
    {
        if(relativeAngle == 0) return;

        currentAngle += relativeAngle;

        if (_debug)
            Debug.Log($"Relative angle: {relativeAngle}, Target angle: {currentAngle}");

        StopAllCoroutines();
        StartCoroutine(TurnCoroutine(currentAngle));
    }

    /// <summary>
    /// Normalizes the angle to be between -180 and 180 degrees.
    /// </summary>
    /// <param name="angle">Angle in degrees.</param>
    /// <returns>Normalized angle between -180 and 180 degrees.</returns>
    private float NormalizeAngle(float angle)
    {
        angle = (angle + 180) % 360;  // Normalize to 0-360
        if (angle < 0) angle += 360;  // Handle negative angles
        return angle - 180;  // Normalize to -180 to 180 range
    }

    /// <summary>
    /// Coroutine to smoothly rotate the object to the target angle.
    /// </summary>
    /// <param name="targetAngle">The target angle in world space.</param>
    /// <returns>Coroutine enumerator.</returns>
    private IEnumerator TurnCoroutine(float targetAngle)
    {
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
