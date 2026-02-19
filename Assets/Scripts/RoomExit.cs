using UnityEngine;

/// <summary>
/// Puerta de salida de la habitación.
/// Permanece cerrada hasta que RoomManager llame a SetOpen(true).
/// Al entrar el jugador, avisa a RoomManager para iniciar la transición.
///
/// Setup en el room prefab:
///   - Añadir un Collider2D con "Is Trigger" activado.
///   - (Opcional) Añadir SpriteRenderer para mostrar/ocultar un sprite de puerta.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class RoomExit : MonoBehaviour
{
    private Collider2D col;
    private SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr  = GetComponent<SpriteRenderer>();

        // Comienza cerrada
        SetOpen(false);
    }

    /// <summary>
    /// Abre o cierra la puerta activando/desactivando el collider y el sprite.
    /// </summary>
    public void SetOpen(bool open)
    {
        if (col != null) col.enabled    = open;
        if (sr  != null) sr.enabled     = open;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (RoomManager.Instance == null) return;

        // Deshabilitar para evitar que se llame múltiples veces
        SetOpen(false);
        RoomManager.Instance.GoToNextRoom();
    }
}
