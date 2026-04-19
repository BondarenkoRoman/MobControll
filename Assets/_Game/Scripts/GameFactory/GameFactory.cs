using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Dreamteck.Splines;

public class GameFactory : MonoBehaviour
{
    public static GameFactory Instance { get; private set; }
    [SerializeField] private SplineSelector splineSelector;
    [SerializeField] private MobEntity mobPrefab;

    [Header("Physics layers (имена как в Tags & Layers)")]
    [SerializeField] private string playerMobLayerName = "player";
    [SerializeField] private string enemyMobLayerName = "enemy";
    [SerializeField] private int defaultCapacity = 256;
    [SerializeField] private int maxSize = 512;

    [SerializeField] private MobEntityConfig mobEntityConfig;
    private IObjectPool<MobEntity> _pool;

    public IReadOnlyList<MobEntity> PlayerMobs => _playerMobs;
    public IReadOnlyList<MobEntity> EnemyMobs  => _enemyMobs;

    private readonly List<MobEntity> _playerMobs = new List<MobEntity>(500);
    private readonly List<MobEntity> _enemyMobs  = new List<MobEntity>(500);


    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _pool = new ObjectPool<MobEntity>(
            createFunc: CreateMob,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    public MobEntity GetEnemyMob(Vector3 position, Quaternion rotation)
    {
        var mob = _pool.Get();
        mob.transform.SetPositionAndRotation(position, rotation);
        mob.OnSpawned(MobTeam.Enemy);
        ApplyMobPhysicsLayer(mob, enemyMobLayerName);
        mob.SetColor(mobEntityConfig.EnemyColor);
        RegisterMob(mob);

        var mover = mob.GetComponent<SplineMobMover>();
        var spline = splineSelector.GetForEnemy();
        var sample = spline.Project(position);
        InitMoverFromSplineSample(mover, spline, sample, position);

        return mob;
    }

    public MobEntity GetPlayerMob(Vector3 position, Quaternion rotation)
    {
        var mob = _pool.Get();
        mob.transform.SetPositionAndRotation(position, rotation);    
        mob.OnSpawned(MobTeam.Player);
        ApplyMobPhysicsLayer(mob, playerMobLayerName);
        mob.SetColor(mobEntityConfig.PlayerColor);
        RegisterMob(mob);

        var mover = mob.GetComponent<SplineMobMover>();
        var spline = splineSelector.GetClosestForPlayer(position);
        var sample = spline.Project(position);
        InitMoverFromSplineSample(mover, spline, sample, position);
        return mob;
    }

    public MobEntity GetPlayerMobOnSpline(SplineComputer spline, double splinePercent, Vector3 position, Quaternion rotation)
    {
        var mob = _pool.Get();
        mob.transform.SetPositionAndRotation(position, rotation);
        mob.OnSpawned(MobTeam.Player);
        ApplyMobPhysicsLayer(mob, playerMobLayerName);
        mob.SetColor(mobEntityConfig.PlayerColor);
        RegisterMob(mob);

        var mover = mob.GetComponent<SplineMobMover>();
        var sample = spline.Evaluate(splinePercent);
        InitMoverFromSplineSample(mover, spline, sample, position, splinePercent);

        return mob;
    }

    public void ReleaseMob(MobEntity mob)
    {
        if (mob == null) return;
        _pool.Release(mob);
    }

    private static void InitMoverFromSplineSample(SplineMobMover mover, SplineComputer spline, SplineSample sample, Vector3 worldPosition, double? startPercentOverride = null)
    {
        Vector3 delta = worldPosition - sample.position;
        float lateralOffset = Vector3.Dot(delta, sample.right);
        float verticalOffset = Vector3.Dot(delta, sample.up);
        double startPercent = startPercentOverride ?? sample.percent;
        mover.Init(spline, startPercent, lateralOffset, verticalOffset);
    }

    private MobEntity CreateMob()
    {
        var mob = Instantiate(mobPrefab, transform);
        mob.gameObject.SetActive(false);
        return mob;
    }

    private void OnTakeFromPool(MobEntity mob)
    {
        mob.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(MobEntity mob)
    {
        UnregisterMob(mob);
        mob.OnDespawned();
        mob.gameObject.SetActive(false);
        mob.transform.SetParent(transform);
    }

    private void OnDestroyPoolObject(MobEntity mob)
    {
        if (mob != null)
            Destroy(mob.gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        _pool?.Clear();
    }

    private void RegisterMob(MobEntity mob)
    {
        if (mob.Team == MobTeam.Player) _playerMobs.Add(mob);
        else                            _enemyMobs.Add(mob);
    }

    private void UnregisterMob(MobEntity mob)
    {
        _playerMobs.Remove(mob);
        _enemyMobs.Remove(mob);
    }

    private static void ApplyMobPhysicsLayer(MobEntity mob, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer < 0)
            return;

        SetLayerRecursively(mob.transform, layer);
    }

    private static void SetLayerRecursively(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++)
            SetLayerRecursively(t.GetChild(i), layer);
    }
}