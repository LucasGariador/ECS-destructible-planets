using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;

public class PlayerShooting : MonoBehaviour
{
    private EntityManager _entityManager;
    private World _world; 
    private Entity _inputEntity;

    void Start()
    {
        _world = World.DefaultGameObjectInjectionWorld;
        _entityManager = _world.EntityManager;

        _inputEntity = _entityManager.CreateEntity(typeof(RaycastInputData));
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            SendShootRequest();
        }
    }

    void SendShootRequest()
    {
        _entityManager.SetComponentData(_inputEntity, new RaycastInputData
        {
            Origin = transform.position,
            Direction = transform.forward,
            IsFiring = true
        });
    }

    private void OnDestroy()
    {
        if (_world == null || !_world.IsCreated) return;

        if (_entityManager.Exists(_inputEntity))
        {
            _entityManager.DestroyEntity(_inputEntity);
        }
    }
}