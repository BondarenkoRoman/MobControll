using UnityEngine;

public class BattleResolver : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 0.35f;

    [SerializeField] private int checkInterval = 2;
    [SerializeField] private LayerMask playerMobLayer;
    [SerializeField] private LayerMask enemyMobLayer;

    private readonly Collider[] _hitBuffer = new Collider[8];

    private int _frameCounter;

    private void Update()
    {
        _frameCounter++;
        if (_frameCounter % checkInterval != 0) return;

        if (GameFactory.Instance == null) return;
        ResolveTeam(GameFactory.Instance.PlayerMobs, enemyMobLayer);

    }

    private void ResolveTeam(
        System.Collections.Generic.IReadOnlyList<MobEntity> attackers,
        LayerMask targetLayer)
    {
        for (int i = 0; i < attackers.Count; i++)
        {
            MobEntity attacker = attackers[i];
            if (attacker == null || !attacker.IsAlive) continue;
            Vector3 origin = GetMobQueryOrigin(attacker);

            int hits = Physics.OverlapSphereNonAlloc(
                origin,
                detectionRadius,
                _hitBuffer,
                targetLayer,
                QueryTriggerInteraction.Collide
            );
            for (int h = 0; h < hits; h++)
            {
                MobEntity target = _hitBuffer[h].GetComponent<MobEntity>();
                if (target == null || !target.IsAlive) continue;
                bool killedAttacker = attacker.TryKill();
                bool killedTarget   = target.TryKill();
                if (killedAttacker) break;
            }
        }
    }

    private static Vector3 GetMobQueryOrigin(MobEntity mob)
    {
        if (mob.TryGetComponent<SphereCollider>(out var sphere))
            return sphere.transform.TransformPoint(sphere.center);
        if (mob.TryGetComponent<Collider>(out var col))
            return col.bounds.center;
        return mob.transform.position;
    }
}