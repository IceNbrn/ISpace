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
    private Camera fpsCamera;
    
    [SerializeField]
    private WeaponInfo weaponInfo;
    
    [SerializeField]
    private ParticleSystem muzzleFlash1, muzzleFlash2;

    [SerializeField] 
    private GameObject hitEffect;

    [SerializeField]
    private NetworkIdentity networkIdentity;

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
        Shoot();
    }

    [Client]
    private void Shoot()
    {
        if (!networkIdentity.isLocalPlayer) 
            return;
        
        CmdOnShoot();
        
        Transform camTransform = fpsCamera.transform;
        RaycastHit hit;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, weaponInfo.Range))
        {
            Debug.Log($"HIT: {hit.transform.name}");

            CmdOnHit(hit.point, hit.normal);
        } 
    }

    [Command]
    private void CmdOnHit(Vector3 position, Vector3 normal)
    {
        RpcHitEffect(position, normal);
    }

    [ClientRpc]
    private void RpcHitEffect(Vector3 position, Vector3 normal)
    {
        GameObject gameObject = (GameObject) Instantiate(hitEffect, position, Quaternion.LookRotation(normal));
        Destroy(gameObject, 1.5f);
    }

    [Command]
    private void CmdOnShoot()
    {
        RpcMuzzleFlash();
    }

    [ClientRpc]
    private void RpcMuzzleFlash()
    {
        muzzleFlash1.Play();
        muzzleFlash2.Play();
    }
}