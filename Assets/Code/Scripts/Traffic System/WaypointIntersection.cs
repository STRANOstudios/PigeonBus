namespace TrafficSystem
{
    public class WaypointIntersection : Waypoint
    {
        public Waypoint leftWaypoint;
        public Waypoint rightWaypoint;

        public WaypointType waypointType = WaypointType.Normal;
    }

    public enum WaypointType
    {
        Stop,
        Normal
    }
}
