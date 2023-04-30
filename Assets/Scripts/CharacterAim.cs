using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAim : MonoBehaviour
{
    private PlayerInput playerinput;
    public GameObject mainCamera;
    public GameObject aimCamera;

    public void onAim(InputAction.CallbackContext ctx)
    {
        if (mainCamera.activeInHierarchy)
        {
            mainCamera.SetActive(false);
            aimCamera.SetActive(true);
        }
        else
        {
            aimCamera.SetActive(false);
            mainCamera.SetActive(true);
        }
    }

    private void Awake()
    {
        playerinput = GetComponent<PlayerInput>();
    }
}
