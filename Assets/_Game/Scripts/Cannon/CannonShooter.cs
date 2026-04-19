using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [SerializeField] private GameFactory gameFactory;
    [SerializeField] private Transform firePoint;
    [SerializeField] [Min(0f)] private float fireCooldown = 0.35f;

    private float _nextFireTime;

    private void Update()
    {
        if (!Input.GetMouseButton(0))
            return;

        if (Time.time < _nextFireTime)
            return;

        Fire();
        _nextFireTime = Time.time + fireCooldown;
    }

    private void Fire()
    {
        gameFactory.GetPlayerMob(firePoint.position, firePoint.rotation);
    }
}
