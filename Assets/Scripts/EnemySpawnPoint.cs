using UnityEngine;

/// <summary>
/// Marca la posición donde se spawnea un enemigo dentro de un room prefab.
/// Colocar este componente en GameObjects vacíos (sin renderer) dentro del prefab.
/// El orden en la jerarquía determina qué prefab de enemigo se asigna:
/// el primer SpawnPoint → enemyPrefabs[0], el segundo → enemyPrefabs[1], etc.
/// </summary>
public class EnemySpawnPoint : MonoBehaviour { }
