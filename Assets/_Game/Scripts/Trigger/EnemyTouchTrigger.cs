using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class EnemyTouchTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMobLayer;
    [SerializeField] private UnityEvent onEnemyEntered;

    [SerializeField] private bool triggerOnce = true;

    private bool _firedGlobally;
    private readonly Dictionary<MobEntity, int> _processedMobSession = new(16);

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && _firedGlobally)
            return;

        if (!TryGetMob(other, out var mob))
            return;
        if (mob.Team != MobTeam.Enemy)
            return;

        int mobLayer = mob.gameObject.layer;
        if ((enemyMobLayer.value & (1 << mobLayer)) == 0)
            return;

        if (!triggerOnce)
        {
            int session = mob.SpawnSession;
            if (_processedMobSession.TryGetValue(mob, out int recorded) && recorded == session)
                return;
            _processedMobSession[mob] = session;
        }

        if (triggerOnce)
            _firedGlobally = true;

        onEnemyEntered?.Invoke();
    }

    private static bool TryGetMob(Collider other, out MobEntity mob)
    {
        if (other.TryGetComponent(out mob))
            return true;
        mob = other.GetComponentInParent<MobEntity>();
        return mob != null;
    }
}
