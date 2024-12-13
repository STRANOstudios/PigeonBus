using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Route : Singleton<Route>
{
    [Title("References")]
    [SerializeField, Tooltip("The bus livery's color")] private List<Material> _routes = new();
    [SerializeField, Tooltip("The bus stop's color")] private List<Material> _routesBusStop = new();
    [SerializeField] List<GameObject> _busStop = new();

    private void OnValidate()
    {
        for (int i = 0; i < _routes.Count; i++)
        {
            for (int j = i + 1; j < _routes.Count; j++)
            {
                if (_routes[i] == _routes[j])
                {
                    _routes.RemoveAt(j);
                    j--;
                }
            }
        }
    }

    private void Awake()
    {
        for (int i = 0; i < _busStop.Count; i++)
        {
            RoutePack routePack = GetRandomRoute();

            Meshreference mesh = _busStop[i].GetComponent<Meshreference>();

            mesh.mesh.material = _routesBusStop[routePack.number];
            mesh.numberID = routePack.number;
        }
    }

    /// <summary>
    /// Get a random route
    /// </summary>
    public RoutePack GetRandomRoute()
    {
        int i = Random.Range(0, _routes.Count);
        return new RoutePack(_routes[i], i);
    }
}

public class RoutePack
{
    public Material route;
    public int number;

    public RoutePack(Material route, int number)
    {
        this.route = route;
        this.number = number;
    }
}