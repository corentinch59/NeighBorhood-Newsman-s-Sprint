using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableObject : MonoBehaviour, ITargetable
{
    private void Awake()
    {
        if (Random.Range(0f, 1f) < 0.8f)
        {
            gameObject.SetActive(false);
        }
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public GameObject GetGameObject() // new method
    {
        return gameObject;
    }
}