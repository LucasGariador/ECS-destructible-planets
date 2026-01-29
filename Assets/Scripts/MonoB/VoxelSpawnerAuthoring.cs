using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class VoxelSpawnerAuthoring : MonoBehaviour
{
    public GameObject voxelPrefab;
    [Range(5, 100)]
    public float radius = 20f;

    class Baker : Baker<VoxelSpawnerAuthoring>
    {
        public override void Bake(VoxelSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new VoxelSpawner
            {
                VoxelPrefab = GetEntity(authoring.voxelPrefab, TransformUsageFlags.Dynamic),
                Radius = authoring.radius
            });
        }
    }
}

public struct VoxelSpawner : IComponentData
{
    public Entity VoxelPrefab;
    public float Radius;
}