using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

[UpdateAfter(typeof(ProjectileCollisionSystem))]
[BurstCompile]
public partial struct VoxelDestructionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<PhysicsWorldSingleton>()) return;

        PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var gravityLookup = SystemAPI.GetComponentLookup<GravityTarget>(true);

        foreach (var (explosion, entity) in SystemAPI.Query<RefRO<ExplosionEvent>>().WithEntityAccess())
        {
            float3 explosionPos = explosion.ValueRO.Position;
            float radius = explosion.ValueRO.Radius;
            float outwardForce = 25.0f;

            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

            if (physicsWorld.CalculateDistance(new PointDistanceInput
            {
                Position = explosionPos,
                MaxDistance = radius,
                Filter = CollisionFilter.Default
            }, ref hits))
            {
                foreach (var hit in hits)
                {
                    Entity hitEntity = hit.Entity;

                    if (SystemAPI.HasComponent<VoxelData>(hitEntity) && !SystemAPI.HasComponent<PhysicsVelocity>(hitEntity))
                    {
                        var random = Unity.Mathematics.Random.CreateFromIndex((uint)hitEntity.Index + (uint)state.WorldUnmanaged.Time.ElapsedTime);

                        
                        ecb.AddComponent(hitEntity, PhysicsMass.CreateDynamic(MassProperties.UnitSphere, 1f));
                        ecb.AddComponent(hitEntity, new PhysicsDamping { Linear = 0.2f, Angular = 0.5f });

                       
                        ecb.RemoveComponent<PhysicsCollider>(hitEntity);

                        
                        float3 impulseDirection = math.up();

                        if (gravityLookup.HasComponent(hitEntity))
                        {
                            float3 planetCenter = gravityLookup[hitEntity].CenterPosition;
                            
                            impulseDirection = math.normalize(hit.Position - planetCenter);
                        }

                        float3 finalVelocity = impulseDirection * outwardForce * random.NextFloat(0.5f, 1.5f);
                        ecb.AddComponent(hitEntity, new PhysicsVelocity { Linear = finalVelocity });

                        
                        if (SystemAPI.HasComponent<PlanetaryGravityTag>(hitEntity))
                            ecb.RemoveComponent<PlanetaryGravityTag>(hitEntity);
                        ecb.AddComponent<PhysicsGravityFactor>(hitEntity, new PhysicsGravityFactor { Value = 0f });


                    }
                }
            }
            hits.Dispose();
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}