using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    public PlayerAbilityController abilityController;

    [System.Serializable]
    public class AbilitySlot
    {
        public Ability ability;
        public KeyCode key;
    }
    public List<AbilitySlot> slots = new();

    public List<KeyCode> predefinedKeys = new()
    {
    KeyCode.B, KeyCode.C, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
    KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.T,
    KeyCode.U, KeyCode.V, KeyCode.X, KeyCode.Y, KeyCode.Z,

    KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
    KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,

    KeyCode.LeftShift, KeyCode.RightShift,
    KeyCode.LeftAlt, KeyCode.RightAlt,
    KeyCode.LeftControl, KeyCode.RightControl,
    KeyCode.Tab,

    KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6,
    KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12
    };
    public List<Ability> abilities = new();
    public void AddAbility()
    {
        if (predefinedKeys.Count == 0) return;
        var slot = new AbilitySlot();
        slot.ability = abilities[Random.Range(0, abilities.Count)];
        abilities.Remove(slot.ability);
        slot.key = predefinedKeys[Random.Range(0, predefinedKeys.Count)];
        predefinedKeys.Remove(slot.key);
        slots.Add(slot);
        Debug.Log("Gained ability: " + slot.ability.name + " assigned to key: " + slot.key);
        abilityController.slots.Add(new PlayerAbilityController.AbilitySlot { ability = slot.ability, key = slot.key });
    }
}