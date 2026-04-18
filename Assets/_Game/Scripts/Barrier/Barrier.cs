using UnityEngine;
using UnityEngine.Events;

public class Barrier : MonoBehaviour
{
    [SerializeField] private BarrierConfig BarrierConfig;
    [SerializeField] private UnityEvent OnTriggerMob;


    public void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<MobEntity>(out var mob))
        {
            OnTriggerMob?.Invoke();
            mob.GetComponent<SplineMobMover>().StopFollowing();
            mob.TryKill();
        }
    }
}
