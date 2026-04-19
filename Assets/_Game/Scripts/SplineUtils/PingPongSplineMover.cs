using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class PingPongSplineMover : SplineMover
{
    protected override void Init()
    {
        base.Init();
        _follower.follow = true;
        _follower.wrapMode = SplineFollower.Wrap.PingPong;
        _follower.followSpeed = Mathf.Abs(_config.MoveSpeed);
    }
}
