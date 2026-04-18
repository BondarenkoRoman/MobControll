using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveGenerator : MonoBehaviour
{
    public static EnemyWaveGenerator Instance { get; private set; }

    [SerializeField] private List<WaveConfig> waveConfigs = new List<WaveConfig>();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private FormationStrategy formationStrategy;
    [SerializeField] private GameFactory gameFactory;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartWaves();
    }

    public void StartWaves()
    {
        StartCoroutine(SpawnWaves());
    }

    public void StopWaves()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnWaves()
    {
        for (int i = 0; i < waveConfigs.Count; i++)
        {
            WaveConfig config = waveConfigs[i];

            if (config.preWaveDelay > 0f)
                yield return new WaitForSeconds(config.preWaveDelay);

            yield return StartCoroutine(SpawnWave(config));

            if (config.preWaveDelay > 0f)
                yield return new WaitForSeconds(config.afterWaveDelay);
        }
    }

    private IEnumerator SpawnWave(WaveConfig config)
    {
        int remaining  = config.enemyCount;

        while (remaining > 0)
        {
            int index = config.enemyCount - remaining;
            CreateEnemy(index);
            remaining--;
            if (remaining > 0)
                yield return new WaitForSeconds(config.enemySpawnInGroupInterval);
        }
    }

    private void CreateEnemy(int index)
    {
        Vector3 worldPos = spawnPoint.TransformPoint(formationStrategy.GetPositionByIndex(index));
        gameFactory.GetEnemyMob(worldPos, spawnPoint.rotation);
    }
}