using Unity.Entities;
using Unity.Mathematics;


public struct PlayerTag : IComponentData { }

public struct PlayerSettings : IComponentData
{
    public float MoveSpeed;
    public float RotateSpeed;
    public float BankingAmount;
}

public struct PlayerInput : IComponentData
{
    public float2 MoveInput;
    public float2 LookInput;
    public bool IsBoosting;
    public bool IsFiring;
}

public struct WeaponStats : IComponentData
{
    public Entity ProjectilePrefab;
    public float FireRate;
    public float ProjectileSpeed;
    public float CooldownTimer;
}

public struct MuzzleOffset : IBufferElementData
{
    public float3 LocalPosition;
}

public struct MainCameraTarget : IComponentData
{
    public float3 HitPosition;
    public bool HitSomething;
}