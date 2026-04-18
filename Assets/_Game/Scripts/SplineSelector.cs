using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class SplineSelector : MonoBehaviour
{
    public List<SplineComputer> enemySplineComputers;
    public List<SplineComputer> playerSplineComputers;

    public SplineComputer GetForEnemy()
    {
        return enemySplineComputers[0];
    }

    public SplineComputer GetClosestForPlayer(Vector3 spawnPoint)
    {
        if (playerSplineComputers == null || playerSplineComputers.Count == 0) return null;

        SplineComputer bestSpline = null;
        float bestSqrDistance = float.MaxValue;

        for (int i = 0; i < playerSplineComputers.Count; i++)
        {
            SplineComputer candidate = playerSplineComputers[i];
            if (candidate == null) continue;

            Vector3 playerStartPoint = candidate.EvaluatePosition(0.0);
            float sqrDistance = (playerStartPoint - spawnPoint).sqrMagnitude;

            if (sqrDistance < bestSqrDistance)
            {
                bestSqrDistance = sqrDistance;
                bestSpline = candidate;
            }
        }

        return bestSpline;
    }
}
