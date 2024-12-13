using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrafficSystem
{
    // This script requires the AgentNavigationController component to be attached to the same GameObject.
    [RequireComponent(typeof(AgentNavigationController))]
    public class WaypointNavigator : MonoBehaviour
    {
        [Title("Reference")]
        [SerializeField, Required] public Waypoint currentWaypoint;

        [Title("UI")]
        [SerializeField, ShowIf("_isPlayer"), Required] private CanvasGroup canvasGroup;
        [SerializeField, ShowIf("_isPlayer"), Required] private Button btnStop;
        [SerializeField, ShowIf("_isPlayer"), Required] private Button btnLeft;
        [SerializeField, ShowIf("_isPlayer"), Required] private Button btnRight;

        [Title("Settings")]
        [SerializeField] private bool _isPlayer = false;
        [SerializeField, ShowIf("_isPlayer"), MinValue(0f)] private float QTEDistance = 5f;

        [Title("Debug")]
        [SerializeField] private bool _debug = false;

        // The agent navigation controller component.
        private AgentNavigationController agent;

        // Flags to track the navigator's state.
        private bool frontBack = false; // Whether the navigator is moving forward or backward.
        private bool intersectionReached = false; // Whether the navigator has reached an intersection.

        // The next waypoint to navigate to.
        private Waypoint nextWaypoint;

        // Called when the script is initialized.
        private void Awake()
        {
            // Get the agent navigation controller component.
            agent = GetComponent<AgentNavigationController>();
        }

        // Called when the script is started.
        private void Start()
        {
            // Set the destination of the agent to the current waypoint.
            agent.SetDestination(currentWaypoint.GetPosition());
        }

        // Called every fixed framerate frame.
        private void FixedUpdate()
        {
            // Check if the navigator has reached its destination.
            ReachedDestination();

            // If the navigator is for a player and is within the QTE distance of the current waypoint, handle player interaction.
            if (_isPlayer && Vector3.Distance(transform.position, currentWaypoint.GetPosition()) <= QTEDistance && nextWaypoint == null)
            {
                IntersectionBehaviorPlayer();
            }

            // If the navigator is for an entity, handle entity behavior.
            if (!_isPlayer)
            {
                IntersectionBehaviorEntity();
            }
        }

        // Called when the navigator reaches its destination.
        private void ReachedDestination()
        {
            // If the agent has reached its destination and this is not an intersection, update the current waypoint.
            if (agent.ReachedDestination && !intersectionReached)
            {
                // Get the previous or next waypoint based on the frontBack flag.
                Waypoint tmp = frontBack ? currentWaypoint.prevWaypoint : currentWaypoint.nextWaypoint;

                // Check if the current waypoint has branches and if so, randomly select a branch.
                bool shouldBranch = false;
                if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
                {
                    shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio;
                }

                // If branching is enabled, select a random branch.
                if (shouldBranch)
                {
                    tmp = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
                }

                // If the selected waypoint is null, switch the frontBack flag and try again.
                if (tmp == null)
                {
                    frontBack = !frontBack;
                    tmp = frontBack ? currentWaypoint.prevWaypoint : currentWaypoint.nextWaypoint;
                }

                // Update the current waypoint and set the agent's destination.
                currentWaypoint = tmp;
                if (currentWaypoint != null)
                {
                    agent.SetDestination(currentWaypoint.GetPosition());
                }
                else if (_debug)
                {
                    // Log a message if there are no more waypoints in either direction.
                    Debug.Log("No more waypoints in either direction.");
                }
            }
            // If the agent has reached its destination and this is an intersection, update the current waypoint and reset the intersection flag.
            else if (agent.ReachedDestination)
            {
                if (_debug) Debug.Log("Intersection reached");

                currentWaypoint = nextWaypoint;
                agent.SetDestination(currentWaypoint.GetPosition());

                intersectionReached = false;
                // Reset the next waypoint and intersection flag.
                nextWaypoint = null;
            }
        }

        // Handles player interaction at an intersection.
        private void IntersectionBehaviorPlayer()
        {
            // Log a message if debug is enabled.
            if (_debug) Debug.Log($"Current Waypoint: {currentWaypoint}");

            // Check if the current waypoint is an intersection.
            if (currentWaypoint is WaypointIntersection)
            {
                // Get the intersection waypoint.
                WaypointIntersection waypointIntersection = currentWaypoint as WaypointIntersection;

                // Get the parent transform of the waypoint and the canvas group.
                Transform waypointParent = waypointIntersection.transform.parent.transform.parent;
                Transform canvasGroupParent = canvasGroup.transform.parent;

                // Move the canvas group to the intersection waypoint.
                canvasGroupParent.position = new Vector3(waypointParent.position.x, canvasGroupParent.position.y, waypointParent.position.z);

                // Fade in the canvas group.
                GetComponent<FadeInOut>().FadeIn(canvasGroup);

                // Disable all buttons by default.
                btnStop.gameObject.SetActive(false);
                btnLeft.gameObject.SetActive(false);
                btnRight.gameObject.SetActive(false);

                // Enable the stop button if the intersection is a stop.
                if (waypointIntersection.waypointType == WaypointType.Stop)
                {
                    btnStop.gameObject.SetActive(true);
                }
                // Enable the left and right buttons if the intersection is a normal intersection.
                else if (waypointIntersection.waypointType == WaypointType.Normal)
                {
                    if (waypointIntersection.leftWaypoint != null)
                    {
                        btnLeft.gameObject.SetActive(true);
                    }
                    if (waypointIntersection.rightWaypoint != null)
                    {
                        btnRight.gameObject.SetActive(true);
                    }
                }
            }
            // If the current waypoint is not an intersection and the canvas group is visible, fade it out.
            else if (canvasGroup.alpha > 0)
            {
                GetComponent<FadeInOut>().FadeOut(canvasGroup);
            }
        }

        // Handles entity behavior at an intersection.
        private void IntersectionBehaviorEntity()
        {
            // Check if the current waypoint is an intersection.
            if (currentWaypoint is WaypointIntersection)
            {
                // Get the intersection waypoint.
                WaypointIntersection waypointIntersection = currentWaypoint as WaypointIntersection;

                // If the intersection is a stop, do nothing.
                if (waypointIntersection.waypointType == WaypointType.Stop) return;

                // Create a list of possible directions.
                List<Direction> waypoints = new();

                // Add the left direction if the left waypoint is not null.
                if (waypointIntersection.leftWaypoint != null)
                {
                    waypoints.Add(Direction.Left);
                }
                // Add the right direction if the right waypoint is not null.
                if (waypointIntersection.rightWaypoint != null)
                {
                    waypoints.Add(Direction.Right);
                }
                // Add the forward direction if the next waypoint is not null.
                if (waypointIntersection.nextWaypoint != null)
                {
                    waypoints.Add(Direction.Forward);
                }

                // Randomly select a direction.
                int random = Random.Range(0, waypoints.Count);

                // Switch on the selected direction.
                switch (waypoints[random])
                {
                    case Direction.Left:
                        Left();
                        break;
                    case Direction.Right:
                        Right();
                        break;
                    case Direction.Forward:
                        break;
                }
            }
        }

        // Called when the player presses the stop button.
        public void Stop()
        {
            // Set the next waypoint to the current intersection waypoint.
            SetNextWaypoint(currentWaypoint as WaypointIntersection, 1);
        }

        // Called when the player presses the left button.
        public void Left()
        {
            // Set the next waypoint to the left waypoint of the current intersection waypoint.
            SetNextWaypoint(currentWaypoint as WaypointIntersection, 0);
        }

        // Called when the player presses the right button.
        public void Right()
        {
            // Set the next waypoint to the right waypoint of the current intersection waypoint.
            SetNextWaypoint(currentWaypoint as WaypointIntersection, 1);
        }

        // Sets the next waypoint based on the intersection waypoint and direction.
        private void SetNextWaypoint(WaypointIntersection waypointIntersection, int index = 0) /*( 0 = left | 1 = right)*/
        {
            // Log a message if debug is enabled.
            if (_debug) Debug.Log($"SetNextWaypoint {index}");

            // Reset the frontBack flag.
            frontBack = false;

            // Set the next waypoint based on the direction.
            switch (index)
            {
                case 0:
                    nextWaypoint = waypointIntersection.leftWaypoint;
                    break;
                case 1:
                    nextWaypoint = waypointIntersection.rightWaypoint;
                    break;
            }

            // Set the 
            // Set the intersection reached flag to true.
            intersectionReached = true;

            // Fade out the canvas group if the player is interacting.
            if (_isPlayer) GetComponent<FadeInOut>().FadeOut(canvasGroup);
        }

        // Enum for the possible directions.
        private enum Direction
        {
            Left,
            Right,
            Forward
        }
    }
}
