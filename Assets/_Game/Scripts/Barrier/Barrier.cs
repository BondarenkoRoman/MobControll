using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Barrier : MonoBehaviour
{
    [SerializeField] private BarrierConfig BarrierConfig;
    [SerializeField] private TextMeshPro _hpText;
    [SerializeField] private UnityEvent OnTriggerMob;

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
        if(other.TryGetComponent<MobEntity>(out var mob))
        {
            OnTriggerMob?.Invoke();
            mob.GetComponent<SplineMobMover>().StopFollowing();
            mob.TryKill();
            SetHp(--_hp);
        }
    }
}
