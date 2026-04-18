using UnityEngine;
using DG.Tweening;

public enum TransformTweenProperty
{
    LocalScale,
    LocalPosition,
    LocalEulerAngles
}

public enum TweenPlaybackMode
{
    OneWay,
    OutAndBack
}

public class TransformTweenAnimator : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private TransformTweenProperty _property = TransformTweenProperty.LocalScale;
    [SerializeField] private TweenPlaybackMode _playback = TweenPlaybackMode.OneWay;
    [SerializeField] private Vector3 _endValue = Vector3.one;
    [SerializeField] [Min(0f)] private float _duration = 0.5f;
    [SerializeField] [Min(0f)] private float _delay;
    [SerializeField] private Ease _ease = Ease.OutQuad;

    [SerializeField] [Min(0f)] private float _returnDuration;
    [SerializeField] private Ease _returnEase = Ease.InQuad;
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private bool _snapping;

    private Tween _tween;
    private Vector3 _baselineScale;
    private Vector3 _baselineLocalPosition;
    private Vector3 _baselineLocalEuler;

    private Transform Target => _target != null ? _target : transform;

    private void Awake()
    {
        CacheBaseline();
    }

    public void CacheBaseline()
    {
        var tr = Target;
        _baselineScale = tr.localScale;
        _baselineLocalPosition = tr.localPosition;
        _baselineLocalEuler = tr.localEulerAngles;
    }

    private void Start()
    {
        if (_playOnStart)
            Play();
    }

    private void OnDestroy()
    {
        KillTween();
    }

    public void Play()
    {
        KillTween();
        ApplyBaselineForAnimatedProperty();
        _tween = BuildTween();
        if (_tween == null)
            return;
        _tween.SetDelay(_delay);
        if (_playback == TweenPlaybackMode.OneWay)
            _tween.SetEase(_ease);
    }

    public void StopAndResetToBaseline()
    {
        KillTween();
        ApplyBaselineForAnimatedProperty();
    }

    private void ApplyBaselineForAnimatedProperty()
    {
        var tr = Target;
        switch (_property)
        {
            case TransformTweenProperty.LocalScale:
                tr.localScale = _baselineScale;
                break;
            case TransformTweenProperty.LocalPosition:
                tr.localPosition = _baselineLocalPosition;
                break;
            case TransformTweenProperty.LocalEulerAngles:
                tr.localEulerAngles = _baselineLocalEuler;
                break;
        }
    }

    public void KillTween()
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();
        _tween = null;
    }

    private Tween BuildTween()
    {
        if (_playback == TweenPlaybackMode.OutAndBack)
            return BuildOutAndBackSequence();
        return BuildOneWayTween();
    }

    private Tween BuildOneWayTween()
    {
        var t = Target;
        switch (_property)
        {
            case TransformTweenProperty.LocalScale:
                return t.DOScale(_endValue, _duration);
            case TransformTweenProperty.LocalPosition:
                return t.DOLocalMove(_endValue, _duration, _snapping);
            case TransformTweenProperty.LocalEulerAngles:
                return t.DOLocalRotate(_endValue, _duration, RotateMode.Fast);
            default:
                return null;
        }
    }

    private Tween BuildOutAndBackSequence()
    {
        var tr = Target;
        float backDur = _returnDuration > 0f ? _returnDuration : _duration;

        var seq = DOTween.Sequence();
        switch (_property)
        {
            case TransformTweenProperty.LocalScale:
                {
                    Vector3 start = _baselineScale;
                    seq.Append(tr.DOScale(_endValue, _duration).SetEase(_ease));
                    seq.Append(tr.DOScale(start, backDur).SetEase(_returnEase));
                    break;
                }
            case TransformTweenProperty.LocalPosition:
                {
                    Vector3 start = _baselineLocalPosition;
                    seq.Append(tr.DOLocalMove(_endValue, _duration, _snapping).SetEase(_ease));
                    seq.Append(tr.DOLocalMove(start, backDur, _snapping).SetEase(_returnEase));
                    break;
                }
            case TransformTweenProperty.LocalEulerAngles:
                {
                    Vector3 start = _baselineLocalEuler;
                    seq.Append(tr.DOLocalRotate(_endValue, _duration, RotateMode.Fast).SetEase(_ease));
                    seq.Append(tr.DOLocalRotate(start, backDur, RotateMode.Fast).SetEase(_returnEase));
                    break;
                }
            default:
                return null;
        }
        return seq;
    }
}
