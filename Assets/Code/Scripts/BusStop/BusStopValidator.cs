using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class BusStopValidator : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required, Tooltip("The display of the bus stop")] private MeshRenderer display;

    [Title("Settings")]
    [SerializeField, MinValue(1), Tooltip("The reward for reaching correct the bus stop")] private int reward = 1;
    [SerializeField, MinValue(0), Tooltip("The reward for reaching wrong the bus stop")] private int penalty = 1;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [ShowIf("_debug"), ShowInInspector] private Material route;
    [ShowIf("_debug")] public int routeID;

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
        RoutePack pack = Route.Instance.GetRandomRoute();

        route = pack.route;
        routeID = pack.number;

        display.GetComponent<MeshRenderer>().material = route;
    }

    /// <summary>
    /// Checks if the bus stop is correct
    /// </summary>
    /// <param name="routeID"></param>
    public void CheckRoute(int routeID)
    {
        bool isCorrectRoute = this.routeID == routeID;

        if (_debug)
            Debug.Log(isCorrectRoute ? "Correct route" : $"Wrong route: {this.routeID} != {routeID}");

        OnBusStopReached?.Invoke(isCorrectRoute ? reward : -penalty);
    }
}
