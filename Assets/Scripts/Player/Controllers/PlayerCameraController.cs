using System;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private GameObject _fpsCamera;
        [SerializeField] private Transform _cameraPosition;
        
        private float _rotationX;
        private float _rotationY;
        
        private PlayerInActions _controls;

        private void Start()
        {
            _fpsCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

        private void LateUpdate()
        {
            transform.position = _cameraPosition.position;
            transform.rotation = _cameraPosition.rotation;
        }
    }
}
