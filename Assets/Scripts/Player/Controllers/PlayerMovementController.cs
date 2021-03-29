using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerMovementController : NetworkBehaviour
    {
        // Speeds
        [Header("Speeds Settings")]
        [SerializeField] private float _moveSpeed      = 15f;
        [SerializeField] private float _rollSpeed      = 40f;
        [SerializeField] private float _heightSpeed    = 8f;
        //[SerializeField] private float _drag           = 2f;
        [SerializeField] private float _dragColliding  = 100f;
        
        private static float _mouseSensitivity;
        
        // UI
        [SerializeField] private GameObject _canvasUI;
        
        private float _rotationX, _rotationY, _rotationZ;
        
        private Rigidbody _rigidbody;
        private Vector3 _move = Vector3.zero;

        // Inputs
        private PlayerInActions _controls;

        private void Start()
        {
            if (!isLocalPlayer)
                return;

            _mouseSensitivity = GameManager.Singleton.GetPlayerSettings().Sensitivity * 0.1f;
            
            _canvasUI.SetActive(true);
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void OnEnable()
        {
            _controls = PlayerInputs.PlayerControls;
            _controls?.Player.Move.Enable();
            _controls?.Player.Height.Enable();
            _controls?.Player.Roll.Enable();
        }

        private void OnDisable()
        {
            if (!isLocalPlayer)
                return;
            
            _controls?.Player.Move.Disable();
            _controls?.Player.Height.Disable();
            _controls?.Player.Roll.Disable();
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;
            
            Rotate();
            Move();
            ControlHeight();
            //ControlVelocity();
        }

        
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;
            
            ControlVelocity();
        }

        private void Rotate()
        {
            Vector2 movementInput = _controls.Player.Look.ReadValue<Vector2>();
            float rollInput = _controls.Player.Roll.ReadValue<float>();

            movementInput.x *= _mouseSensitivity;
            movementInput.y *= _mouseSensitivity;
            rollInput *= _rollSpeed * Time.deltaTime;

            _rotationX -= movementInput.y;
            _rotationY += movementInput.x;
            _rotationZ += rollInput;
            
            
            // Roll
            transform.Rotate(0f, 0f, _rotationZ, Space.Self);
 
            // Pitch
            transform.Rotate(_rotationX, 0f, 0f, Space.Self);
 
            // Yaw
            transform.Rotate(0f, _rotationY, 0f, Space.Self);
            
            
            /*
            Vector3 roll = new Vector3(0.0f, 0.0f, _rotationZ);
            _rigidbody.rotation *= Quaternion.Euler(roll.x, roll.y, roll.z);
            
            Vector3 pitch = new Vector3(_rotationX, 0.0f, 0.0f);
            _rigidbody.rotation *= Quaternion.Euler(pitch.x, pitch.y, pitch.z);
            
            Vector3 yaw = new Vector3(0.0f, _rotationY, 0.0f);
            _rigidbody.rotation *= Quaternion.Euler(yaw.x, yaw.y, yaw.z);*/
            
            _rotationX = 0f;
            _rotationY = 0f;
            _rotationZ = 0f;
        }

        private void Move()
        {
            Vector2 movementInput = _controls.Player.Move.ReadValue<Vector2>();
            _move += transform.right * movementInput.x + transform.forward * movementInput.y;
            _move *= _moveSpeed * Time.fixedDeltaTime;
        }
        
        private void ControlHeight()
        {
            float movementInput = _controls.Player.Height.ReadValue<float>();
            if (Mathf.Abs(movementInput) > 0.0001f)
            {
                _move += transform.up * movementInput;
                _move *= _heightSpeed * Time.fixedDeltaTime;
            }
        }

        private void ControlVelocity()
        {
            _rigidbody.velocity += _move;
            /*
            _rigidbody.velocity += _move;
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _moveSpeed);*/
            //_rigidbody.velocity *= Mathf.Exp(-Time.deltaTime * _drag);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isLocalPlayer)
                return;
            _rigidbody.angularDrag = _dragColliding;
            //_rigidbody.freezeRotation = true;
        }

        private void OnCollisionExit(Collision other)
        {
            if (!isLocalPlayer)
                return;
            _rigidbody.angularDrag = 1f;
            //_rigidbody.freezeRotation = false;
        }

        public static void SetSensitivity(float value) => _mouseSensitivity = value;
    }
}
