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

        // This is a coroutine that will spawn cars at designated waypoints
        IEnumerator SpawnCars()
        {
            // Find all Waypoint objects in the scene and store them in an array
            Waypoint[] spawnPoints = FindObjectsOfType<Waypoint>();

            for (int i = 0; i < amountOfCars; i++)
            {
                int j = Random.Range(0, spawnPoints.Length);

                // Instantiate a new car at the selected spawn point, using a random prefab from the carPrefab array
                GameObject newCar = Instantiate(carPrefab[Random.Range(0, carPrefab.Length)], spawnPoints[j].transform.position, spawnPoints[j].transform.rotation);

                // Get the WaypointNavigator component from the newly spawned car and set its current waypoint to the spawn point
                newCar.GetComponent<WaypointNavigator>().currentWaypoint = spawnPoints[j];

                // Wait for the end of the current frame before spawning the next car
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

