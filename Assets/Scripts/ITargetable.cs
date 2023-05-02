using UnityEngine;

public interface ITargetable
{
    Vector3 GetPosition();
    Transform GetTransform();
    GameObject GetGameObject(); // new method
}
