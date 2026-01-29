using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Rendering;

[BurstCompile]
public partial struct VoxelSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<SpawnerConfig>()) return;
        Entity voxelPrefab = SystemAPI.GetSingleton<SpawnerConfig>().VoxelPrefab;


        var queryTotal = SystemAPI.QueryBuilder().WithAll<PlanetGenerator>().Build();
        int totalGenerators = queryTotal.CalculateEntityCount();

        Entity planetEntity = Entity.Null;
        float3 planetPos = float3.zero;
        float planetRadius = 0;
        bool found = false;


        foreach (var (gen, transform, entity) in
                 SystemAPI.Query<RefRW<PlanetGenerator>, RefRO<LocalTransform>>()
                 .WithNone<VoxelData>()
                 .WithEntityAccess())
        {
            if (gen.ValueRO.IsProcessed) continue;

            UnityEngine.Debug.Log($"[Planeta Real] Encontrado: {entity.Index}. Total Generadores detectados: {totalGenerators}");

            gen.ValueRW.IsProcessed = true;

            planetEntity = entity;
            planetPos = transform.ValueRO.Position;
            planetRadius = gen.ValueRO.Radius;
            found = true;
            break;
        }

        if (found)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            float safeRadius = math.clamp(planetRadius, 1, 20);
            float radiusSq = safeRadius * safeRadius;
            int r = (int)math.ceil(safeRadius);

            int voxelsSpawned = 0;

            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    for (int z = -r; z <= r; z++)
                    {
                        float3 localPos = new float3(x, y, z);
                        if (math.lengthsq(localPos) <= radiusSq)
                        {
                            Entity voxel = ecb.Instantiate(voxelPrefab);
                            float3 finalPos = planetPos + localPos;

                            ecb.RemoveComponent<PlanetGenerator>(voxel);

                            ecb.SetComponent(voxel, LocalTransform.FromPosition(finalPos).WithScale(0.95f));

                            float dist = math.sqrt(math.lengthsq(localPos));
                            float4 color = (dist > safeRadius - 2) ? new float4(0.2f, 0.8f, 0.2f, 1) : new float4(0.5f, 0.3f, 0.1f, 1);
                            ecb.AddComponent(voxel, new URPMaterialPropertyBaseColor { Value = color });

                            ecb.AddComponent(voxel, new VoxelData { Health = 100f });
                            ecb.AddComponent(voxel, new GravityTarget { CenterPosition = planetPos });

                            voxelsSpawned++;
                        }
                    }
                }
            }

            UnityEngine.Debug.Log($"--> Generados {voxelsSpawned} vóxeles. Fin del frame.");
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}