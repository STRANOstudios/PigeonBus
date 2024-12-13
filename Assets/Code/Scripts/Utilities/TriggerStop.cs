using Sirenix.OdinInspector;
using System;
using TrafficSystem;
using UnityEngine;
using UnityEngine.Events;

public class TriggerStop : TriggerVisualizer
{
    [Title("Settings")]
    [SerializeField, MinValue(0f), PropertyOrder(-3)] private float _waitTime;
    [SerializeField, PropertyOrder(-2)] private LayerMask _triggerLayer;
    [SerializeField, PropertyOrder(-1), Tooltip("Event triggered when the player exits the trigger zone.")] private TriggerEvent _onTriggerExit = new();

    // Serializable class for trigger event
    [Serializable] public class TriggerEvent : UnityEvent { }

    private void Start() { }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggerLayer == (_triggerLayer | (1 << other.gameObject.layer)))
        {
            other.GetComponent<AgentNavigationController>().Wait(_waitTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggerLayer == (_triggerLayer | (1 << other.gameObject.layer)))
        {
            _onTriggerExit.Invoke();
        }
    }
}
