using UnityEngine;
using UnityEngine.InputSystem;

public class FlyCamera : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f;
    public float boostMultiplier = 3f;
    public float mouseSensitivity = 0.5f;
    public float rotationSpeed = 10f;

    [Header("Limits")]
    public float maxVerticalAngle = 85f;

    private Vector2 _rotation;

    private void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rotation = new Vector2(transform.eulerAngles.y, 0);
    }

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        HandleRotation();
        HandleMovement();
        HandleCursorToggle();
    }

    private void HandleRotation()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        _rotation.x += mouseDelta.x * mouseSensitivity;
        _rotation.y -= mouseDelta.y * mouseSensitivity;

        _rotation.y = Mathf.Clamp(_rotation.y, -maxVerticalAngle, maxVerticalAngle);

        transform.localRotation = Quaternion.Euler(_rotation.y, _rotation.x, 0f);
    }

    private void HandleMovement()
    {
        float speed = moveSpeed;

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            speed *= boostMultiplier;
        }

        Vector3 direction = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) direction += transform.forward;
        if (Keyboard.current.sKey.isPressed) direction -= transform.forward;
        if (Keyboard.current.dKey.isPressed) direction += transform.right;
        if (Keyboard.current.aKey.isPressed) direction -= transform.right;

        if (Keyboard.current.eKey.isPressed) direction += Vector3.up;
        if (Keyboard.current.qKey.isPressed) direction -= Vector3.up;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
        }

        transform.position += direction * speed * Time.deltaTime;
    }


    private void HandleCursorToggle()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool isLocked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !isLocked;
        }
    }
}