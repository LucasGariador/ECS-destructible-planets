using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float rotateSpeed = 5f;
    public float fireRate = 0.2f;
    public float projectileSpeed = 20f;
    public float cooldownTimer = 2f;
    public GameObject projectilePrefab;
    public float3 muzzleOffsetRight;
    public float3 muzzleOffsetLeft;

    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            DynamicBuffer<MuzzleOffset> buffer = AddBuffer<MuzzleOffset>(entity);

            // 1. Datos de Juego
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new PlayerInput());
            buffer.Add(new MuzzleOffset { LocalPosition = authoring.muzzleOffsetRight });
            buffer.Add(new MuzzleOffset { LocalPosition = authoring.muzzleOffsetLeft });
            AddComponent(entity, new WeaponStats
            {
                ProjectilePrefab = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                FireRate = 0.2f,
                ProjectileSpeed = 20f,
                CooldownTimer = 2f
            });
            AddComponent(entity, new PlayerSettings
            {
                MoveSpeed = authoring.moveSpeed,
                RotateSpeed = authoring.rotateSpeed
            });
        }
    }
}