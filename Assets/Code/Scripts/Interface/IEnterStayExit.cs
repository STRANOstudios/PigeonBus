using UnityEngine;

public interface IEnterStayExit
{
    public Collider ColliderEntry { get; set; }
    public Collider ColliderStay { get; set; }
    public Collider ColliderExit { get; set; }
}
