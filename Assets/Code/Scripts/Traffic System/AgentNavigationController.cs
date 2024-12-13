using Sirenix.OdinInspector;
using System.Collections;
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
        [SerializeField, Tooltip("Ray cast offset")] private Vector3 offset = Vector3.up;
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
        private Vector3 hitPosition;

        private void Awake()
        {
            currentSpeed = movementSpeed;
        }

        private void FixedUpdate()
        {
            if (inWaiting) return;

            HandleObstacleDetection();
            Move();
        }

        private void HandleObstacleDetection()
        {
            if (_debug) Debug.Log("HandleObstacleDetection");

            Ray ray = new(transform.TransformPoint(offset), transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, obstacleLayer))
            {
                // Adjust speed based on proximity to the obstacle
                float distanceToObstacle = hit.distance;
                float speedAdjustment = Mathf.Lerp(0, movementSpeed, distanceToObstacle / raycastDistance);
                currentSpeed = speedAdjustment < 0.1 ? 0 : speedAdjustment;

                if (_debug) Debug.Log($"Obstacle detected at distance: {distanceToObstacle}, adjusted speed: {currentSpeed}");

                hitPosition = hit.point;
            }
            else
            {
                // No obstacle detected, reset speed to normal
                if (_debug) Debug.Log("No obstacle detected, resetting speed.");

                hitPosition = Vector3.zero;

                currentSpeed = movementSpeed;
            }
        }

        private void Move()
        {
            if (transform.position != _targetPosition)
            {
                Vector3 destinationDirection = _targetPosition - transform.position;
                destinationDirection.y = 0f;

                float destinationDistance = destinationDirection.magnitude;

                if (destinationDistance >= stoppingDistance)
                {
                    _reachedTarget = false;

                    transform.position += currentSpeed * Time.fixedDeltaTime * destinationDirection.normalized;

                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(destinationDirection),
                        rotationSpeed / 10 * Time.fixedDeltaTime
                    );
                }
                else
                {
                    _reachedTarget = true;
                }
            }
        }

        /// <summary>
        /// Sets the destination
        /// </summary>
        /// <param name="destination"></param>
        public void SetDestination(Vector3 destination)
        {
            _targetPosition = destination;
            _reachedTarget = false;
        }

        /// <summary>
        /// Waits for the specified amount of seconds
        /// </summary>
        /// <param name="seconds"></param>
        public void Wait(float seconds)
        {
            StartCoroutine(WaitCoroutine(seconds));
        }

        private IEnumerator WaitCoroutine(float delay)
        {
            inWaiting = true;
            yield return new WaitForSeconds(delay);
            inWaiting = false;
        }

        /// <summary>
        /// Returns true if the target has been reached
        /// </summary>
        public bool ReachedDestination => _reachedTarget;

        public float Speed => currentSpeed;
        public Vector3 TargetPosition => _targetPosition;

        private void OnDrawGizmos()
        {
            if (!_onDrawGizmos || offset == null) return;

            Gizmos.color = _gizmosColor;
            Gizmos.DrawRay(transform.TransformPoint(offset), transform.forward * raycastDistance);

            if (hitPosition != Vector3.zero)
            {
                Gizmos.DrawSphere(hitPosition, _targetSize);
            }
        }
    }
}
