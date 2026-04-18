using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<MobEntity>(out var mob))
        {
            mob.GetComponent<SplineMobMover>().StopFollowing();
            mob.TryKill();
        }
    }
}
