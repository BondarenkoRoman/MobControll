using UnityEngine;
using Dreamteck.Splines;

[RequireComponent(typeof(SplineFollower))]
public class SplineMobMover : MonoBehaviour
{
    private SplineFollower _follower;
    private SplineComputer _spline;

    private void Awake()
    {
        _follower = GetComponent<SplineFollower>();
        _follower.wrapMode     = SplineFollower.Wrap.Default;
    }

    public void Init(SplineComputer spline, double startPercent, float lateralOffset, float verticalOffset)
    {
        _spline = spline;
        _follower.follow = false;
        _follower.spline = spline;
        _follower.RebuildImmediate();
        _follower.motion.offset = new Vector2(lateralOffset, verticalOffset);
        _follower.preserveUniformSpeedWithOffset = true;
        _follower.SetPercent(startPercent);

        _follower.follow = true;
    }

    public void StopFollowing() => _follower.follow = false;

    public double CurrentPercent => _follower.GetPercent();
    public SplineComputer CurrentSpline => _follower != null ? _follower.spline : null;
}