using UnityEngine;
using DG.Tweening;

public enum MobTeam { Player, Enemy }

public class MobEntity : MonoBehaviour
{
    [SerializeField] private MobEntityConfig mobEntityConfig;
    [SerializeField] private SplineMobMover mover;
    public MobTeam Team { get; private set; }
    public bool IsAlive { get; private set; }

    public int SpawnSession { get; private set; }

    [SerializeField] private Renderer meshRenderer;

    private bool _isDying;

    private MaterialPropertyBlock _mpb;
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        if (meshRenderer == null) meshRenderer = GetComponent<Renderer>();
    }

    public void OnSpawned(MobTeam team)
    {
        SpawnSession++;
        Team      = team;
        IsAlive   = true;
        _isDying  = false;
    }

    public void OnDespawned()
    {
        IsAlive  = false;
        _isDying = false;
        transform.DOKill();
    }

    public bool TryKill()
    {
        if (!IsAlive || _isDying) 
            return false;

        _isDying = true;
        SetColor(mobEntityConfig.DeathColor);
        DieTween();
        mover.StopFollowing();
        return true;
    }

    private void DieTween()
    {
        transform.DOKill();
        var seq = DOTween.Sequence().SetLink(gameObject);
        seq.Join(transform.DOLocalRotate(new Vector3(-90f, 0f, 0f), mobEntityConfig.DieRotationDuration,
         RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad));
        seq.Join(transform.DOMoveY(transform.position.y - 1f, mobEntityConfig.DieMoveDownDuration)
            .SetEase(Ease.OutQuad));
        seq.OnComplete(OnDeathTweenFinished);
    }

    private void OnDeathTweenFinished()
    {
        GameFactory.Instance?.ReleaseMob(this);
    }

    public void SetColor(Color color)
    {
        _mpb.SetColor(ColorID, color);
        meshRenderer.SetPropertyBlock(_mpb);
    }
}