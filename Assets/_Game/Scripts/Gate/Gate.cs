using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using TMPro;

public class Gate : MonoBehaviour
{
    [SerializeField] private GateConfig gateConfig;
    [SerializeField] private TextMeshPro multText;

    private readonly Dictionary<MobEntity, int> _processedMobSession = new(32);

    public void Start()
    {
        SetMult();
    }

    private void SetMult()
    {
        multText.text = $"X{gateConfig.Mult}"; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<MobEntity>(out var mob))
            return;

        if (!mob.TryGetComponent<SplineMobMover>(out var sourceMover))
            return;

        int session = mob.SpawnSession;
        if (_processedMobSession.TryGetValue(mob, out int recorded) && recorded == session)
            return;
        _processedMobSession[mob] = session;

        SplineComputer spline = sourceMover.CurrentSpline;
        double mobPercent = sourceMover.CurrentPercent;
        SplineSample sampleAtMob = spline.Evaluate(mobPercent);
        var count = gateConfig.Mult - 1;
        for (int i = 0; i < count; i++)
        {
            float lateral = (i - (gateConfig.Mult - 1) * 0.5f) * gateConfig.LateralSpacing;
            Vector3 spawnPos = mob.transform.position
                + sampleAtMob.forward * gateConfig.AdvanceAlongSplineDistance
                + sampleAtMob.right * lateral;
            spawnPos.y = mob.transform.position.y;

            SplineSample projected = spline.Project(spawnPos);
            GameFactory.Instance.GetPlayerMobOnSpline(spline, projected.percent, spawnPos, projected.rotation);
        }
    }
}
