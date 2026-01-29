using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public partial struct ProjectileCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        var projectileGroup = SystemAPI.GetComponentLookup<ProjectileTag>(true);
        var voxelGroup = SystemAPI.GetComponentLookup<VoxelData>(true);

        state.Dependency = new ProjectileHitJob
        {
            ECB = ecb,
            ProjectileGroup = projectileGroup,
            VoxelGroup = voxelGroup,
            TransformGroup = transformLookup
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    struct ProjectileHitJob : ITriggerEventsJob
    {
        public EntityCommandBuffer ECB;
        [ReadOnly] public ComponentLookup<ProjectileTag> ProjectileGroup;
        [ReadOnly] public ComponentLookup<VoxelData> VoxelGroup;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;


            Entity projectileEntity = Entity.Null;
            Entity hitTargetEntity = Entity.Null;

            if (ProjectileGroup.HasComponent(entityA)) { projectileEntity = entityA; hitTargetEntity = entityB; }
            else if (ProjectileGroup.HasComponent(entityB)) { projectileEntity = entityB; hitTargetEntity = entityA; }
            else return;


            if (VoxelGroup.HasComponent(hitTargetEntity))
            {

                if (TransformGroup.HasComponent(projectileEntity))
                {
                    var pos = TransformGroup[projectileEntity].Position;

                    Entity explosion = ECB.CreateEntity();
                    ECB.AddComponent(explosion, new ExplosionEvent
                    {
                        Position = pos,
                        Radius = 3.0f,
                        Force = 10.0f
                    });

                }
            }

            ECB.DestroyEntity(projectileEntity);
        }
    }
}