using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Vector2 size = new Vector2(1, 1);

    public Vector2 Size
    {
        get { return size; }
    }
}