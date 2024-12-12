using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class PlayerController : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0f)] private float speed = 10f;
    [SerializeField] Vector3 startPosition = Vector3.zero;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [SerializeField] private bool _onDrawGizmos = false;
    [SerializeField, ShowIf("_onDrawGizmos"), ColorPalette] Color _gizmosColor = Color.green;
    [SerializeField, MinValue(0f), ShowIf("_onDrawGizmos")] private float _targetSize = 1f;

    // States
    private IFinalState _currentState;
    private IFinalState _movementState;
    private IFinalState _stopState;

    private void Start()
    {
        _movementState = new Run(this);
        _stopState = new Stop();
        _currentState = _movementState;
        _currentState.Enter();
    }

    private void FixedUpdate()
    {
        _currentState?.Update();
    }

    /// <summary>
    /// Switches the player state to a new state.
    /// </summary>
    /// <param name="newState"></param>
    public void SwitchState(IFinalState newState)
    {
        if (_debug) Debug.Log($"Switching state to {newState.GetType().Name}");

        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    /// <summary>
    /// Starts a transition coroutine that switches the player state to Stop and back to Movement after a delay.
    /// </summary>
    /// <param name="delay">Time in seconds to wait in the Stop state before resuming Movement.</param>
    public void StartTransitionCoroutine(float delay)
    {
        StartCoroutine(TransitionCoroutine(delay));
    }

    private IEnumerator TransitionCoroutine(float delay)
    {
        SwitchState(_stopState); // Switch to Stop state
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        SwitchState(_movementState); // Switch back to Movement state
    }

    /// <summary>
    /// Resets the player's position to the start position.
    /// </summary>
    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity; 
    }

    /// <summary>
    /// Returns the speed of the player.
    /// </summary>
    public float Speed => speed;

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos) return;

        Gizmos.color = _gizmosColor;
        Gizmos.DrawSphere(startPosition, _targetSize);
    }
}
