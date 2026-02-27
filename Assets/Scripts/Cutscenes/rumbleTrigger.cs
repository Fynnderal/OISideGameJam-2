using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class rumbleTrigger : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] CameraShakeSettings cameraRumble;
    [SerializeField] CinemachineImpulseListener[] impulseListeners;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Collider2D _collider;
    [SerializeField] ParticleSystem _rocksParticles;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CameraController.Instance.ScreenShake(impulseSource, cameraRumble, impulseListeners);
            _audioSource.Play();
            _collider.enabled = false;
            _rocksParticles.Play();
        }
    }
}
