using Unity.Entities;
using Unity.Mathematics;


public struct VoxelData : IComponentData
{
    public float3 GridPosition;
    public float Health;
}

public struct FallingTag : IComponentData, IEnableableComponent { }

public struct PlanetaryGravityTag : IComponentData, IEnableableComponent { }

public struct PlanetProcessedTag : IComponentData { }

public struct SpawnerConfig : IComponentData
{
    public Entity VoxelPrefab;
}


public struct IsMagmaTag : IComponentData { }
public struct IsRockTag : IComponentData { }