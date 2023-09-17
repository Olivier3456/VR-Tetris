using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference teleport;
    [SerializeField] private InputActionReference pause;
    [Space(10)]
    [SerializeField] private TeleportManager teleportManager;

    


    private void OnEnable()
    {
        teleport.action.Enable();
        teleport.action.performed += teleportManager.Teleport;

        pause.action.Enable();
        pause.action.performed += GameManager.instance.TogglePlayPause;
    }
    private void OnDisable()
    {
        teleport.action.Disable();
        teleport.action.performed -= teleportManager.Teleport;

        pause.action.Disable();
        pause.action.performed -= GameManager.instance.TogglePlayPause;
    }





}
