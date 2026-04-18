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

        // Только позиция по сплайну: без поворота по касательной и без сдвига по Y.
        // _follower.applyDirectionRotation = false;
        // _follower.motion.applyRotation = false;
        // _follower.motion.applyPositionY = false;
    }
}
