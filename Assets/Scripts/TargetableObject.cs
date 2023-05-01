using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableObject : MonoBehaviour, ITargetable
{
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
