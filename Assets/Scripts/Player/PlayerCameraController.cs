using System;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private float _mouseSensitivity = 8.0f;
        [SerializeField] private Transform _playerBody;
        [SerializeField] private Rigidbody _playerBodyRigidbody;
        [SerializeField] private GameObject _fpsCamera;
        private float _rotationX;
        private float _rotationY;
        private Quaternion _rollRotation;
        
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

        public Tuple<Quaternion, Quaternion> Rotate()
        {
            Vector2 movementInput = _controls.Player.Look.ReadValue<Vector2>();
            movementInput.x *= _mouseSensitivity * Time.deltaTime;
            movementInput.y *= _mouseSensitivity * Time.deltaTime;
            Vector3 euler = _playerBodyRigidbody.rotation.eulerAngles;

            _rotationX += movementInput.y;
            _rotationY += movementInput.x;
            
            Quaternion yaw = Quaternion.Euler(Vector3.up * _rotationY);
            Quaternion pitch = Quaternion.Euler(Vector3.left * _rotationX);

            return new Tuple<Quaternion, Quaternion>(yaw, pitch);
        }
    }
}
