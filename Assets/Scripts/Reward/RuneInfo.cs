using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuneInfo
{
    public string runeCode;
    public int damage;
    public int mpCost;

    private int initialDamage;
    private int initialMpCost;

    public void AddRuneInfo(int damageValue, int mpCostValue)
    {
        damage += damageValue;
        if(mpCostValue < 0 && mpCost + mpCostValue < 0)
        {
            Debug.LogWarning("MP cost cannot be negative. Setting to 0.");
            mpCost = 0; // MP cost cannot be negative
        }
        else if (mpCostValue > 0 || mpCost + mpCostValue >= 0){
            mpCost += mpCostValue;
        }
    }

    public void CacheInitialValues(){
        initialDamage = damage;
        initialMpCost = mpCost;
    }   

    public void ResetToInitialValues(){
        damage = initialDamage;
        mpCost = initialMpCost;
    }

}
