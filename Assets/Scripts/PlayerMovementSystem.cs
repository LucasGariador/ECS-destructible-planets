using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (velocity, transform, input, settings) in
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>, RefRO<PlayerInput>, RefRO<PlayerSettings>>()
                 .WithAll<PlayerTag>())
        {
            float currentSpeed = math.length(velocity.ValueRO.Linear);
            float minSpeedForControl = 2.0f;
            float authority = math.clamp(currentSpeed / minSpeedForControl, 0f, 1f);


            if (authority > 0.01f)
            {

                float yawInput = input.ValueRO.LookInput.x * settings.ValueRO.RotateSpeed * authority;
                float pitchInput = -input.ValueRO.LookInput.y * settings.ValueRO.RotateSpeed * authority;

                float3 yawVector = math.up() * yawInput;

                float3 pitchVector = transform.ValueRO.Right() * pitchInput;


                float3 right = transform.ValueRO.Right();
                float rollError = right.y;

                velocity.ValueRW.Angular = yawVector + pitchVector;
            }
            else
            {
                velocity.ValueRW.Angular = float3.zero;
            }

            float targetSpeed = settings.ValueRO.MoveSpeed;
            if (input.ValueRO.IsBoosting) targetSpeed *= 2.5f;

            float3 forwardThrust = transform.ValueRO.Forward() * input.ValueRO.MoveInput.y;
            float3 sideThrust = transform.ValueRO.Right() * input.ValueRO.MoveInput.x;

            if (math.lengthsq(forwardThrust + sideThrust) > 0.001f)
            {
                velocity.ValueRW.Linear += (forwardThrust + sideThrust) * targetSpeed * deltaTime;
            }
        }
    }
}