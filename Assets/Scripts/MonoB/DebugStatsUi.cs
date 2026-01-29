using UnityEngine;
using Unity.Entities;
using TMPro;
using Unity.Collections;

public class DebugStatsUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text statsText;

    [Header("Settings")]
    public float refreshRate = 0.2f;

    private EntityManager _entityManager;
    private EntityQuery _voxelQuery;
    private EntityQuery _projectileQuery;
    private EntityQuery _explosionQuery;

    private float _timer;
    private float _deltaTime = 0.0f;

    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        _entityManager = world.EntityManager;

        _voxelQuery = _entityManager.CreateEntityQuery(typeof(VoxelData));

        _projectileQuery = _entityManager.CreateEntityQuery(typeof(ProjectileTag));

    }

    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

        _timer += Time.deltaTime;
        if (_timer >= refreshRate)
        {
            UpdateUI();
            _timer = 0f;
        }
    }

    void UpdateUI()
    {
        float msec = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;

        int voxelCount = _voxelQuery.CalculateEntityCount();
        int projCount = _projectileQuery.CalculateEntityCount();

        NativeArray<Entity> allEntities = _entityManager.GetAllEntities(Allocator.Temp);
        int totalEntities = allEntities.Length;
        allEntities.Dispose();

        string text =
            $"<b>PERFORMANCE MONITOR</b>\n" +
            $"------------------\n" +
            $"FPS: <color=yellow>{fps:0.}</color> ({msec:0.0} ms)\n" +
            $"Total Entities: <color=white>{totalEntities}</color>\n" +
            $"\n" +
            $"<b>ACTIVE OBJECTS</b>\n" +
            $"Voxels: <color=green>{voxelCount}</color>\n" +
            $"Projectiles: <color=#00FFFF>{projCount}</color>";

        statsText.text = text;
    }
}