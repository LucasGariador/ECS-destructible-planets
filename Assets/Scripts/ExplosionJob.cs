using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct ExplosionJob : IJobEntity
{
    public float3 Center;
    public float Radius;
    public float Force;

    void Execute(ref VoxelData voxel, EnabledRefRW<FallingTag> fallingTag)
    {
        float dist = math.distance(voxel.GridPosition, Center);
        if (dist < Radius)
        {
            voxel.Health -= Force / dist;
            if (voxel.Health <= 0)
            {
                fallingTag.ValueRW = true;
            }
        }
    }
}