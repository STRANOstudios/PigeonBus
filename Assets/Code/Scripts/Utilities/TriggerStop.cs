using Sirenix.OdinInspector;
using TrafficSystem;
using UnityEngine;

public class TriggerStop : TriggerVisualizer
{
    [Title("References")]
    [SerializeField] private AgentNavigationController agent;
    [SerializeField] private BusStopValidator validator;

    [Title("Settings")]
    [SerializeField, MinValue(0f), PropertyOrder(-3)] private float _waitTime;
    [SerializeField, PropertyOrder(-2)] private LayerMask _triggerLayer;

    private void Start() { }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggerLayer == (_triggerLayer | (1 << other.gameObject.layer)))
        {
            agent.Wait(_waitTime);
            validator.CheckRoute(other.GetComponentInParent<Meshreference>().numberID);
        }
    }
}
