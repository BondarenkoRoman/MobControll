using DG.Tweening;
using TMPro;
using UnityEngine;

public class TMPScaleAppearTween : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private bool _playOnEnable = true;

    [SerializeField] private float _fromScale = 0f;
    [SerializeField] private float _midScale = 0.9f;
    [SerializeField] private float _toScale = 1f;

    [SerializeField] [Min(0f)] private float _firstDuration = 1f;
    [SerializeField] [Min(0f)] private float _secondDuration = 1f;

    [SerializeField] private Ease _firstEase = Ease.OutBack;
    [SerializeField] private Ease _secondEase = Ease.OutQuad;

    private Sequence _sequence;

    private Transform Target => _target != null ? _target : transform;

    private void OnEnable()
    {
        if (_playOnEnable)
            Play();
    }

    private void OnDisable()
    {
        Kill();
    }

    public void Play()
    {
        Kill();

        var tr = Target;
        tr.localScale = Vector3.one * _fromScale;

        _sequence = DOTween.Sequence();
        _sequence.Append(tr.DOScale(_midScale, _firstDuration).SetEase(_firstEase));
        _sequence.Append(tr.DOScale(_toScale, _secondDuration).SetEase(_secondEase));
    }

    public void Kill()
    {
        if (_sequence != null && _sequence.IsActive())
            _sequence.Kill();

        _sequence = null;
    }
}
