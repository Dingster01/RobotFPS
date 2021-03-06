﻿using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject scoreboard;

    private PlayerController controller;


    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }

    private void Start()
    {
        PauseMenu.IsOn = false;
    }


    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }


    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
    }


}
