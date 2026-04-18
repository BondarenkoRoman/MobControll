using UnityEngine;

[CreateAssetMenu(menuName = "MobControl/WaveFormationStrategy", fileName = "WaveFormationStrategy")]
public class WaveFormationStrategy : FormationStrategy
{
    [SerializeField, Min(1)] private int firstLineCount = 4;
    [SerializeField, Min(0)] private int lineStep = 3;
    [SerializeField, Min(1)] private int maxPerLine = 10;
    [SerializeField, Min(0.01f)] private float spacing = 1f;
    [SerializeField, Range(0f, 0.5f)] public float positionJitter = 0.25f;
    [SerializeField, Range(0f, 2f)] public float depthScatter = 0.8f;


    public override Vector3 GetPositionByIndex(int index)
    {
        if (index < 0)
            return Vector3.zero;

        int safeMaxPerLine = Mathf.Max(1, maxPerLine);
        int safeFirstLineCount = Mathf.Clamp(firstLineCount, 1, safeMaxPerLine);
        int safeLineStep = Mathf.Max(0, lineStep);
        float safeSpacing = Mathf.Max(0.01f, spacing);

        int row = 0;
        int rowStartIndex = 0;
        int rowWidth = safeFirstLineCount;

        while (index >= rowStartIndex + rowWidth)
        {
            rowStartIndex += rowWidth;
            row++;
            rowWidth = GetRowWidth(row, safeFirstLineCount, safeLineStep, safeMaxPerLine);
        }

        int col = index - rowStartIndex;
        float rowVisualWidth = (rowWidth - 1) * safeSpacing;
        float startX = -rowVisualWidth * 0.5f;

        float x = startX + col * safeSpacing;
        float z = -row * safeSpacing;

        float safeJitter = Mathf.Max(0f, positionJitter);
        float safeDepthScatter = Mathf.Max(0f, depthScatter);

        if (safeJitter > 0f)
            x += Random.Range(-safeJitter, safeJitter) * safeSpacing;

        if (safeDepthScatter > 0f)
            z += Random.Range(-safeDepthScatter * 0.5f, safeDepthScatter * 0.5f) * safeSpacing;

        return new Vector3(x, 0f, z);
    }

    private int GetRowWidth(int row, int firstLineCount, int lineStep, int maxPerLine)
    {
        int candidate = firstLineCount + row * lineStep;
        return Mathf.Clamp(candidate, 1, maxPerLine);
    }
}
