using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Barrier : MonoBehaviour
{
    [SerializeField] private UnityEvent OnTriggerMob;
    [SerializeField] private LayerMask collisionPayer;

    private readonly Dictionary<MobEntity, int> _processedMobSession = new(32);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<MobEntity>(out var mob) ||
            (collisionPayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        int session = mob.SpawnSession;
        if (_processedMobSession.TryGetValue(mob, out int recorded) && recorded == session)
            return;
        _processedMobSession[mob] = session;

        OnTriggerMob?.Invoke();
        if (mob.TryGetComponent<SplineMobMover>(out var mover))
            mover.StopFollowing();
        mob.TryKill();
    }
}
