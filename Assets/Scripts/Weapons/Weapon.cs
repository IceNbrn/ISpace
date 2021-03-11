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

public class Weapon : NetworkBehaviour
{
    // Weapon Stuff
    [SerializeField]
    private WeaponInfo weaponInfo;
    private int _bulletsAvailable;
    
    // Camera
    [SerializeField] 
    private Camera fpsCamera;
    
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
    
    // DEBUG
    private Stopwatch _stopwatch = new Stopwatch();

    public void OnEnable()
    {
        PlayerInputs.PlayerControls.Player.Fire.Enable();
        PlayerInputs.PlayerControls.Player.Reload.Enable();
    }

    public void OnDisable()
    {
        PlayerInputs.PlayerControls.Player.Fire.Disable();
        PlayerInputs.PlayerControls.Player.Reload.Disable();
    }

    // Start is called before the first frame update
    public void Start()
    {
        PlayerInputs.PlayerControls.Player.Fire.started += context => _isShooting = true;
        PlayerInputs.PlayerControls.Player.Fire.canceled += context => _isShooting = false;
        PlayerInputs.PlayerControls.Player.Reload.performed += CooloffAction;

        _bulletsAvailable = weaponInfo.Capacity;
    }

    private void CooloffAction(InputAction.CallbackContext obj)
    {
        CoolOffWeapon();
    }

    private void CoolOffWeapon()
    {
        // Make sure reloading is not happening already
        // So se don't reload more than 1 time
        if (!_isReloading)
        {
            _stopwatch.Stop();
            Debug.Log($"Time to empty: {_stopwatch.ElapsedMilliseconds * 0.001} seconds");
            _stopwatch.Reset();
            StartCoroutine(CooloffCoroutine());
        }
    }

    private IEnumerator CooloffCoroutine()
    {
        Debug.Log("Reloading");

        _isReloading = true;
        yield return new WaitForSeconds(weaponInfo.CoolOffTime);
        _bulletsAvailable = weaponInfo.Capacity;
        _isReloading = false;
    }

    private void Update()
    {
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
            
            _stopwatch.Start();
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
        if (!networkIdentity.isLocalPlayer) 
            return;

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
        float rotation = weaponInfo.RecoilCurve.Evaluate(0.2f);
        transform.Rotate(rotation, 0f, 0f, Space.Self);
    }

    [Command]
    private void CmdOnShoot()
    {
        _bulletsAvailable--;
        Debug.Log($"Bullets: {_bulletsAvailable}");
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
        Destroy(gameObject, 1.5f);
    }
    
    // Send a muzzle flash to the other clients
    [ClientRpc]
    private void RpcMuzzleFlash()
    {
        muzzleFlash1.Play();
        muzzleFlash2.Play();
    }
}