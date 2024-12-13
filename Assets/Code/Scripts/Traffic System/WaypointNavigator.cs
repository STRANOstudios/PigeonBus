using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrafficSystem
{
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

        private AgentNavigationController agent;

        private bool frontBack = false;
        private bool intersectionReached = false;

        private Waypoint nextWaypoint;

        private void Awake()
        {
            agent = GetComponent<AgentNavigationController>();
        }

        private void Start()
        {
            agent.SetDestination(currentWaypoint.GetPosition());
        }

        private void FixedUpdate()
        {
            ReachedDestination();

            if (_isPlayer && Vector3.Distance(transform.position, currentWaypoint.GetPosition()) <= QTEDistance && nextWaypoint == null)
            {
                IntersectionBehaviorPlayer();
            }

            if (!_isPlayer)
            {
                IntersectionBehaviorEntity();
            }
        }

        private void ReachedDestination()
        {
            if (agent.ReachedDestination && !intersectionReached)
            {
                Waypoint tmp = frontBack ? currentWaypoint.prevWaypoint : currentWaypoint.nextWaypoint;

                bool shouldBranch = false;

                if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
                {
                    shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio;
                }

                if (shouldBranch)
                {
                    tmp = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
                }

                if (tmp == null)
                {
                    frontBack = !frontBack;
                    tmp = frontBack ? currentWaypoint.prevWaypoint : currentWaypoint.nextWaypoint;
                }

                if (_debug) Debug.Log($"tmp {tmp} | currentWaypoint {currentWaypoint} | Direction: {(frontBack ? "Back" : "Front")}");

                currentWaypoint = tmp;

                if (currentWaypoint != null)
                {
                    agent.SetDestination(currentWaypoint.GetPosition());
                }
                else if (_debug)
                {
                    Debug.Log("No more waypoints in either direction.");
                }
            }
            else if (agent.ReachedDestination)
            {
                if (_debug) Debug.Log("Intersection reached");

                currentWaypoint = nextWaypoint;
                agent.SetDestination(currentWaypoint.GetPosition());

                intersectionReached = false;
                nextWaypoint = null;
            }
        }

        private void IntersectionBehaviorPlayer()
        {
            if (_debug) Debug.Log($"Current Waypoint: {currentWaypoint}");

            if (currentWaypoint is WaypointIntersection)
            {
                WaypointIntersection waypointIntersection = currentWaypoint as WaypointIntersection;

                Transform waypointParent = waypointIntersection.transform.parent.transform.parent;
                Transform canvasGroupParent = canvasGroup.transform.parent;
                canvasGroupParent.position = new Vector3(waypointParent.position.x, canvasGroupParent.position.y, waypointParent.position.z);

                GetComponent<FadeInOut>().FadeIn(canvasGroup);

                btnStop.gameObject.SetActive(false);
                btnLeft.gameObject.SetActive(false);
                btnRight.gameObject.SetActive(false);

                if (waypointIntersection.waypointType == WaypointType.Stop)
                {
                    btnStop.gameObject.SetActive(true);
                }
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
            else if (canvasGroup.alpha > 0)
            {
                GetComponent<FadeInOut>().FadeOut(canvasGroup);
            }
        }

        private void IntersectionBehaviorEntity()
        {
            if (currentWaypoint is WaypointIntersection)
            {
                WaypointIntersection waypointIntersection = currentWaypoint as WaypointIntersection;

                if (waypointIntersection.waypointType == WaypointType.Stop) return;

                List<Direction> waypoints = new();

                if (waypointIntersection.leftWaypoint != null)
                {
                    waypoints.Add(Direction.Left);
                }
                if (waypointIntersection.rightWaypoint != null)
                {
                    waypoints.Add(Direction.Right);
                }
                if (waypointIntersection.nextWaypoint != null)
                {
                    waypoints.Add(Direction.Forward);
                }

                int random = Random.Range(0, waypoints.Count);
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

        public void Stop()
        {
            SetNextWaypoint(currentWaypoint as WaypointIntersection, 1);
        }

        public void Left()
        {
            SetNextWaypoint(currentWaypoint as WaypointIntersection, 0);
        }

        public void Right()
        {
            SetNextWaypoint(currentWaypoint as WaypointIntersection, 1);
        }

        private void SetNextWaypoint(WaypointIntersection waypointIntersection, int index = 0) /*( 0 = left | 1 = right)*/
        {
            if (_debug) Debug.Log($"SetNextWaypoint {index}");

            frontBack = false;
            nextWaypoint = waypointIntersection.nextWaypoint;

            switch (index)
            {
                case 0:
                    nextWaypoint = waypointIntersection.leftWaypoint;
                    break;
                case 1:
                    nextWaypoint = waypointIntersection.rightWaypoint;
                    break;
            }

            intersectionReached = true;
            if (_isPlayer) GetComponent<FadeInOut>().FadeOut(canvasGroup);
        }

        private enum Direction
        {
            Left,
            Right,
            Forward
        }
    }
}
