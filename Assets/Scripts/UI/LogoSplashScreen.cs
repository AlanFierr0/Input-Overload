using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Script para mostrar un logo/splash screen por un tiempo determinado
/// y luego pasar automáticamente a la siguiente escena.
/// Coloca este script en cualquier GameObject de la escena del logo.
/// </summary>
public class LogoSplashScreen : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo en segundos que se mostrará el logo antes de continuar")]
    public float tiempoDeEspera = 5f;
    
    [Tooltip("Nombre de la escena a la que se cargará después (debe estar en Build Settings)")]
    public string siguienteEscena = "MainMenu";
    
    [Header("Opciones Adicionales")]
    [Tooltip("Permitir saltar la pantalla del logo presionando cualquier tecla o clic")]
    public bool permitirSaltar = true;
    
    [Tooltip("Mostrar un texto indicando que se puede saltar (opcional)")]
    public bool mostrarTextoSaltar = true;
    
    [Tooltip("Hacer fade out antes de cambiar de escena")]
    public bool usarFadeOut = false;
    
    [Tooltip("Duración del fade out en segundos")]
    public float duracionFadeOut = 1f;
    
    [Header("Referencias (Opcional)")]
    [Tooltip("Panel o imagen para hacer fade out (debe tener un CanvasGroup)")]
    public CanvasGroup panelFade;
    
    private bool transicionIniciada = false;
    private float tiempoTranscurrido = 0f;

    void Start()
    {
        // Asegurar que el juego no esté pausado
        Time.timeScale = 1f;
        
        // Ocultar el cursor del sistema para usar el crosshair
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        
        // Si hay panel de fade, asegurarse de que esté transparente al inicio
        if (panelFade != null && usarFadeOut)
        {
            panelFade.alpha = 0f;
        }
        
        Debug.Log($"LogoSplashScreen: Mostrando logo por {tiempoDeEspera} segundos");
    }

    void Update()
    {
        // Si ya se inició la transición, no hacer nada más
        if (transicionIniciada) return;
        
        // Contar el tiempo
        tiempoTranscurrido += Time.deltaTime;
        
        // Verificar si se presiona alguna tecla o clic para saltar
        if (permitirSaltar && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            Debug.Log("LogoSplashScreen: Usuario saltó la pantalla del logo");
            IniciarTransicion();
            return;
        }
        
        // Si se acabó el tiempo, iniciar transición
        if (tiempoTranscurrido >= tiempoDeEspera)
        {
            Debug.Log("LogoSplashScreen: Tiempo completado, cambiando de escena");
            IniciarTransicion();
        }
    }
    
    /// <summary>
    /// Inicia la transición a la siguiente escena
    /// </summary>
    void IniciarTransicion()
    {
        if (transicionIniciada) return;
        
        transicionIniciada = true;
        
        if (usarFadeOut && panelFade != null)
        {
            // Hacer fade out antes de cambiar de escena
            StartCoroutine(FadeOutYCambiar());
        }
        else
        {
            // Cambiar de escena directamente
            CambiarEscena();
        }
    }
    
    /// <summary>
    /// Corrutina para hacer fade out suave antes de cambiar de escena
    /// </summary>
    IEnumerator FadeOutYCambiar()
    {
        float tiempoTranscurrido = 0f;
        
        while (tiempoTranscurrido < duracionFadeOut)
        {
            tiempoTranscurrido += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, tiempoTranscurrido / duracionFadeOut);
            panelFade.alpha = alpha;
            yield return null;
        }
        
        panelFade.alpha = 1f;
        CambiarEscena();
    }
    
    /// <summary>
    /// Cambia a la siguiente escena
    /// </summary>
    void CambiarEscena()
    {
        Debug.Log($"LogoSplashScreen: Cargando escena '{siguienteEscena}'");
        SceneManager.LoadScene(siguienteEscena);
    }
    
    // Método para llamar desde un botón si quieres agregar uno
    public void SaltarLogo()
    {
        IniciarTransicion();
    }
}

