using UnityEngine;
using Dreamteck.Splines;

[RequireComponent(typeof(SplineFollower))]
public class SplineMobMover : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 4f;
    public float GetBaseSpeed() => baseSpeed;

    private SplineFollower _follower;

    private void Awake()
    {
        _follower = GetComponent<SplineFollower>();

        // Follower сам двигает Transform — отключаем MoveStep в CrowdManager
        // для этого моба (CrowdManager проверяет HasSplineMover)
       // _follower.updateMethod = SplineFollower.Method.Update;
        _follower.follow       = true;
        _follower.wrapMode     = SplineFollower.Wrap.Default;
    }

    /// <summary>
    /// Вызывается после выдачи моба из GameFactory.
    /// lateralOffset/verticalOffset сохраняют индивидуальное смещение моба
    /// относительно центральной линии сплайна.
    /// </summary>
    public void Init(
        SplineComputer spline,
        double startPercent = 0.0,
        float lateralOffset = 0f,
        float verticalOffset = 0f
    )
    {
        _follower.follow = false;
        _follower.spline = spline;
        _follower.motion.offset = new Vector2(lateralOffset, verticalOffset);
        _follower.preserveUniformSpeedWithOffset = true;
        _follower.SetPercent(startPercent);

        _follower.follow = true;
    }

    //public void SetSpeed(float speed) => _follower.followSpeed = speed;

    public void  SetSpeed(float s) => _follower.followSpeed = -Mathf.Abs(s);

    // Когда моб умирает — останавливаем follower чтобы он не двигал
    // деактивированный объект (экономия на лишних вычислениях)
    public void StopFollowing() => _follower.follow = false;

    // Процент пути 0..1 — нужен GateController для спавна рядом
    public double CurrentPercent => _follower.GetPercent();
    public SplineComputer CurrentSpline => _follower != null ? _follower.spline : null;
}