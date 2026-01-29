using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct PlanetaryGravitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float gravityStrength = 5.0f;

        foreach (var (velocity, transform, gravityTarget) in
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<LocalTransform>, RefRO<GravityTarget>>()
                 .WithAll<PlanetaryGravityTag>())
        {
            float3 myPlanetCenter = gravityTarget.ValueRO.CenterPosition;

            float3 directionToCenter = math.normalize(myPlanetCenter - transform.ValueRO.Position);

            velocity.ValueRW.Linear += deltaTime * gravityStrength * -directionToCenter;
        }
    }
}