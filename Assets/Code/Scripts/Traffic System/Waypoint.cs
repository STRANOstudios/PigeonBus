using Sirenix.OdinInspector;
using UnityEngine;

namespace TrafficSystem
{
    public class Waypoint : MonoBehaviour
    {
        [Title("Settings")]
        public Waypoint prevWaypoint;
        public Waypoint nextWaypoint;
        [Range(0.1f, 5f)] public float width = 1f;

        [Button]
        public void ResetWaypoints()
        {
            prevWaypoint = null;
            nextWaypoint = null;
        }

        public Vector3 GetPosition()
        {
            Vector3 minbound = transform.position + transform.right * width / 2f;
            Vector3 maxbound = transform.position - transform.right * width / 2f;

            return Vector3.Lerp(minbound, maxbound, Random.Range(0f, 1f));
        }
    }
}

