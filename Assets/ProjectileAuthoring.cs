using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{

    class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ProjectileTag());
        }
    }
}

public struct ProjectileTag : IComponentData { }

public struct ExplosionEvent : IComponentData
{
    public float3 Position;
    public float Radius;
    public float Force;
}