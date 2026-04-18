using UnityEngine;
using UnityEngine.Pool;

public class GameFactory : MonoBehaviour
{
    public static GameFactory Instance { get; private set; }
    [SerializeField] private SplineSelector splineSelector;
    [SerializeField] private MobEntity mobPrefab;
    [SerializeField] private int defaultCapacity = 256;
    [SerializeField] private int maxSize = 512;
    private IObjectPool<MobEntity> _pool;


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
        Vector3 delta = position - sample.position;
        float lateralOffset = Vector3.Dot(delta, sample.right);
        float verticalOffset = Vector3.Dot(delta, sample.up);
        mover.Init(spline, sample.percent, lateralOffset, verticalOffset);

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
        Vector3 delta = position - sample.position;
        float lateralOffset = Vector3.Dot(delta, sample.right);
        float verticalOffset = Vector3.Dot(delta, sample.up);
        mover.Init(spline, sample.percent, lateralOffset, verticalOffset);

        return mob;
    }

    public void ReleaseMob(MobEntity mob)
    {
        if (mob == null) return;
        _pool.Release(mob);
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
}