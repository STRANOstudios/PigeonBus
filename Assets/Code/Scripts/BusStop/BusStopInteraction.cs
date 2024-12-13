using Sirenix.OdinInspector;
using UnityEngine;

public class BusStopInteraction : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required, Tooltip("The bus stop signal")] private GameObject busStopSingal;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [ShowIf("_debug"), ShowInInspector] private Material route;
    [ShowIf("_debug")] public int routeID;

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

    /// <summary>
    /// Sets the route to a random one.
    /// </summary>
    private void SetRout()
    {
        RoutePack pack = Route.Instance.GetRandomRoute();

        route = pack.route;
        routeID = pack.number;

        busStopSingal.GetComponent<MeshRenderer>().material = route;
    }
}