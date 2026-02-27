using Unity.Cinemachine;
using UnityEngine;
using KBCore.Refs;

/// <summary>
/// Main camera controller for handling Cinemachine screen shakes.
/// Provides convenience methods to configure and trigger impulse sources
/// and to stop all currently playing camera shakes.
/// </summary>
public class CameraController : ValidatedMonoBehaviour
{
    public static CameraController Instance;
    public GameObject Player;

    [SerializeField, Self] CinemachineImpulseListener _impulseListener;

    private CinemachineImpulseDefinition _impulseDefinition;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    /// <summary>
    /// Configure and trigger a screen shake using a single shared impulse listener.
    /// </summary>
    /// <param name="source">The Cinemachine impulse source to configure and fire.</param>
    /// <param name="cameraShakeSettings">ScriptableObject that defines listener and source settings for the shake.</param>
    /// <param name="resetCurrentImpulses">If true, clears all current impulses before generating the new one.</param>
    public void ScreenShake(CinemachineImpulseSource source, CameraShakeSettings cameraShakeSettings, bool resetCurrentImpulses=true)
    {
        if (resetCurrentImpulses)
            StopAllCurrentShakes();

        _impulseDefinition = source.ImpulseDefinition;

        _impulseDefinition.CustomImpulseShape = cameraShakeSettings.ImpulseCurve;
        
        source.DefaultVelocity = cameraShakeSettings.Velocity;


        _impulseDefinition.ImpulseDuration = cameraShakeSettings.DurationSource;

        _impulseListener.ReactionSettings.AmplitudeGain = cameraShakeSettings.Amplitude;    
        _impulseListener.ReactionSettings.FrequencyGain = cameraShakeSettings.Frequency;
        _impulseListener.ReactionSettings.Duration = cameraShakeSettings.DurationListener;
       
        source.GenerateImpulseWithForce(cameraShakeSettings.ImpulseForce);
    }
    /// <summary>
    /// Configure and trigger a screen shake using multiple impulse listeners.
    /// Useful when several listeners should react with the same settings (e.g., split-screen or layered cameras).
    /// </summary>
    /// <param name="source">The Cinemachine impulse source to configure and fire.</param>
    /// <param name="cameraShakeSettings">ScriptableObject that defines listener and source settings for the shake.</param>
    /// <param name="impulseListeners">Array of listeners to update with the given reaction settings.</param>
    /// <param name="resetCurrentImpulses">If true, clears all current impulses before generating the new one.</param>
    public void ScreenShake(CinemachineImpulseSource source, CameraShakeSettings cameraShakeSettings, CinemachineImpulseListener[] impulseListeners, bool resetCurrentImpulses = true)
    {
        if (resetCurrentImpulses)
            StopAllCurrentShakes();

        _impulseDefinition = source.ImpulseDefinition;

        _impulseDefinition.CustomImpulseShape = cameraShakeSettings.ImpulseCurve;

        source.DefaultVelocity = cameraShakeSettings.Velocity;


        _impulseDefinition.ImpulseDuration = cameraShakeSettings.DurationSource;
        foreach (CinemachineImpulseListener listener in impulseListeners)
        {
            listener.ReactionSettings.AmplitudeGain = cameraShakeSettings.Amplitude;
            listener.ReactionSettings.FrequencyGain = cameraShakeSettings.Frequency;
            listener.ReactionSettings.Duration = cameraShakeSettings.DurationListener;
        }

        source.GenerateImpulseWithForce(cameraShakeSettings.ImpulseForce);
    }

    public void StopAllCurrentShakes()
    {
        CinemachineImpulseManager.Instance.Clear();
    }
}
