using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TrafficSystem
{
    [RequireComponent(typeof(AgentNavigationController))]
    public class WaypointNavigator : MonoBehaviour
    {
        [Title("Reference")]
        [SerializeField, Required] private Waypoint currentWaypoint;

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
                IntersectionBehavior();
            }
        }

        private void ReachedDestination()
        {
            if (agent.ReachedDestination && !intersectionReached)
            {
                Waypoint tmp = frontBack ? currentWaypoint.prevWaypoint : currentWaypoint.nextWaypoint;

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

        private void IntersectionBehavior()
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
            if(_debug) Debug.Log($"SetNextWaypoint {index}");

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
            GetComponent<FadeInOut>().FadeOut(canvasGroup);
        }
    }
}
