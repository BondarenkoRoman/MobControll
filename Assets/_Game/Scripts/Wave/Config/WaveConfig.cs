using UnityEngine;

[CreateAssetMenu(menuName = "MobControl/Wave Config", fileName = "WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Min(1)] public int enemyCount = 10;
    [Min(0f)] public float preWaveDelay = 2f;
    [Min(0f)] public float afterWaveDelay = 2f;
    [Min(0.01f)] public float enemySpawnInGroupInterval = 1.5f;
}