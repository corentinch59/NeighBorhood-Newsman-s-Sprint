using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem<T> where T : ITargetable
{
    private List<T> targets = new List<T>();
    private T currentTarget;

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

    public T GetClosestTarget(Camera camera, float maxDistance, float radius)
    {
        T closestTarget = default(T);
        float closestDistance = Mathf.Infinity;

        Vector3 centerOfScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = camera.ScreenPointToRay(centerOfScreen);

        foreach (T target in targets)
        {
            float distance = Vector3.Distance(ray.origin, target.GetPosition());

            if (distance > maxDistance || distance > radius)
            {
                continue;
            }

            float distanceToScreenCenter = Vector3.Distance(centerOfScreen, camera.WorldToScreenPoint(target.GetPosition()));

            if (distanceToScreenCenter < closestDistance)
            {
                closestDistance = distanceToScreenCenter;
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
