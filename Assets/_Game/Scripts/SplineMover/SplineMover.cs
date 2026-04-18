using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class SplineMover : MonoBehaviour
{
    [SerializeField] protected SplineFollower _follower;
    [SerializeField] private SplineComputer spline;
    [SerializeField] protected SplineMoverConfig _config;

    private void Start() => Init();

    protected virtual void Init()
    {
        _follower.spline = spline;
    }
}
