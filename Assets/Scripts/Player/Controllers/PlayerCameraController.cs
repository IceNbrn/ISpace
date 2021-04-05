using System;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private GameObject fpsCamera;
        [SerializeField] private Camera camera;
        [SerializeField] private AudioListener audioListener;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Vector3 offset;
        [SerializeField] private GameObject weaponModel;
        
        private Transform _cameraTransform;

        private NetworkIdentity _localPlayer;
        
        private float _rotationX;
        private float _rotationY;
        
        private PlayerInActions _controls;

        private void Awake()
        {
            LocalPlayerAnnouncer.OnLocalPlayerUpdated += OnLocalPlayerUpdated;
            SpacePlayer.OnPlayerStatusUpdated += OnPlayerStatusUpdated;
        }

        private void OnPlayerStatusUpdated(EPlayerStatus status)
        {
            switch (status)
            {
                case EPlayerStatus.ALIVE:
                    // Models
                    weaponModel.SetActive(true);
                    break;
                case EPlayerStatus.DEAD:
                    // Models
                    weaponModel.SetActive(false);
                    break;
                case EPlayerStatus.SPECTATING:
                    // Models
                    weaponModel.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        private void OnLocalPlayerUpdated(NetworkIdentity obj)
        {
            _localPlayer = obj;

            InitializeCamera();
        }

        private void OnEnable()
        {
            _controls = PlayerInputs.PlayerControls;
            _controls.Player.Look.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Look.Enable();
        }
        
        private void FixedUpdate()
        {
            /*
            transform.position = Vector3.SmoothDamp(transform.position, cameraPosition.position, ref _velocity, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, cameraPosition.rotation, Time.deltaTime * lerpSpeed);*/
            if (!camera.enabled)
                return;
            
            transform.position = _cameraTransform.position;
            transform.rotation = _cameraTransform.rotation;
        }

        private bool InitializeCamera()
        {
            if (_localPlayer != null)
            {
                camera.enabled = true;
                audioListener.enabled = true;
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                _cameraTransform = _localPlayer.GetComponent<SpacePlayer>().CameraTransform;
                
                return true;
            }

            return false;
        }
    }
}
