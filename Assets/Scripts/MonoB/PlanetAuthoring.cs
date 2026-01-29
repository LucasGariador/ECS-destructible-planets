using UnityEngine;
using Unity.Entities;

public class PlanetAuthoring : MonoBehaviour
{
    public float radius = 15f;
    public int seed = 1234;

    class Baker : Baker<PlanetAuthoring>
    {
        public override void Bake(PlanetAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlanetGenerator
            {
                Radius = authoring.radius,
                RandomSeed = authoring.seed
            });

        }
    }
}