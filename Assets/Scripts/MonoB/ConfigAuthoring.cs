using UnityEngine;
using Unity.Entities;

public class ConfigAuthoring : MonoBehaviour
{
    public GameObject voxelPrefab;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnerConfig
            {
                VoxelPrefab = GetEntity(authoring.voxelPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}