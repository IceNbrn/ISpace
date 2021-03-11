using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Weapons;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : NetworkBehaviour
{
    // Weapon Stuff
    [SerializeField]
    private WeaponInfo weaponInfo;
    private int _bulletsAvailable;
    
    // Player
    [SerializeField] 
    private Camera fpsCamera;
    
    private Rigidbody _playerRigidbody;
    
    // Effects
    [SerializeField]
    private ParticleSystem muzzleFlash1, muzzleFlash2;
    [SerializeField] 
    private GameObject hitEffect;

    // Network
    [SerializeField]
    private NetworkIdentity networkIdentity;

    private float _nextFireTime;
    private bool _isShooting;
    private bool _isReloading;
    
    public void OnEnable()
    {
        PlayerInputs.PlayerControls.Player.Fire.Enable();
        PlayerInputs.PlayerControls.Player.Reload.Enable();
    }

    public void OnDisable()
    {
        if (!isLocalPlayer)
            return;
        PlayerInputs.PlayerControls.Player.Fire.Disable();
        PlayerInputs.PlayerControls.Player.Reload.Disable();
    }

    // Start is called before the first frame update
    public void Start()
    {
        if (!isLocalPlayer)
            return;
        _playerRigidbody = GetComponent<Rigidbody>();
        
        PlayerInputs.PlayerControls.Player.Fire.started += context => _isShooting = true;
        PlayerInputs.PlayerControls.Player.Fire.canceled += context => _isShooting = false;
        PlayerInputs.PlayerControls.Player.Reload.performed += CooloffAction;

        _bulletsAvailable = weaponInfo.Capacity;
    }

    private void CooloffAction(InputAction.CallbackContext obj)
    {
        if (!isLocalPlayer)
            return;
        CoolOffWeapon();
    }

    private void CoolOffWeapon()
    {
        // Make sure reloading is not happening already
        // So se don't reload more than 1 time
        if (!_isReloading)
        {
            StartCoroutine(CooloffCoroutine());
        }
    }

    private IEnumerator CooloffCoroutine()
    {
        _isReloading = true;
        yield return new WaitForSeconds(weaponInfo.CoolOffTime);
        _bulletsAvailable = weaponInfo.Capacity;
        _isReloading = false;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        
        // Semi-Auto Mode
        if (_isShooting && weaponInfo.FireRate == 0 && _bulletsAvailable > 0)
        {
            Shoot();
            _isShooting = false;
        }
        // Full-Auto Mode
        else if (_isShooting && Time.time >= _nextFireTime && _bulletsAvailable > 0)
        {
            _nextFireTime = Time.time + (1.0f / weaponInfo.FireRate);
            Shoot();
        }
        // No "bullets", time to reload
        else if (_bulletsAvailable <= 0)
        {
            CoolOffWeapon();
        }
    }

    [Client]
    private void Shoot()
    {
        _bulletsAvailable--;
        ApplyRecoil();
        
        CmdOnShoot();
        
        Transform camTransform = fpsCamera.transform;
        RaycastHit hit;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, weaponInfo.Range))
        {
            CmdOnHit(hit.point, hit.normal);
        }
    }

    private void ApplyRecoil()
    {
        Vector3 force = -transform.forward * weaponInfo.RecoilForce;
        _playerRigidbody.AddForce(force, ForceMode.Impulse);
    }

    [Command]
    private void CmdOnShoot()
    {
        RpcMuzzleFlash();
    }

    [Command]
    private void CmdOnHit(Vector3 position, Vector3 normal)
    {
        RpcHitEffect(position, normal);
    }

    // Send a hit effect to the other clients
    [ClientRpc]
    private void RpcHitEffect(Vector3 position, Vector3 normal)
    {
        GameObject gameObject = (GameObject) Instantiate(hitEffect, position, Quaternion.LookRotation(normal));
        Destroy(gameObject, 1.0f);
    }
    
    // Send a muzzle flash to the other clients
    [ClientRpc]
    private void RpcMuzzleFlash()
    {
        muzzleFlash1.Play();
        muzzleFlash2.Play();
    }
}