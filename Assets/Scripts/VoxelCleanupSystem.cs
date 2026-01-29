using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public partial struct VoxelCleanupSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float killDistanceSq = 100f * 100f;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transform, entity) in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                 .WithAll<VoxelData>()
                 .WithEntityAccess())
        {
            if (math.lengthsq(transform.ValueRO.Position) > killDistanceSq)
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}