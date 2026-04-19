using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraIntroTransition : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera startCamera;
    [SerializeField] private CinemachineVirtualCamera secondCamera;

    [SerializeField] [Min(0f)] private float transitionDelay = 0.5f;
    [SerializeField] [Min(0f)] private float blendDuration = 1.5f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool disableStartCameraAfterBlend = false;

    private bool _transitionStarted;
    private CinemachineBrain _brain;
    private CinemachineBlendDefinition _cachedBlend;
    private bool _hasCachedBlend;

    private void Awake()
    {
        if (startCamera != null)
            startCamera.Priority = 20;

        if (secondCamera != null)
            secondCamera.Priority = 10;
    }

    private void Start()
    {
        if (playOnStart)
            StartTransition();
    }

    [ContextMenu("Start Transition")]
    public void StartTransition()
    {
        if (_transitionStarted)
            return;

        if (startCamera == null || secondCamera == null)
        {
            Debug.LogWarning("CameraIntroTransition: Assign both cameras in inspector.", this);
            return;
        }

        _transitionStarted = true;
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        _brain = FindObjectOfType<CinemachineBrain>();
        if (_brain != null)
        {
            _cachedBlend = _brain.m_DefaultBlend;
            _hasCachedBlend = true;
            _brain.m_DefaultBlend = new CinemachineBlendDefinition(
                CinemachineBlendDefinition.Style.EaseInOut,
                blendDuration
            );
        }

        if (transitionDelay > 0f)
            yield return new WaitForSeconds(transitionDelay);

        secondCamera.Priority = startCamera.Priority + 1;

        if (blendDuration > 0f)
            yield return new WaitForSeconds(blendDuration);

        if (disableStartCameraAfterBlend)
            startCamera.gameObject.SetActive(false);

        if (_hasCachedBlend && _brain != null)
            _brain.m_DefaultBlend = _cachedBlend;
    }
}
