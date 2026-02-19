using UnityEngine;

/// <summary>
/// Define la música asociada a una habitación.
/// </summary>
[CreateAssetMenu(fileName = "New Room Music Config", menuName = "Audio/Room Music Config")]
public class RoomMusicConfig : ScriptableObject
{
    [SerializeField] public AudioClip bgmClip;
    [SerializeField] public bool loop = true;
}
