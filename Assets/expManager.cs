using UnityEngine;

public class expManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve expCurve;
    public int currentLvl;
    public int totalExp;
    public int prevLvlExp;
    public int nextLvlExp;



    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            GainExp(50);
            Debug.Log("Current Level: " + currentLvl + " Total Exp: " + totalExp + " Next Level Exp: " + nextLvlExp);
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
    }

    void UpdateLvl()
    {
        prevLvlExp = (int)expCurve.Evaluate(currentLvl);
        nextLvlExp = (int)expCurve.Evaluate(currentLvl + 1);
    }
}
