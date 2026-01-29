using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public partial class PlayerInputSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void OnUpdate()
    {
        float2 moveDir = float2.zero;
        float2 lookDir = float2.zero;
        bool isBoosting = false;
        bool isFiring = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveDir.y += 1f;
            if (Keyboard.current.sKey.isPressed) moveDir.y -= 1f;
            if (Keyboard.current.aKey.isPressed) moveDir.x -= 1f;
            if (Keyboard.current.dKey.isPressed) moveDir.x += 1f;
            if (Keyboard.current.leftShiftKey.isPressed) isBoosting = true;
        }

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            lookDir = new float2(delta.x, delta.y);

            if(Mouse.current.leftButton.isPressed) isFiring = true;
        }

        foreach (var input in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<PlayerTag>())
        {
            input.ValueRW.MoveInput = moveDir;
            input.ValueRW.LookInput = lookDir;
            input.ValueRW.IsBoosting = isBoosting;
            input.ValueRW.IsFiring = isFiring;
        }
    }
}