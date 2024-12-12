using Sirenix.OdinInspector;
using UnityEngine;

public class LCurve : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private Transform _pointC;
    [SerializeField] private Transform _pointD;

    public void GetObject(TriggerCustomHandler other)
    {

    }
}
