using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Módulo de input personalizado que funciona incluso cuando Time.timeScale = 0
/// Procesa hover, clicks y movimiento del mouse correctamente cuando el juego está pausado
/// </summary>
public class UnscaledTimeInputModule : StandaloneInputModule
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void UpdateModule()
    {
        // SIEMPRE actualizar el módulo, incluso con tiempo pausado
        base.UpdateModule();
    }

    public override void Process()
    {
        // SIEMPRE procesar eventos, incluso con tiempo pausado
        base.Process();
    }

    public override bool IsModuleSupported()
    {
        return true;
    }

    public override bool ShouldActivateModule()
    {
        // SIEMPRE activar el módulo
        return true;
    }
    
    public override void ActivateModule()
    {
        base.ActivateModule();
        
        // Mantener el cursor oculto para usar el crosshair personalizado
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
    }
}

