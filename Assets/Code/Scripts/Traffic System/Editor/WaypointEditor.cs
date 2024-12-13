using UnityEditor;
using UnityEngine;

namespace TrafficSystem
{
    [InitializeOnLoad()]
    public class WaypointEditor
    {
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
        public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
        {
            if(waypoint is WaypointIntersection) return;

            if ((gizmoType & GizmoType.Selected) != 0)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.yellow * 0.5f;
            }

            Gizmos.DrawSphere(waypoint.transform.position, 1f);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2f), waypoint.transform.position + (-waypoint.transform.right * waypoint.width / 2f));

            if (waypoint.prevWaypoint != null)
            {
                Gizmos.color = Color.red;
                Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
                Vector3 offsetTo = waypoint.prevWaypoint.transform.right * waypoint.prevWaypoint.width / 2f;

                Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.prevWaypoint.transform.position + offsetTo);
            }

            if (waypoint.nextWaypoint != null)
            {
                Gizmos.color = Color.green;
                Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
                Vector3 offsetTo = waypoint.nextWaypoint.transform.right * -waypoint.nextWaypoint.width / 2f;

                Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.nextWaypoint.transform.position + offsetTo);
            }
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
        public static void OnDrawSceneGizmo(WaypointIntersection waypoint, GizmoType gizmoType)
        {
            if ((gizmoType & GizmoType.Selected) != 0)
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = Color.blue * 0.5f;
            }

            Gizmos.DrawSphere(waypoint.transform.position, 1f);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2f), waypoint.transform.position + (-waypoint.transform.right * waypoint.width / 2f));

            // previus

            if (waypoint.prevWaypoint != null)
            {
                Gizmos.color = Color.red;
                Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
                Vector3 offsetTo = waypoint.prevWaypoint.transform.right * waypoint.prevWaypoint.width / 2f;

                Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.prevWaypoint.transform.position + offsetTo);
            }

            // straight

            if (waypoint.nextWaypoint != null)
            {
                Gizmos.color = Color.green;
                Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
                Vector3 offsetTo = waypoint.nextWaypoint.transform.right * -waypoint.nextWaypoint.width / 2f;

                Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.nextWaypoint.transform.position + offsetTo);
            }

            // left

            if (waypoint.leftWaypoint != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
                Vector3 offsetTo = waypoint.leftWaypoint.transform.right * -waypoint.leftWaypoint.width / 2f;

                Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.leftWaypoint.transform.position + offsetTo);
            }

            // right

            if (waypoint.rightWaypoint != null)
            {
                Gizmos.color = Color.magenta;
                Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
                Vector3 offsetTo = waypoint.rightWaypoint.transform.right * -waypoint.rightWaypoint.width / 2f;

                Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.rightWaypoint.transform.position + offsetTo);
            }
        }
    }
}
