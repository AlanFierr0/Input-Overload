using UnityEngine;
using System.Collections;

/// <summary>
/// Gestor de audio centralizado.
/// Maneja la música de fondo (BGM) en todas las escenas.
/// Persiste entre escenas usando DontDestroyOnLoad.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmAudioSource;
    
    [Header("Settings")]
    [SerializeField] private float fadeDuration;
    [SerializeField] private float bgmVolume;

    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    private AudioClip currentBGM;
    private bool isFading = false;

    void Awake()
    {
        // Implementar Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Crear AudioSource si no existe
        if (bgmAudioSource == null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource.loop = true;
            bgmAudioSource.volume = bgmVolume;
        }
    }

    /// <summary>
    /// Reproduce música directamente sin fade
    /// </summary>
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: AudioClip es nulo");
            return;
        }

        // Si ya está reproduciendo la misma canción, no hacer nada
        if (bgmAudioSource.isPlaying && currentBGM == clip)
        {
            return;
        }

        // Detener fade si hay uno en curso
        if (isFading)
        {
            StopAllCoroutines();
            isFading = false;
        }

        currentBGM = clip;
        bgmAudioSource.loop = loop;
        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = bgmVolume;
        bgmAudioSource.Play();
    }

    /// <summary>
    /// Detiene la música directamente
    /// </summary>
    public void StopBGM()
    {
        if (isFading)
        {
            StopAllCoroutines();
            isFading = false;
        }

        bgmAudioSource.Stop();
    }

    /// <summary>
    /// Pausa la música
    /// </summary>
    public void PauseBGM()
    {
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
        }
    }

    /// <summary>
    /// Reanuda la música
    /// </summary>
    public void ResumeBGM()
    {
        if (!bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }
    }

    /// <summary>
    /// Cambia el volumen de la música de fondo
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume;
        }
    }

}
