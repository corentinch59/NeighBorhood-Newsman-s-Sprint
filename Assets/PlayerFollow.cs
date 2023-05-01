using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(90, player.rotation.eulerAngles.y, 0);
        transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
    }
}
