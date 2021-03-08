using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

public class Weapon : NetworkBehaviour
{
    [SerializeField]
    private WeaponInfo weaponInfo;

    [SerializeField] 
    private Camera fpsCamera;

    public void OnEnable()
    {
        PlayerInputs.PlayerControls.Player.Fire.Enable();
    }

    public void OnDisable()
    {
        PlayerInputs.PlayerControls.Player.Fire.Disable();
    }

    // Start is called before the first frame update
    public void Start()
    {
        PlayerInputs.PlayerControls.Player.Fire.started += ShootAction;    
    }

    private void ShootAction(InputAction.CallbackContext obj)
    {
        //weaponInfo.MuzzleFlash.Play();
        
        Transform camTransform = fpsCamera.transform;
        RaycastHit hit;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, weaponInfo.Range))
        {
            Debug.Log($"HIT: {hit.transform.name}");
        }
    }
}