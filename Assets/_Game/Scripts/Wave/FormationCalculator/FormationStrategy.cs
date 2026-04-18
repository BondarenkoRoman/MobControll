using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FormationStrategy : ScriptableObject
{
   public abstract Vector3 GetPositionByIndex(int index);
}
