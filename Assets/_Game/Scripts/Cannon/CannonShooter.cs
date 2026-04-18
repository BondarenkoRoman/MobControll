using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [SerializeField] private GameFactory gameFactory;
    [SerializeField] private Transform firePoint;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Fire();
    }

    private void Fire()
    {
        gameFactory.GetPlayerMob(firePoint.position, firePoint.rotation);
    }
}
