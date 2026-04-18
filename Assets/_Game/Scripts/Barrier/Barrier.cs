using UnityEngine;
using UnityEngine.Events;

public class Barrier : MonoBehaviour
{
    [SerializeField] private UnityEvent OnTriggerMob;
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
