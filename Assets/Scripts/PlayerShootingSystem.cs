using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerShootingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        if (!SystemAPI.HasSingleton<MainCameraTarget>()) return;
        float3 targetAimPoint = SystemAPI.GetSingleton<MainCameraTarget>().HitPosition;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (input, weapon, transform, muzzles) in
                 SystemAPI.Query<RefRO<PlayerInput>, RefRW<WeaponStats>, RefRO<LocalTransform>, DynamicBuffer<MuzzleOffset>>()
                 .WithAll<PlayerTag>())
        {

            if (weapon.ValueRO.CooldownTimer > 0)
            {
                weapon.ValueRW.CooldownTimer -= deltaTime;
            }


            if (input.ValueRO.IsFiring && weapon.ValueRO.CooldownTimer <= 0)
            {

                weapon.ValueRW.CooldownTimer = weapon.ValueRO.FireRate;


                foreach (var muzzle in muzzles)
                {

                    float3 muzzleWorldPos = transform.ValueRO.TransformPoint(muzzle.LocalPosition);


                    Entity projectile = ecb.Instantiate(weapon.ValueRO.ProjectilePrefab);

                    float3 shootDirection = math.normalize(targetAimPoint - muzzleWorldPos);


                    quaternion lookRotation = quaternion.LookRotation(shootDirection, math.up());


                    ecb.SetComponent(projectile, LocalTransform.FromPositionRotation(muzzleWorldPos, lookRotation));

                    ecb.SetComponent(projectile, new PhysicsVelocity
                    {
                        Linear = shootDirection * weapon.ValueRO.ProjectileSpeed,
                        Angular = float3.zero
                    });
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}