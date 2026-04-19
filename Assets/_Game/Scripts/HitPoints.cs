using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IntUnityEvent : UnityEvent<int> { }

public class HitPoints : MonoBehaviour
{
    [SerializeField] private int _defaultMaxHp = 10;
    [SerializeField] private IntUnityEvent _onCurrentChanged = new();
    [SerializeField] private UnityEvent _onDepleted = new();

    [SerializeField] TextMeshPro _hpText;

    private int _maxHp;
    private int _currentHp;

    public int Current => _currentHp;
    public int Max => _maxHp;

    public IntUnityEvent OnCurrentChanged => _onCurrentChanged;
    public UnityEvent OnDepleted => _onDepleted;

    private void Start()
    {
        Configure(_defaultMaxHp);
    }

    public void Configure(int maxHp)
    {
        _maxHp = Mathf.Max(1, maxHp);
        _currentHp = _maxHp;
        _onCurrentChanged.Invoke(_currentHp);
    }


    public void ApplyDamage(int amount)
    {
        _currentHp = Mathf.Max(0, _currentHp - amount);
        _onCurrentChanged.Invoke(_currentHp);
        if (_currentHp == 0)
            _onDepleted.Invoke();
    }

    public void SetHpText()
    {
        _hpText.text = $"{_currentHp}";
    }
}
