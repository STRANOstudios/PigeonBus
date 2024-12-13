using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Route : Singleton<Route>
{
    [Title("References")]
    [SerializeField, Tooltip("The bus livery's color")] private List<Material> _routes = new();
    [SerializeField, Tooltip("The bus stop's color")] private List<Material> _routesBusStop = new();
    [SerializeField] List<GameObject> _busStop = new();
    [SerializeField, ProgressBar(0, 100, ColorGetter = "GetHealthBarColor"), Tooltip("The chance of a route pack being selected")] private int _difficutly = 50;

    private RoutePack _selectedRoutePack;

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
        _selectedRoutePack = GetRandomRoute();

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
        int randomValue = Random.Range(0, 100);
        if (randomValue < _difficutly && _selectedRoutePack != null)
        {
            return _selectedRoutePack;
        }
        else
        {
            int i = Random.Range(0, _routes.Count);
            return new RoutePack(_routes[i], i);
        }
    }

    private Color GetHealthBarColor(float value)
    {
        Color orange = new Color(247 / 255f, 91 / 255f, 2 / 255f);

        if (value <= 50f)
        {
            return Color.Lerp(Color.red, orange, value / 50f);
        }
        else
        {
            return Color.Lerp(orange, Color.green, (value - 50f) / 50f);
        }
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