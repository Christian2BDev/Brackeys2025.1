using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PlayerController : MonoBehaviour
    {
        [Header("Camera Settings")] [SerializeField]
        private float cameraFolowingSpeed;

        [SerializeField] private Vector3 offset = new Vector3(0, 13, -0.5f);
        private Vector3 _vel = Vector3.zero;
        private Camera _camera;

        [Header("Movement clamping")] [SerializeField]
        private float acceleration;

        [SerializeField] private float deceleration;
        [SerializeField] private Vector3 maxVelocity;
        [SerializeField] private Vector3 minVelocity;
        private Vector2 _moveInput;
        private Vector3 _currentVelocity;
        private Rigidbody _rb;

        [Header("Player rotation")] [SerializeField]
        private float rotationSpeed;

        [SerializeField] private LayerMask groundLayer;

        [Header("Dash Settings")] [SerializeField]
        private float dashForce;

        [SerializeField] private float dashCooldown;
        [SerializeField] private float dashDuration;
        private bool _canDash;
        private bool _isDashing;
        private float _dashTimeLeft;
        private Vector3 _dashDirection;
        private float _lastDashTime;


        [Header("Stats")] 
        public bool inSubmitArea;
        public GameObject submitArea;
        public List<GameObject> pickups = new List<GameObject>();
        [SerializeField] private GameObject holdingItem;
        private GameObject _lastSelected;

        private PlayerInput _playerInput;



        private void Start()
        {
            _camera = Camera.main;
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            MovePlayerAndCamera();
            LookTowardsMouse();
            DashCooldown();

            if (_isDashing)
            {
                ApplyDashForce();
            }
        }

        void MovePlayerAndCamera()
        {
            //player
            var targetVelocity = new Vector3(_moveInput.x, 0, _moveInput.y) * 10f;
            _currentVelocity =
                ClampVector3(
                    Vector3.Lerp(_currentVelocity, targetVelocity,
                        (targetVelocity.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime),
                    minVelocity, maxVelocity);
            _rb.linearVelocity = new Vector3(_currentVelocity.x, _rb.linearVelocity.y, _currentVelocity.z);

            //camera
            Vector3 targetPosition = new Vector3(transform.position.x, offset.y, transform.position.z + offset.z);
            _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, targetPosition, ref _vel,
                cameraFolowingSpeed);
        }

        void LookTowardsMouse()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 2;
            if (Physics.Raycast(_camera.ScreenPointToRay(mousePosition), out var hit, 999999, groundLayer))
            {
                Vector3 targetDirection = hit.point - transform.position;
                targetDirection.y = 0;
                float singleStep = rotationSpeed * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDirection);
            }
        }

        private Vector3 ClampVector3(Vector3 vector, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y),
                Mathf.Clamp(vector.z, min.z, max.z)
            );
        }

        void DashCooldown()
        {
            if (Time.time >= _lastDashTime + dashCooldown)
            {
                _canDash = true;
            }
        }

        private void OnSprint()
        {
            var dir = new Vector3(_moveInput.x, 0, _moveInput.y);
            if (_canDash)
            {
                _dashDirection = dir;
                _isDashing = true;
                _dashTimeLeft = dashDuration;
                _lastDashTime = Time.time;

                _canDash = false;
                Debug.Log("Dash");
            }
        }

        void ApplyDashForce()
        {
            var forceThisFrame = dashForce * Time.fixedDeltaTime / dashDuration;
            _rb.AddForce(_dashDirection * forceThisFrame, ForceMode.VelocityChange);

            _dashTimeLeft -= Time.fixedDeltaTime;
            if (_dashTimeLeft <= 0f)
            {
                _isDashing = false;
            }
        }

        private void OnMove(InputValue moveVector)
        {
            _moveInput = moveVector.Get<Vector2>();
        }

        private void OnInteract()
        {
            if (inSubmitArea)
            {
                Debug.Log("Submitting!");
                Submit();
            }
            else if (pickups.Count > 0 && holdingItem == null)
            {
                Debug.Log("Picking up!");
                PickUp();
            }
            else
            {
                Debug.Log("Nothing to interact with!");
            }
        }

        private void PickUp()
        {
            foreach (var item in pickups)
            {
                if (_lastSelected == null)
                {
                    _lastSelected = item;
                }
                else if (Vector3.Distance(gameObject.transform.position, item.transform.position) <
                         Vector3.Distance(gameObject.transform.position, _lastSelected.transform.position))
                {
                    _lastSelected = item;
                }
            }

            pickups.Remove(_lastSelected);
            holdingItem = _lastSelected;
            holdingItem.GetComponent<MeshCollider>().enabled = false;
            holdingItem.GetComponent<Part>().canvasE.SetActive(false);
            holdingItem.transform.parent = transform.GetChild(0);
            holdingItem.transform.localPosition = Vector3.zero;
        }

        private void Submit()
        {
            if (holdingItem == null)
            {
                Debug.Log("You're not holding anything!");
                return;
            }
            
            submitArea.GetComponent<GameController>().SubmitItem(holdingItem);
            OnDrop();
        }

        private void OnDrop()
        {
            if (holdingItem != null)
            {
                holdingItem.transform.parent = null;
                holdingItem.GetComponent<MeshCollider>().enabled = true;
                holdingItem.GetComponent<Part>().canvasE.SetActive(true);
                holdingItem = null;
               
            }
        }
        
        

    }
    
