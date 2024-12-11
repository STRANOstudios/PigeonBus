using Sirenix.OdinInspector;
using UnityEngine;

public class BusStopInteraction : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required, Tooltip("The bus stop singal")] private GameObject busStopSingal;

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

    private void SetRout()
    {
        route = Route.Instance.GetRandomRoute();

        busStopSingal.GetComponent<MeshRenderer>().material = route;
    }
}
