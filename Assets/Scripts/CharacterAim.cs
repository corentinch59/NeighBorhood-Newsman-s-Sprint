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
    public Animator anim;

    public void onAim(InputAction.CallbackContext ctx)
    {
        if (mainCamera.activeInHierarchy)
        {
            mainCamera.SetActive(false);
            aimCamera.SetActive(true);
            anim.SetBool("Throw", true);
            Time.timeScale = 0.5f;

        }
        else
        {
            aimCamera.SetActive(false);
            mainCamera.SetActive(true);
            anim.SetBool("Throw", false);
            anim.speed = 5;
            Time.timeScale = 1;
        }
    }

    private void Awake()
    {
        playerinput = GetComponent<PlayerInput>();
    }
}
