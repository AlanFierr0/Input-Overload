using UnityEngine;

/// <summary>
/// Define el contenido de una habitación: el prefab del mapa y los enemigos a spawnear.
///
/// Crear un asset desde:  Assets > Create > Rooms > Room Data
///
/// Notas:
///   - roomPrefab debe contener hijos con EnemySpawnPoint, RoomEntryPoint y RoomExit.
///   - enemyPrefabs[i] se spawnea en spawnPoints[i % totalSpawnPoints].
///   - Si enemyPrefabs está vacío, la puerta se abre inmediatamente.
/// </summary>
[CreateAssetMenu(fileName = "RoomData", menuName = "Rooms/Room Data")]
public class RoomData : ScriptableObject
{
    [Tooltip("Prefab de la habitación. Debe contener EnemySpawnPoint(s), un RoomEntryPoint y un RoomExit.")]
    public GameObject roomPrefab;

    [Tooltip("Posición global donde instanciar esta habitación en el mundo.")]
    public Vector3 roomPosition = Vector3.zero;

    [Tooltip("Prefabs de enemigos a spawnear. Índice i → EnemySpawnPoint i (cíclico si hay menos spawn points que enemigos).")]
    public GameObject[] enemyPrefabs;
}
