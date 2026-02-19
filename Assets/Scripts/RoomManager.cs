using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona la secuencia de habitaciones estilo Hades (lista ordenada).
/// Al matar todos los enemigos se abre la puerta de salida.
/// Al entrar, hay fade out/in y se carga la siguiente habitación.
/// Al completar todas: muestra el FloorCompleteUI.
///
/// Setup en GameScene:
///   1. Crear un GameObject vacío llamado "RoomManager" y añadir este script.
///   2. Asignar la lista de RoomData assets en el Inspector.
///   3. Asignar los prefabs de fadePanel (CanvasGroup sobre panel negro) y floorCompletePanel.
///   4. El Player debe existir como raíz en la escena (no dentro del room prefab).
/// </summary>
public class RoomManager : MonoBehaviour
{
    [Header("Room Configuration")]
    [Tooltip("Lista ordenada de RoomData. Se cargan en secuencia.")]
    public RoomData[] roomList;

    [Header("Room Offset")]
    [Tooltip("Posición global donde instanciar cada habitación.")]
    public Vector3 roomSpawnOffset = Vector3.zero;

    [Header("Fade Transition")]
    [Tooltip("CanvasGroup que cubre la pantalla (panel negro). Se anima alpha 0→1→0.")]
    public CanvasGroup fadePanel;

    [Tooltip("Duración del fade out/in en segundos.")]
    public float fadeDuration = 0.5f;

    [Header("Floor Complete")]
    [Tooltip("Panel UI que aparece al terminar todas las habitaciones.")]
    public GameObject floorCompletePanel;

    // Singleton
    private static RoomManager _instance;
    public static RoomManager Instance => _instance;

    // Estado interno
    private int currentRoomIndex = 0;
    private GameObject currentRoomInstance;
    private Transform playerTransform;
    private PlayerMovement playerMovement;
    private Health playerHealth;
    private List<Health> activeEnemyHealths = new List<Health>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Encontrar al jugador una sola vez
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerMovement  = player.GetComponent<PlayerMovement>();
            playerHealth    = player.GetComponent<Health>();
        }
        else
        {
            Debug.LogError("RoomManager: No se encontró un GameObject con tag 'Player' en la escena.");
        }

        // Asegurarse de que el fadePanel esté invisible al inicio
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(true);
        }

        // Ocultar el panel de floor complete
        if (floorCompletePanel != null)
        {
            floorCompletePanel.SetActive(false);
        }
    }

    void Start()
    {
        // Cargar la primera habitación
        if (roomList.Length > 0)
        {
            LoadRoom(currentRoomIndex);
        }
        else
        {
            Debug.LogError("RoomManager: roomList está vacío. No hay habitaciones para cargar.");
        }
    }

    /// <summary>
    /// Instancia el room prefab, spawnea enemigos, y suscribe sus eventos OnDeath.
    /// </summary>
    void LoadRoom(int index)
    {
        if (index < 0 || index >= roomList.Length)
        {
            Debug.LogError($"RoomManager: Índice {index} fuera de rango en roomList.");
            return;
        }

        RoomData data = roomList[index];
        if (data == null || data.roomPrefab == null)
        {
            Debug.LogError($"RoomManager: RoomData o roomPrefab nulo en índice {index}.");
            return;
        }

        // Instanciar la habitación con offset configurado
        currentRoomInstance = Instantiate(data.roomPrefab, roomSpawnOffset, Quaternion.identity);
        currentRoomInstance.name = $"Room_{index}_{data.roomPrefab.name}";

        // Encontrar spawn points
        EnemySpawnPoint[] spawnPoints = currentRoomInstance.GetComponentsInChildren<EnemySpawnPoint>(true);

        // Spawnear enemigos
        if (data.enemyPrefabs != null && data.enemyPrefabs.Length > 0)
        {
            if (spawnPoints.Length == 0)
            {
                Debug.LogWarning($"RoomManager: No se encontraron EnemySpawnPoint en {data.roomPrefab.name}. " +
                                 "No se spawnearon enemigos.");
            }
            else
            {
                for (int i = 0; i < data.enemyPrefabs.Length; i++)
                {
                    if (data.enemyPrefabs[i] == null) continue;

                    // Distribución cíclica si hay más enemigos que spawn points
                    int spawnIndex = i % spawnPoints.Length;
                    Vector3 spawnPos = spawnPoints[spawnIndex].transform.position;

                    GameObject enemyGO = Instantiate(data.enemyPrefabs[i], spawnPos, Quaternion.identity);
                    enemyGO.transform.SetParent(currentRoomInstance.transform);

                    // Registrar el Health del enemigo
                    Health health = enemyGO.GetComponent<Health>();
                    if (health != null)
                    {
                        activeEnemyHealths.Add(health);
                        health.OnDeath += OnEnemyDied;
                    }
                    else
                    {
                        Debug.LogWarning($"RoomManager: El enemigo {data.enemyPrefabs[i].name} no tiene componente Health.");
                    }
                }
            }
        }

        // Si no hay enemigos, abrir la puerta inmediatamente
        if (activeEnemyHealths.Count == 0)
        {
            OpenRoomExit();
        }

        // Mover al jugador al punto de entrada
        RoomEntryPoint entryPoint = currentRoomInstance.GetComponentInChildren<RoomEntryPoint>(true);
        if (entryPoint != null && playerTransform != null)
        {
            playerTransform.position = entryPoint.transform.position;
        }
        else if (entryPoint == null)
        {
            Debug.LogWarning($"RoomManager: No se encontró RoomEntryPoint en {data.roomPrefab.name}. " +
                             "El jugador no se reubicó.");
        }
    }

    /// <summary>
    /// Callback cuando un enemigo muere.
    /// </summary>
    void OnEnemyDied()
    {
        // Contar cuántos enemigos siguen vivos (OnDeath se dispara antes del Destroy)
        int aliveCount = 0;
        foreach (Health h in activeEnemyHealths)
        {
            if (h != null && h.currentHealth > 0)
            {
                aliveCount++;
            }
        }

        // Si todos murieron, abrir la puerta de salida
        if (aliveCount == 0)
        {
            OpenRoomExit();
        }
    }

    /// <summary>
    /// Activa el RoomExit para permitir al jugador salir.
    /// </summary>
    void OpenRoomExit()
    {
        if (currentRoomInstance == null) return;

        RoomExit exit = currentRoomInstance.GetComponentInChildren<RoomExit>(true);
        if (exit != null)
        {
            exit.SetOpen(true);
        }
        else
        {
            Debug.LogWarning($"RoomManager: No se encontró RoomExit en {currentRoomInstance.name}.");
        }
    }

    /// <summary>
    /// Llamado por RoomExit cuando el jugador entra al trigger.
    /// Ejecuta la transición de habitación.
    /// </summary>
    public void GoToNextRoom()
    {
        StartCoroutine(RoomTransitionCoroutine());
    }

    /// <summary>
    /// Fade out → destruir habitación actual → cargar siguiente → fade in
    /// o mostrar panel de floor complete si no hay más habitaciones.
    /// </summary>
    IEnumerator RoomTransitionCoroutine()
    {
        // Bloquear al jugador
        if (playerMovement != null) playerMovement.lockMovement = true;
        if (playerHealth   != null) playerHealth.ChangeVulnerability(false);

        // Fade out
        yield return FadeOut();

        // Desuscribir eventos de enemigos muertos
        foreach (Health h in activeEnemyHealths)
        {
            if (h != null) h.OnDeath -= OnEnemyDied;
        }
        activeEnemyHealths.Clear();

        // Destruir la habitación actual
        if (currentRoomInstance != null)
        {
            Destroy(currentRoomInstance);
            currentRoomInstance = null;
        }

        // Avanzar al siguiente índice
        currentRoomIndex++;

        // ¿Quedan habitaciones?
        if (currentRoomIndex < roomList.Length)
        {
            // Cargar siguiente habitación
            LoadRoom(currentRoomIndex);

            // Fade in
            yield return FadeIn();

            // Desbloquear jugador
            if (playerMovement != null) playerMovement.lockMovement = false;
            if (playerHealth   != null) playerHealth.ChangeVulnerability(true);
        }
        else
        {
            // Completado el piso
            ShowFloorComplete();
        }
    }

    /// <summary>
    /// Anima fadePanel.alpha de 0 a 1.
    /// </summary>
    IEnumerator FadeOut()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 1f;
    }

    /// <summary>
    /// Anima fadePanel.alpha de 1 a 0.
    /// </summary>
    IEnumerator FadeIn()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f;
    }

    /// <summary>
    /// Muestra el panel "Floor Complete" y pausa el juego.
    /// </summary>
    void ShowFloorComplete()
    {
        if (floorCompletePanel != null)
        {
            floorCompletePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("RoomManager: floorCompletePanel no está asignado. " +
                             "El jugador completó el piso pero no hay UI de victoria.");
        }
    }
}
