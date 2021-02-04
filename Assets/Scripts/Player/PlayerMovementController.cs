using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        // Speeds
        [SerializeField] private float _moveSpeed      = 5f;
        [SerializeField] private float _rollSpeed      = 2f;
        [SerializeField] private float _heightSpeed    = 2f;
        [SerializeField] private float _drag           = 2f;
        
        [SerializeField] private float _mouseSensitivity = 5.0f;
        
        private float _currentSpeed;
        
        private float _rotationX, _rotationY, _rotationZ;
        
        private Rigidbody _rigidbody;
        private Vector3 _move = Vector3.zero;
        
        // Inputs
        private PlayerInActions _controls;

        private void Start()
        {
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
            _controls?.Player.Move.Disable();
            _controls?.Player.Height.Disable();
            _controls?.Player.Roll.Disable();
        }

        private void FixedUpdate()
        {
            Rotate();
            Move();
            ControlHeight();
            ControlVelocity();
        }

        private void Rotate()
        {
            Vector2 movementInput = _controls.Player.Look.ReadValue<Vector2>();
            float rollInput = _controls.Player.Roll.ReadValue<float>();
            Vector3 euler = _rigidbody.rotation.eulerAngles;
            
            
            movementInput.x *= _mouseSensitivity * Time.deltaTime;
            movementInput.y *= _mouseSensitivity * Time.deltaTime;
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

            _rotationX = 0;
            _rotationY = 0;
            _rotationZ = 0;
        }

        private void Move()
        {
            Vector2 movementInput = _controls.Player.Move.ReadValue<Vector2>();
            _move += transform.right * movementInput.x + transform.forward * movementInput.y;
            _move *= _moveSpeed * Time.deltaTime;
        }
        
        private void ControlHeight()
        {
            float movementInput = _controls.Player.Height.ReadValue<float>();
            if (Mathf.Abs(movementInput) > 0.0001f)
            {
                _move += transform.up * movementInput;
                _move *= _heightSpeed * Time.deltaTime;
            }
        }

        private void ControlVelocity()
        {
            _rigidbody.velocity += _move;
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _moveSpeed);
            //_rigidbody.velocity *= Mathf.Exp(-Time.deltaTime * _drag);
        }

        private void OnCollisionEnter(Collision other)
        {
            _rigidbody.freezeRotation = true;
        }

        private void OnCollisionExit(Collision other)
        {
            _rigidbody.freezeRotation = false;
        }
    }
}
