using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrafficSystem
{
    public class AgentNavigationController : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField, MinValue(0f)] private float movementSpeed = 5f;
        [SerializeField, MinValue(0f)] private float rotationSpeed = 5f;

        [Title("Obstacle Avoidance")]
        [SerializeField, MinValue(0f)] private float stoppingDistance = 0.5f;
        [SerializeField, MinValue(0f)] private float raycastDistance = 10f;
        [SerializeField, Tooltip("Ray cast offsets")] private Sensor[] offsets;
        [SerializeField] private LayerMask obstacleLayer;

        [Title("Debug")]
        [SerializeField] private bool _debug = false;
        [ShowIf("_debug"), ShowInInspector] private Vector3 _targetPosition;
        [ShowIf("_debug"), ShowInInspector] private bool _reachedTarget = false;
        [ShowIf("_debug"), ShowInInspector] private bool inWaiting = false;

        [SerializeField] private bool _onDrawGizmos = false;
        [SerializeField, ShowIf("_onDrawGizmos"), ColorPalette] private Color _gizmosColor = Color.red;
        [SerializeField, ShowIf("_onDrawGizmos"), Tooltip("The size of the gizmo sphere to draw at the collision point")] private float _targetSize = 1f;

        private float currentSpeed;
        private List<Vector3> hitPositions = new ();

        private void Awake()
        {
            currentSpeed = movementSpeed;
        }

        private void FixedUpdate()
        {
            if (inWaiting) return;

            if (offsets.Length > 0) HandleObstacleDetection();
            Move();
        }

        /// <summary>
        /// HandleObstacleDetection handles the detection of obstacles.
        /// </summary>
        private void HandleObstacleDetection()
        {
            if (_debug) Debug.Log("HandleObstacleDetection");

            hitPositions.Clear();

            foreach (Sensor offset in offsets)
            {
                Ray ray = new(offset.transform.position, offset.transform.forward);

                if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, obstacleLayer))
                {
                    // Adjust speed based on proximity to the obstacle
                    float distanceToObstacle = hit.distance;
                    float speedAdjustment = Mathf.Lerp(0, movementSpeed, distanceToObstacle / raycastDistance);
                    currentSpeed = speedAdjustment < 0.1 ? 0 : speedAdjustment;

                    if (_debug) Debug.Log($"Obstacle detected at distance: {distanceToObstacle}, adjusted speed: {currentSpeed}");

                    hitPositions.Add(hit.point);
                    return; // Exit the loop if an obstacle is detected
                }
            }

            // No obstacle detected, reset speed to normal
            if (_debug) Debug.Log("No obstacle detected, resetting speed.");

            hitPositions.Add(Vector3.zero);

            currentSpeed = movementSpeed;
        }

        /// <summary>
        /// Move moves the agent towards its target position.
        /// </summary>
        private void Move()
        {
            // Check if the current position of the agent is not equal to the target position
            if (transform.position != _targetPosition)
            {
                // Calculate the direction from the current position to the target position
                Vector3 destinationDirection = _targetPosition - transform.position;

                // Ignore the vertical component of the direction (i.e., set y to 0) to ensure the agent moves only horizontally
                destinationDirection.y = 0f;

                // Calculate the distance between the current position and the target position
                float destinationDistance = destinationDirection.magnitude;

                // Check if the distance to the target position is greater than or equal to the stopping distance
                if (destinationDistance >= stoppingDistance)
                {
                    // Set the flag to indicate that the target has not been reached yet
                    _reachedTarget = false;

                    // Move the agent towards the target position by adding a fraction of the direction vector to the current position
                    // The fraction is calculated based on the current speed and the fixed delta time
                    transform.position += currentSpeed * Time.fixedDeltaTime * destinationDirection.normalized;

                    // Rotate the agent to face the target position by interpolating between the current rotation and the target rotation
                    // The interpolation is done using the Quaternion.Slerp function, which ensures a smooth rotation
                    // The rotation speed is scaled by the fixed delta time to ensure consistent rotation speed
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(destinationDirection),
                        rotationSpeed / 10 * Time.fixedDeltaTime
                    );
                }
                else
                {
                    // If the distance to the target position is less than the stopping distance, set the flag to indicate that the target has been reached
                    _reachedTarget = true;
                }
            }
        }

        /// <summary>
        /// Sets the destination of the agent.
        /// </summary>
        /// <param name="destination">The destination position.</param>
        public void SetDestination(Vector3 destination)
        {
            _targetPosition = destination;
            _reachedTarget = false;
        }

        /// <summary>
        /// Waits for the specified amount of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait.</param>
        public void Wait(float seconds)
        {
            StartCoroutine(WaitCoroutine(seconds));
        }

        /// <summary>
        /// WaitCoroutine is a coroutine that waits for the specified amount of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait.</param>
        private IEnumerator WaitCoroutine(float delay)
        {
            inWaiting = true;
            yield return new WaitForSeconds(delay);
            inWaiting = false;
        }

        /// <summary>
        /// Returns true if the target has been reached.
        /// </summary>
        public bool ReachedDestination => _reachedTarget;

        /// <summary>
        /// Gets the current speed of the agent.
        /// </summary>
        public float Speed => currentSpeed;

        /// <summary>
        /// Gets the target position of the agent.
        /// </summary>
        public Vector3 TargetPosition => _targetPosition;

        private void OnDrawGizmos()
        {
            if (!_onDrawGizmos || offsets == null || offsets.Length == 0) return;


            foreach (Sensor offset in offsets)
            {
                Gizmos.color = offset.GizmosColor;
                Gizmos.DrawRay(offset.transform.position, offset.transform.forward * offset.RaycastDistance);
            }

            if (hitPositions != null && hitPositions.Count > 0)
            {
                foreach (var position in hitPositions)
                {
                    if (position != Vector3.zero) // Ignora posizioni non valide
                    {
                        Gizmos.DrawSphere(position, _targetSize);
                    }
                }
            }
        }
    }
}