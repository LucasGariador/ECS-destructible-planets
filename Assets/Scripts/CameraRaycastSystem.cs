using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(PlayerShootingSystem))]
public partial class CameraRaycastSystem : SystemBase
{
    protected override void OnCreate()
    {
        EntityManager.CreateSingleton<MainCameraTarget>();
    }

    protected override void OnUpdate()
    {
        if (Camera.main == null) return;

        UnityEngine.Ray unityRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastInput input = new RaycastInput
        {
            Start = unityRay.origin,
            End = unityRay.origin + (unityRay.direction * 1000f),
            Filter = CollisionFilter.Default
        };

        PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        RefRW<MainCameraTarget> targetData = SystemAPI.GetSingletonRW<MainCameraTarget>();

        if (physicsWorld.CastRay(input, out Unity.Physics.RaycastHit hit))
        {
            targetData.ValueRW.HitPosition = hit.Position;
            targetData.ValueRW.HitSomething = true;
        }
        else
        {
            targetData.ValueRW.HitPosition = input.End;
            targetData.ValueRW.HitSomething = false;
        }
    }
}