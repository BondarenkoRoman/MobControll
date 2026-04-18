using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Barrier : MonoBehaviour
{
    [SerializeField] private BarrierConfig BarrierConfig;
    [SerializeField] private TextMeshPro _hpText;
    [SerializeField] private UnityEvent OnTriggerMob;

    private readonly Dictionary<MobEntity, int> _processedMobSession = new(32);

    private int _hp;

    private void Awake()
    {
        _hp = BarrierConfig.Hp;
        UpdateHpDisplay();
    }

    private void UpdateHpDisplay()
    {
        _hpText.text = _hp.ToString();
    }

    private void SetHp(int hp)
    {
        _hp = Mathf.Max(0, hp);
        UpdateHpDisplay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<MobEntity>(out var mob))
            return;

        int session = mob.SpawnSession;
        if (_processedMobSession.TryGetValue(mob, out int recorded) && recorded == session)
            return;
        _processedMobSession[mob] = session;

        OnTriggerMob?.Invoke();
        if (mob.TryGetComponent<SplineMobMover>(out var mover))
            mover.StopFollowing();
        mob.TryKill();
        SetHp(--_hp);
    }
}
