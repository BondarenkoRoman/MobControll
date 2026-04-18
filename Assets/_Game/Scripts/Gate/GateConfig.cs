using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MobControl/GateConfig", fileName = "GateConfig")]
public class GateConfig : ScriptableObject
{
    public int Mult = 2;
    public float AdvanceAlongSplineDistance = 1.2f;
    public float LateralSpacing = 0.55f;
}
