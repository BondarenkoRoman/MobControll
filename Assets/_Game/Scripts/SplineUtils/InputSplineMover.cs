using UnityEngine;
using Dreamteck.Splines;

public class InputSplineMover : SplineMover
{
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    private double _inputPercent;


    protected override void Init()
    {
        base.Init();
        _follower.follow = false;
        _follower.wrapMode = SplineFollower.Wrap.Default;
        _follower.RebuildImmediate();
        _inputPercent = _follower.spline.Project(_follower.transform.position).percent;
        _follower.SetPercent(_inputPercent);
    }

    private void Update()
    {

        float direction = 0f;
        if (Input.GetKey(moveLeftKey)) direction -= 1f;
        if (Input.GetKey(moveRightKey)) direction += 1f;

        if (Mathf.Approximately(direction, 0f)) return;

        _inputPercent += direction * _config.MoveSpeed * Time.deltaTime;
        _inputPercent = Mathf.Clamp01((float)_inputPercent);
        _follower.SetPercent(_inputPercent);
    }
}
