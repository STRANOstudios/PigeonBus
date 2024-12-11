using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Route : Singleton<Route>
{
    [Title("References")]
    [Tooltip("The route's color")]
    [SerializeField] private List<Material> _routes = new();

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

    /// <summary>
    /// Get a random route
    /// </summary>
    public Material GetRandomRoute()
    {
        return _routes[Random.Range(0, _routes.Count)];
    }

}