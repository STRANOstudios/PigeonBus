using Sirenix.OdinInspector;
using TrafficSystem;
using UnityEngine;

public class VehicleWheelController : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private Transform[] _rearWheelMeshes;
    [SerializeField] private Transform[] _frontWheelMeshes;

    [Title("Settings")]
    [SerializeField, MinValue(0f), MaxValue(90f)] private float _maxTurnAngle;
    [SerializeField, MinValue(0f)] private float _wheelRadius = 0.35f;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [ShowIf("_debug"), ShowInInspector]private float speed;
    [SerializeField] private bool _showDrawGizmos;

    private AgentNavigationController agent;

    private void Awake()
    {
        agent = GetComponent<AgentNavigationController>();
    }

    private void Update()
    {
        speed = agent.Speed;
        RotateRearWheels();
        RotateFrontWheels();
    }

    private void RotateRearWheels()
    {
        float rotationAngle = speed * 360 * Time.deltaTime / (2 * Mathf.PI * _wheelRadius);
        foreach (var wheel in _rearWheelMeshes)
        {
            wheel.Rotate(rotationAngle, 0, 0);
        }
    }

    private void RotateFrontWheels()
    {
        Vector3 directionToTarget = agent.TargetPosition - transform.position;
        directionToTarget.y = 0;

        float angle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        float rotationAngleY = Mathf.Clamp(angle, -_maxTurnAngle, _maxTurnAngle);

        foreach (var wheel in _frontWheelMeshes)
        {
            wheel.localRotation = Quaternion.Euler(0, rotationAngleY, 0);
        }
    }

    private void OnDrawGizmos()
    {
        if(!_showDrawGizmos) return;

        if (_rearWheelMeshes.Length > 0)
        {
            Gizmos.color = Color.green;
            foreach (var wheel in _rearWheelMeshes)
            {
                Gizmos.DrawWireSphere(wheel.position, _wheelRadius);
            }
        }

        if (_frontWheelMeshes.Length > 0)
        {
            Gizmos.color = Color.red;
            foreach (var wheel in _frontWheelMeshes)
            {
                Gizmos.DrawWireSphere(wheel.position, _wheelRadius);
            }
        }
    }
}
