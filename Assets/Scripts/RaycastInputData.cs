using Unity.Entities;
using Unity.Mathematics;

public struct RaycastInputData : IComponentData
{
    public float3 Origin;
    public float3 Direction;
    public bool IsFiring;
}