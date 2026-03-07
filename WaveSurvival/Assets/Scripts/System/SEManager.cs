using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    [SerializeField] AudioSource _audioSource;

    float _lastPlayTime;

    void Awake()
    {
        Instance = this;
    }

    public void PlaySE(AudioClip clip)
    {
        if (Time.time - _lastPlayTime < 0.1f)
            return;

        _audioSource.PlayOneShot(clip);
        _lastPlayTime = Time.time;
    }
}