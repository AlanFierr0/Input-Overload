using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Módulo de input personalizado que funciona incluso cuando Time.timeScale = 0
/// </summary>
public class UnscaledTimeInputModule : StandaloneInputModule
{
    private bool m_ForceModuleActive;

    protected override void Awake()
    {
        base.Awake();
        m_ForceModuleActive = true;
    }

    public override void UpdateModule()
    {
        // Actualizar el módulo incluso con tiempo pausado
        if (eventSystem.isFocused || m_ForceModuleActive)
        {
            base.UpdateModule();
        }
    }

    public override bool IsModuleSupported()
    {
        return true;
    }

    public override bool ShouldActivateModule()
    {
        // Activar el módulo incluso con tiempo pausado
        if (!base.ShouldActivateModule())
            return false;

        return m_ForceModuleActive || Input.GetMouseButtonDown(0) || 
               Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) ||
               Input.touchCount > 0;
    }
}

