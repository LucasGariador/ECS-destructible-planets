using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditorInternal;
using UnityEngine;

public class CameraFollowEntity : MonoBehaviour
{
    public Vector3 offset = Vector3.zero;
    private quaternion renderRot = quaternion.identity;

    private EntityManager _entityManager;
    private Entity _targetEntity;

    void LateUpdate()
    {
        if (_entityManager == default)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return;
            _entityManager = world.EntityManager;
        }

        if (_targetEntity == Entity.Null)
        {
            var query = _entityManager.CreateEntityQuery(typeof(PlayerTag));
            if (!query.IsEmpty) _targetEntity = query.GetSingletonEntity();
            return;
        }

        if (!_entityManager.Exists(_targetEntity)) return;

        LocalTransform entityTransform = _entityManager.GetComponentData<LocalTransform>(_targetEntity);

        transform.rotation = (Quaternion)renderRot;
        transform.position = entityTransform.Position + (float3)offset;
        transform.rotation = entityTransform.Rotation;
    }
}