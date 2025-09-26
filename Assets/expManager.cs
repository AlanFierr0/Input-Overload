using UnityEngine;

public class expManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve expCurve;
    public int currentLvl;
    public int totalExp;
    public int prevLvlExp;
    public int nextLvlExp;
    public AbilityManager abilityManager;
    public int testExpGain = 50;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GainExp(testExpGain);
            Debug.Log("Gained " + testExpGain + " EXP. Total EXP: " + totalExp + ". Current Level: " + currentLvl);
        }
    }
    public void GainExp(int exp)
    {
        totalExp += exp;
        if (totalExp >= nextLvlExp)
        {
            LevelUp();
        }
    }
    void LevelUp()
    {
        currentLvl++;
        UpdateLvl();
        GainAbility();
    }

    void UpdateLvl()
    {
        prevLvlExp = (int)expCurve.Evaluate(currentLvl);
        nextLvlExp = (int)expCurve.Evaluate(currentLvl + 1);
    }

    void GainAbility()
    {
        if (abilityManager == null)
        {
            abilityManager = GetComponent<AbilityManager>();
        }
        abilityManager.AddAbility();
    }
}
