using UnityEditor;
using UnityEngine;

namespace TrafficSystem
{
    [InitializeOnLoad()]
    public class WaypointEditor
    {
        // Colors used for selected and unselected waypoints
        private static Color _selectedColor = Color.yellow;
        private static Color _unselectedColor = Color.yellow * 0.5f;
        private static Color _blueSelectedColor = Color.blue;
        private static Color _blueUnselectedColor = Color.blue * 0.5f;

        /// <summary>
        /// Draws a gizmo for a waypoint in the scene view.
        /// </summary>
        /// <param name="waypoint">The waypoint to draw.</param>
        /// <param name="gizmoType">The type of gizmo to draw.</param>
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
        public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
        {
            // Draw the waypoint with the default colors
            DrawWaypoint(waypoint, gizmoType, _selectedColor, _unselectedColor);
        }

        /// <summary>
        /// Draws a gizmo for a waypoint intersection in the scene view.
        /// </summary>
        /// <param name="waypoint">The waypoint intersection to draw.</param>
        /// <param name="gizmoType">The type of gizmo to draw.</param>
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
        public static void OnDrawSceneGizmo(WaypointIntersection waypoint, GizmoType gizmoType)
        {
            // Draw the waypoint intersection with blue colors
            DrawWaypoint(waypoint, gizmoType, _blueSelectedColor, _blueUnselectedColor);
        }

        /// <summary>
        /// Draws a waypoint and its connections in the scene view.
        /// </summary>
        /// <param name="waypoint">The waypoint to draw.</param>
        /// <param name="gizmoType">The type of gizmo to draw.</param>
        /// <param name="selectedColor">The color to use for the waypoint when it is selected.</param>
        /// <param name="unselectedColor">The color to use for the waypoint when it is not selected.</param>
        private static void DrawWaypoint(Waypoint waypoint, GizmoType gizmoType, Color selectedColor, Color unselectedColor)
        {
            // Set the color of the waypoint based on its selection state
            Gizmos.color = (gizmoType & GizmoType.Selected) != 0 ? selectedColor : unselectedColor;
            Gizmos.DrawSphere(waypoint.transform.position, 1f);

            // Draw a line to represent the width of the waypoint
            Gizmos.color = Color.white;
            Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2f), waypoint.transform.position + (-waypoint.transform.right * waypoint.width / 2f));

            // Draw connections to the previous and next waypoints
            DrawConnection(waypoint.prevWaypoint, waypoint, Color.red);
            DrawConnection(waypoint.nextWaypoint, waypoint, Color.green);

            // If this is a waypoint intersection, draw connections to the left and right waypoints
            if (waypoint is WaypointIntersection intersection)
            {
                DrawConnection(intersection.leftWaypoint, intersection, Color.cyan);
                DrawConnection(intersection.rightWaypoint, intersection, Color.magenta);
            }
        }

        /// <summary>
        /// Draws a connection between two waypoints in the scene view.
        /// </summary>
        /// <param name="target">The target waypoint.</param>
        /// <param name="source">The source waypoint.</param>
        /// <param name="color">The color to use for the connection.</param>
        private static void DrawConnection(Waypoint target, Waypoint source, Color color)
        {
            // If the target waypoint is not null, draw a line to connect the two waypoints
            if (target != null)
            {
                Gizmos.color = color;
                Vector3 offset = source.transform.right * -source.width / 2f;
                Vector3 offsetTo = target.transform.right * -target.width / 2f;

                Gizmos.DrawLine(source.transform.position + offset, target.transform.position + offsetTo);
            }
        }
    }
}