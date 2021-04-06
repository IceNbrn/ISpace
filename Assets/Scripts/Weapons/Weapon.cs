using System.Collections;
using Mirror;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Weapon : NetworkBehaviour
    {
        // Weapon Stuff
        [SerializeField]
        private WeaponInfo weaponInfo;
    
        [SerializeField]
        private WeaponUI weaponUI;
    
        private int _bulletsAvailable;
        private int _magazineAvailable;

        // Player
        [SerializeField] 
        private string playerUIName = "PlayerHUD";
        [SerializeField] 
        private Camera fpsCamera;
        private Rigidbody _playerRigidbody;
    
        // Effects
        [SerializeField]
        private ParticleSystem muzzleFlash1, muzzleFlash2;
        [SerializeField] 
        private GameObject hitEffect;

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
            _magazineAvailable = weaponInfo.MagazineCapacity;

            //LoadWeaponUI();
            weaponUI.UpdateAmmo(_magazineAvailable, _bulletsAvailable);
        }

        private void LoadWeaponUI()
        {
            if (weaponUI == null)
            {
                GameObject playerHud = GameObject.Find(playerUIName);
                weaponUI = playerHud.GetComponentInChildren<WeaponUI>();
            }
        }

        private void Update()
        {
            if (!isLocalPlayer || _isReloading)
                return;

            // Semi-Auto Mode
            else if (_isShooting && weaponInfo.FireRate == 0 && _magazineAvailable > 0)
            {
                Shoot();
                _isShooting = false;
            }
            // Full-Auto Mode
            else if (_isShooting && Time.time >= _nextFireTime && _magazineAvailable > 0)
            {
                _nextFireTime = Time.time + (1.0f / weaponInfo.FireRate);
                Shoot();
            }
            // No "bullets", time to reload
            else if (_magazineAvailable <= 0 && _bulletsAvailable > 0)
            {
                CoolOffWeapon();
            }
        }

        private void CooloffAction(InputAction.CallbackContext obj)
        {
            if (_bulletsAvailable > 0)
            {
                Debug.Log("Reload");
                CoolOffWeapon();
            }
        }

        private void CoolOffWeapon()
        {
            // Make sure reloading is not happening already
            // So se don't reload more than 1 time
            if (!_isReloading)
            {
                Debug.Log("Reloading...");
                StartCoroutine(CooloffCoroutine());
            }
        }

        private IEnumerator CooloffCoroutine()
        {
            _isReloading = true;
            yield return new WaitForSeconds(weaponInfo.CoolOffTime);

            int weaponMagazine = weaponInfo.MagazineCapacity;
            int needBullets = weaponMagazine - _magazineAvailable;

            _magazineAvailable += needBullets;
            _bulletsAvailable -= needBullets;
        
            weaponUI.UpdateAmmo(_magazineAvailable, _bulletsAvailable);
            _isReloading = false;
        }
    
        [Client]
        private void Shoot()
        {
            _magazineAvailable--;
            ApplyRecoil();
            CmdOnShoot();
        
            Transform camTransform = fpsCamera.transform;
            RaycastHit hit;
            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, weaponInfo.Range))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    CmdPlayerShoot(hit.collider.name, weaponInfo.Damage, transform.name);
                }
                CmdOnHit(hit.point, hit.normal);
            }
            weaponUI.UpdateAmmo(_magazineAvailable, _bulletsAvailable);
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
    
        [Command]
        private void CmdPlayerShoot(string playerId, int damage, string fromPlayer)
        {
            Debug.Log($"{playerId} has been shot");

            SpacePlayer player = GameManager.GetPlayer(playerId);
            if (damage >= 0.0f && damage <= weaponInfo.Damage)
            {
                player.TakeDamage(damage, fromPlayer, weaponInfo.name);
                /*
                if (!hasAuthority && !isServer)
                    player.TakeDamage(damage, fromPlayer);
                else
                    player.RpcTakeDamage(damage, fromPlayer);*/
            }
        }

        // Send a hit effect to the other clients
        [ClientRpc]
        private void RpcHitEffect(Vector3 position, Vector3 normal)
        {
            GameObject instantiatedObject = Instantiate(hitEffect, position, Quaternion.LookRotation(normal));
            Destroy(instantiatedObject, 1.0f);
        }
    
        // Send a muzzle flash to the other clients
        [ClientRpc]
        private void RpcMuzzleFlash()
        {
            muzzleFlash1.Play();
            muzzleFlash2.Play();
        }
    }
}