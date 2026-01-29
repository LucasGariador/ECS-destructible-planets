using Unity.Entities;
using Unity.Mathematics;

public struct PlanetGenerator : IComponentData
{
    public float Radius;
    public int RandomSeed;
    public bool IsProcessed;
}

public struct GravityTarget : IComponentData
{
    public float3 CenterPosition;
}