using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MobControl/MobEntityConfig", fileName = "MobEntityConfig")]
public class MobEntityConfig : ScriptableObject
{
    public float DieRotationDuration = 0.35f;
    public float DieMoveDownDuration = 0.35f;
}
