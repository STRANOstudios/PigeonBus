using Sirenix.OdinInspector;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Required, Tooltip("The player script")] private Turn player;
    [SerializeField, Tooltip("The angle of the intersection")] private float angle = 0f;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    void Start() { }

    /// <summary>
    /// Trigger the intersection
    /// </summary>
    public void Trigger()
    {
        if (_debug) Debug.Log($"Triggering intersection at angle {angle}");

        // call the turn function in the script on the player
        player.TurnPlayer(angle);
        angle = 0f;
    }

    /// <summary>
    /// Set the angle of the intersection
    /// </summary>
    /// <param name="angle"></param>
    public void SetAngle(float angle)
    {
        if (_debug) Debug.Log($"Setting angle to {angle}");

        this.angle = angle;
    }
}
