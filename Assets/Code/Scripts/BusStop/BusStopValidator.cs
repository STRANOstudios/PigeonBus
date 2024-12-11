using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class BusStopValidator : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required, Tooltip("The display of the bus stop")] private GameObject display;

    [Title("Settings")]
    [SerializeField, MinValue(1), Tooltip("The reward for reaching correct the bus stop")] private int reward = 1;
    [SerializeField, MinValue(0), Tooltip("The reward for reaching wrong the bus stop")] private int penalty = 1;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [ShowIf("_debug"), ShowInInspector] private Material route;

    public static event Action<int> OnBusStopReached;

    private void Start()
    {
        SetRout();
    }

    private void OnEnable()
    {
        GameManager.Reset += SetRout;
    }

    private void OnDisable()
    {
        GameManager.Reset -= SetRout;
    }

    private void SetRout()
    {
        route = Route.Instance.GetRandomRoute();

        display.GetComponent<MeshRenderer>().material = route;
    }

    public void CheckRoute(MeshRenderer route)
    {
        bool isCorrectRoute = this.route == route.sharedMaterial;

        if (_debug)
            Debug.Log(isCorrectRoute ? "Correct route" : $"Wrong route: {this.route} != {route.material}");

        OnBusStopReached?.Invoke(isCorrectRoute ? reward : -penalty);
    }
}
