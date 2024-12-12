using Sirenix.OdinInspector;
using UnityEngine;

public class BusStopInteraction : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required, Tooltip("The bus stop signal")] private GameObject busStopSingal;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [ShowIf("_debug"), ShowInInspector] private Material route;

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
        route = Route.Instance.GetRandomRoute();
        busStopSingal.GetComponent<MeshRenderer>().material = route;
    }
}