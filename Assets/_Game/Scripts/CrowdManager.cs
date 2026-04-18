using System.Collections.Generic;
using UnityEngine;



// этот нужен только что бы иметь колекцию заспавленых врагов и игроков
public class CrowdManager : MonoBehaviour
{
    public static CrowdManager Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float enemySpeed  = 4f;

    // Две отдельные команды — не смешиваем в один список,
    // это упрощает логику BattleResolver
    private readonly List<MobEntity> _playerMobs = new List<MobEntity>(500);
    private readonly List<MobEntity> _enemyMobs  = new List<MobEntity>(500);
    private readonly List<MobEntity> _toRemove   = new List<MobEntity>(64);

    public IReadOnlyList<MobEntity> PlayerMobs => _playerMobs;
    public IReadOnlyList<MobEntity> EnemyMobs  => _enemyMobs;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        MoveMobs(_playerMobs, playerSpeed, dt);
        MoveMobs(_enemyMobs,  enemySpeed,  dt);
        FlushRemoved();
    }

    private void MoveMobs(List<MobEntity> mobs, float speed, float dt)
    {
        for (int i = 0; i < mobs.Count; i++)
        {
            var mob = mobs[i];
            if (mob == null || !mob.IsAlive) { _toRemove.Add(mob); continue; }

            // SplineFollower управляет движением сам — не вмешиваемся
            if (mob.GetComponent<SplineMobMover>() != null) continue;

            mob.MoveStep(speed, dt);
        }
    }

    private void FlushRemoved()
    {
        if (_toRemove.Count == 0) return;
        foreach (var mob in _toRemove)
        {
            _playerMobs.Remove(mob);
            _enemyMobs.Remove(mob);
        }
        _toRemove.Clear();
    }

    // ── Registration ────────────────────────────────────────────

    public void RegisterMob(MobEntity mob)
    {
        if (mob.Team == MobTeam.Player) _playerMobs.Add(mob);
        else                            _enemyMobs.Add(mob);
    }

    public void UnregisterMob(MobEntity mob) => _toRemove.Add(mob);

}