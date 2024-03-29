using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerMovementController : NetworkBehaviour
    {
        // Speeds
        [Header("Speeds Settings")]
        [SerializeField] private float _moveSpeed      = 8f;
        [SerializeField] private float _rollSpeed      = 80f;
        [SerializeField] private float _heightSpeed    = 8f;
        //[SerializeField] private float _drag           = 2f;
        [SerializeField] private float _dragColliding  = 10000f;
        [SerializeField] private float dragLerpTime = 0.8f;
        [SerializeField] private Vector3 mapCenter;
        [SerializeField] private float maxDistance = 5000.0f;
        
        private static float _mouseSensitivity;
        
        private float _rotationX, _rotationY, _rotationZ;
        
        private Rigidbody _rigidbody;
        private Vector3 _move = Vector3.zero;

        // Inputs
        private PlayerInActions _controls;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            if (!isLocalPlayer)
                return;
            
            _mouseSensitivity = GameManager.Singleton.PlayerSettings.Sensitivity * 0.1f;
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
        }

        
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;
            
            CmdLimitArea();
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
            
            // Reset the rotations, so it stops rotating
            _rotationX = 0f;
            _rotationY = 0f;
            _rotationZ = 0f;
        }

        private void Move()
        {
            // Gets the input from the player with the new input system
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
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isLocalPlayer)
                return;
            _rigidbody.angularDrag = _dragColliding;
        }

        private void OnCollisionExit(Collision other)
        {
            if (!isLocalPlayer)
                return;
            
            _rigidbody.angularVelocity -= Vector3.Lerp(_rigidbody.angularVelocity, Vector3.zero, dragLerpTime);
        }

        [Command]
        private void CmdLimitArea()
        {
            Debug.Log($"LimitArea - Object: {gameObject.name}");
            Vector3 playerPosition = transform.position;
            Vector3 distanceToCenter = playerPosition - mapCenter;
            if (distanceToCenter.sqrMagnitude > maxDistance)
            {
                Vector3 forceVector = distanceToCenter * (distanceToCenter.sqrMagnitude / 1000.0f);
                Debug.Log($"CMD Negative ForceVector {-forceVector}");
                RpcLimitArea(forceVector);
            }
        }
        
        [ClientRpc]
        private void RpcLimitArea(Vector3 forceVector)
        {
            if (!isLocalPlayer)
                return;
            Debug.Log($"RPC Negative ForceVector {-forceVector}");
            _rigidbody.AddForce(-forceVector, ForceMode.Force);
        }
        
        public static void SetSensitivity(float value) => _mouseSensitivity = value * 0.1f;
    }
}
