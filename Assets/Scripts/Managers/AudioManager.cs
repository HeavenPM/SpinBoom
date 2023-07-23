using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public bool IsSoundOn => _isSoundOn;
    public bool IsMusicOn => _isMusicOn;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundEffectSource;
    [SerializeField] private AudioClip _musicSound;

    private AudioSource _currentAudioSource;
    private bool _isSoundOn = true;
    private bool _isMusicOn = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic(_musicSound, 0.3f);
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (_isSoundOn)
        {
            if (_currentAudioSource != null)
            {
                _currentAudioSource.Stop();
            }

            _currentAudioSource = gameObject.AddComponent<AudioSource>();
            _currentAudioSource.clip = clip;
            _currentAudioSource.volume = volume;
            _currentAudioSource.Play();
        }   
    }

    public void StopSound()
    {
        if (_currentAudioSource != null && _currentAudioSource.isPlaying)
        {
            _currentAudioSource.Stop();
            Destroy(_currentAudioSource);
        }
    }

    public void PlayMusic(AudioClip musicClip, float volume = 1f)
    {
        _musicSource.clip = musicClip;
        _musicSource.volume = volume;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    private void OnToggleMusicState()
    {
        _isMusicOn = !_isMusicOn;

        if (_isMusicOn) PlayMusic(_musicSound, 0.3f);
        else StopMusic();
    }

    private void OnToggleSoundState()
    {
        _isSoundOn = !_isSoundOn;
        if (!_isSoundOn) StopSound();
    }

    private void OnEnable()
    {
        EventManager.ToggleMusicState += OnToggleMusicState;
        EventManager.ToggleSoundState += OnToggleSoundState;
    }

    private void OnDisable()
    {
        EventManager.ToggleMusicState -= OnToggleMusicState;
        EventManager.ToggleSoundState -= OnToggleSoundState;
    }
}
