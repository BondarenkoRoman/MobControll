using UnityEngine;

public enum MobTeam { Player, Enemy }

public class MobEntity : MonoBehaviour
{
    public MobTeam Team { get; private set; }
    public bool IsAlive { get; private set; }

    [SerializeField] private Renderer meshRenderer;

    private bool _isDying;

    private Transform _t;
    private MaterialPropertyBlock _mpb;
    private static readonly int ColorID = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        _t   = transform;
        _mpb = new MaterialPropertyBlock();
        if (meshRenderer == null) meshRenderer = GetComponent<Renderer>();
    }

    // ── Pool ────────────────────────────────────────────────────

    public void OnSpawned(MobTeam team)
    {
        Team      = team;
        IsAlive   = true;
        _isDying  = false;
      //  CrowdManager.Instance.RegisterMob(this);
    }

    public void OnDespawned()
    {
        IsAlive  = false;
        _isDying = false;
   //     CrowdManager.Instance.UnregisterMob(this);
    }

    // ── Movement ────────────────────────────────────────────────

    public void MoveStep(float speed, float deltaTime)
    {
        // Враги идут в противоположном направлении
        float dir = (Team == MobTeam.Player) ? 1f : -1f;
        _t.Translate(Vector3.forward * (dir * speed * deltaTime), Space.World);
    }

    // ── Combat ──────────────────────────────────────────────────

    /// <summary>
    /// Попытка уничтожить моба. Возвращает true если смерть засчитана.
    /// Атомарная проверка через _isDying предотвращает двойной возврат в пул.
    /// </summary>
    public bool TryKill()
    {
        if (!IsAlive || _isDying) return false;
        _isDying = true;

        if (GameFactory.Instance != null)
            GameFactory.Instance.ReleaseMob(this);
        else
            gameObject.SetActive(false);

        return true;
    }

    // Оставляем для совместимости (ловушки, пропасти и т.д.)
    public void Die() => TryKill();

    // ── Visual ──────────────────────────────────────────────────

    public void SetColor(Color color)
    {
        _mpb.SetColor(ColorID, color);
        meshRenderer.SetPropertyBlock(_mpb);
    }
}