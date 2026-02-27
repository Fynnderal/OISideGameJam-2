using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Playables;
using static UnityEngine.ParticleSystem;


/// <summary>
/// Implements the cutscene for the third level.
/// </summary>
public class ThirdLvlCutSceneController : MonoBehaviour
{

    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] CameraShakeSettings cameraRumble;
    [SerializeField] PlayableDirector director;
    [SerializeField] PlayerController player;
    [SerializeField] CinemachineImpulseListener[] impulseListeners;
    [SerializeField] private Lava lava;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _audioSource2;
    [SerializeField] private ParticleSystem _rocksParticles;
    [SerializeField] private ArenaController _musicTrigger;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.Input.DisablePlayerActions();
            director.Play();
            StartCoroutine(CutScene());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            return;
        }
        _audioSource.Play();
        
    }
    IEnumerator CutScene() {
        yield return new WaitForSeconds(6f);
        CameraController.Instance.ScreenShake(impulseSource, cameraRumble, impulseListeners);
        yield return new WaitForSeconds(0.5f);
        _audioSource2.Play();
        _rocksParticles.Play();
        yield return new WaitForSeconds(3f);
        player.Input.EnablePlayerActions();
        director.Stop();
        lava.Stopped = false;
        _musicTrigger.Activate();
    }
}
