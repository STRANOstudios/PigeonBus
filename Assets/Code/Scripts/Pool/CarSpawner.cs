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
        [SerializeField, Range(0f, 50f), Tooltip("Radius of the check sphere")] private float radius = 5f;

        [Title("Debug")]
        [SerializeField] private bool _debug = false;
        [SerializeField, ShowIf("_debug"), Required] private GameObject _spawnArea;

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
                int k = Random.Range(0, carPrefab.Length);

                Vector3 spawnPosition = spawnPoints[j].transform.position;
                Collider[] colliders = Physics.OverlapSphere(spawnPosition, radius);

                if (colliders.Length == 0)
                {
                    GameObject newCar = Instantiate(carPrefab[k], spawnPosition, Quaternion.LookRotation(spawnPoints[j].transform.forward * -1));
                    newCar.GetComponent<WaypointNavigator>().currentWaypoint = spawnPoints[j];
                }

                if (_debug)
                {
                    GameObject spawnArea = Instantiate(_spawnArea, spawnPosition, spawnPoints[j].transform.rotation);
                    spawnArea.transform.localScale = radius * Vector3.one;
                    Destroy(spawnArea, 1f);
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}

