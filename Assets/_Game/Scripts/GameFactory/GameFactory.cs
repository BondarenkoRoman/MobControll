using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Dreamteck.Splines;

public class GameFactory : MonoBehaviour
{
    public static GameFactory Instance { get; private set; }
    [SerializeField] private SplineSelector splineSelector;
    [SerializeField] private MobEntity mobPrefab;
    [SerializeField] private int defaultCapacity = 256;
    [SerializeField] private int maxSize = 512;
    private IObjectPool<MobEntity> _pool;

    public IReadOnlyList<MobEntity> PlayerMobs => _playerMobs;
    private readonly List<MobEntity> _playerMobs = new List<MobEntity>(500);

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
        mob.OnSpawned(MobTeam.Enemy);// тут обьеденить
        mob.SetColor(Color.red);

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
        mob.SetColor(Color.cyan);

        var mover = mob.GetComponent<SplineMobMover>();
        var spline = splineSelector.GetClosestForPlayer(position);
        var sample = spline.Project(position);
        InitMoverFromSplineSample(mover, spline, sample, position);
        // RegisterMob(mob);
        return mob;
    }

    public MobEntity GetPlayerMobOnSpline(SplineComputer spline, double splinePercent, Vector3 position, Quaternion rotation)
    {
        var mob = _pool.Get();
        mob.transform.SetPositionAndRotation(position, rotation);
        mob.OnSpawned(MobTeam.Player);
        mob.SetColor(Color.cyan);

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
        _pool?.Clear();
    }


    private void RegisterMob(MobEntity mob) => _playerMobs.Add(mob);

    private void UnregisterMob(MobEntity mob) => _playerMobs.Remove(mob);
}