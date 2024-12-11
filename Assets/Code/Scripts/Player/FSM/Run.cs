using UnityEngine;

public class Run : IFinalState
{
    private Transform transform;
    private float speed = 10f;

    public Run(PlayerController pc)
    {
        this.transform = pc.transform;
        speed = pc.Speed;
    }

    public void Enter() { }

    public void Exit() { }

    public void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
    }
}
