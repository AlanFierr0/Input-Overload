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
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float bgmVolume = 0.5f;

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
    /// Reproduce música con fade in/out
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

        // Si hay fade actual, cancelarlo
        if (isFading)
        {
            StopAllCoroutines();
        }

        StartCoroutine(CrossFadeBGM(clip, loop));
    }

    /// <summary>
    /// Detiene la música con fade out
    /// </summary>
    public void StopBGM()
    {
        if (isFading)
        {
            StopAllCoroutines();
        }

        StartCoroutine(FadeOutBGM());
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

    /// <summary>
    /// Realiza crossfade entre canciones
    /// </summary>
    private IEnumerator CrossFadeBGM(AudioClip nextClip, bool loop)
    {
        isFading = true;

        // Fade out de la canción actual
        if (bgmAudioSource.isPlaying)
        {
            yield return FadeOut(bgmAudioSource, fadeDuration);
        }

        // Cambiar a nueva canción
        currentBGM = nextClip;
        bgmAudioSource.loop = loop;
        bgmAudioSource.clip = nextClip;
        bgmAudioSource.PlayOneShot(nextClip);
        bgmAudioSource.Play();

        // Fade in de la nueva canción
        yield return FadeIn(bgmAudioSource, fadeDuration);

        isFading = false;
    }

    /// <summary>
    /// Fade out de un AudioSource
    /// </summary>
    private IEnumerator FadeOutBGM()
    {
        isFading = true;
        yield return FadeOut(bgmAudioSource, fadeDuration);
        bgmAudioSource.Stop();
        isFading = false;
    }

    /// <summary>
    /// Anima fade in de un AudioSource
    /// </summary>
    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float elapsed = 0f;
        float startVolume = 0f;
        float endVolume = bgmVolume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    /// <summary>
    /// Anima fade out de un AudioSource
    /// </summary>
    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float elapsed = 0f;
        float startVolume = audioSource.volume;
        float endVolume = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = 0f;
    }
}
