using UnityEngine;

namespace TrafficSystem
{
    public class Sensor : MonoBehaviour
    {
        [Range(0f, 50f)] public float RaycastDistance = 10f;
        public Color GizmosColor = Color.red;
    }
}

