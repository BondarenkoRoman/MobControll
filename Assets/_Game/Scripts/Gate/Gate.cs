using UnityEngine;
using Dreamteck.Splines;
using TMPro;

public class Gate : MonoBehaviour
{
    [SerializeField] private GateConfig gateConfig;
    [SerializeField] private TextMeshPro multText;


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

        SplineComputer spline = sourceMover.CurrentSpline;
        double percent = sourceMover.CurrentPercent;
        percent = spline.Travel(percent, gateConfig.AdvanceAlongSplineDistance, Spline.Direction.Forward);
        SplineSample sample = spline.Evaluate(percent);

        for (int i = 0; i < gateConfig.Mult; i++)
        {
            float lateral = (i - (gateConfig.Mult - 1) * 0.5f) * gateConfig.LateralSpacing;
            Vector3 spawnPos = sample.position + sample.right * lateral;
            Vector3 spawnPosY = new Vector3(spawnPos.x, mob.transform.position.y, spawnPos.z);
            Quaternion spawnRot = sample.rotation;
            GameFactory.Instance.GetPlayerMobOnSpline(spline, percent, spawnPosY, spawnRot);
        }
    }
}
