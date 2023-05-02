using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem<T> where T : ITargetable
{
    private List<T> targets = new List<T>();
    private T currentTarget;
    private List<GameObject> targetIndicators = new List<GameObject>();

    public void PopulateTargetsInRange(Vector3 playerPosition, float range)
    {
        Collider[] colliders = Physics.OverlapSphere(playerPosition, range);
        targets.Clear();

        foreach (Collider collider in colliders)
        {
            T target = collider.gameObject.GetComponent<T>();

            if (target != null)
            {
                targets.Add(target);
            }
        }
    }

    
    public void GenerateTargetIndicators(Material sphereMaterial, float sphereSize)
    {
        //remove all null from target list
        targets.RemoveAll(item => item == null);
        targetIndicators.RemoveAll(item => item == null);
        Debug.Log("Count "+targets.Count+ targets);
        // If there are not enough indicators, create new ones
        while (targetIndicators.Count < targets.Count)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Renderer>().material = sphereMaterial;
            sphere.transform.localScale = Vector3.one * sphereSize;
            sphere.tag = "target";
            targetIndicators.Add(sphere);
        }

        // Update position and parent of each indicator
        for (int i = 0; i < targets.Count; i++)
        {
            if(targets[i] != null){
                targetIndicators[i].transform.position = targets[i].GetPosition();
                targetIndicators[i].transform.parent = targets[i].GetTransform();
                targetIndicators[i].SetActive(true);
            }
        }

        // Hide excess indicators
        for (int i = targets.Count; i < targetIndicators.Count; i++)
        {
            if(targetIndicators[i] != null)
            targetIndicators[i].SetActive(false);
        }
    }

    public void HideTargetIndicators()
    {
        foreach (var indicator in targetIndicators)
        {
            indicator.SetActive(false);
        }
    }

    public T GetTarget(Vector3 position)
    {
        T closestTarget = default(T);
        float closestDistance = Mathf.Infinity;
        foreach (T target in targets)
        {
            float distance = Vector3.Distance(position, target.GetPosition());

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }
        currentTarget = closestTarget;
        return closestTarget;
    }

    public T GetCurrentTarget()
    {
        return currentTarget;
    }
}
