using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace TrafficSystem
{
    public class CarSpawner : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField] private GameObject[] carPrefab;
        [SerializeField, MinValue(0f)] private int amountOfCars = 10;

        private void Start()
        {
            StartCoroutine(SpawnCars());
        }

        IEnumerator SpawnCars()
        {
            Waypoint[] spawnPoints = FindObjectsOfType<Waypoint>();

            for (int i = 0; i < amountOfCars; i++)
            {
                int j = Random.Range(0, spawnPoints.Length);

                GameObject newCar = Instantiate(carPrefab[Random.Range(0, carPrefab.Length)], spawnPoints[j].transform.position, spawnPoints[j].transform.rotation);
                newCar.GetComponent<WaypointNavigator>().currentWaypoint = spawnPoints[j];
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

