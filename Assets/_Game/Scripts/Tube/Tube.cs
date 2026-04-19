using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using UnityEngine.Events;

public class Tube : MonoBehaviour
{
    [SerializeField] private Transform exitPoint;
    [SerializeField] private SplineComputer outputSpline;
    [SerializeField] private float respawnDelay = 0.35f;
    [SerializeField] private LayerMask collisionLayer;

    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityEvent OnExit;

    private readonly Dictionary<MobEntity, int> _processedMobSession = new(16);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<MobEntity>(out var mob) ||
            (collisionLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        int session = mob.SpawnSession;
        if (_processedMobSession.TryGetValue(mob, out int recorded) && recorded == session)
            return;
        _processedMobSession[mob] = session;

        if (exitPoint == null || outputSpline == null || GameFactory.Instance == null)
            return;

        if (mob.TryGetComponent<SplineMobMover>(out var mover))
            mover.StopFollowing();

        GameFactory.Instance.ReleaseMob(mob);
        OnEnter.Invoke();

        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        if (respawnDelay > 0f)
            yield return new WaitForSeconds(respawnDelay);

        if (GameFactory.Instance == null || exitPoint == null || outputSpline == null)
            yield break;

        SplineSample sample = outputSpline.Project(exitPoint.position);
        Vector3 pos = exitPoint.position;
        Quaternion rot = exitPoint.rotation;
        GameFactory.Instance.GetPlayerMobOnSpline(outputSpline, sample.percent, pos, rot);
        OnExit.Invoke();
    }
}
