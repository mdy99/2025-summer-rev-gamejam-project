using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuneInfo
{
    public string runeCode;
    public int damage;
    public int mpCost;

    public void AddRuneInfo(int damageValue, int mpCostValue)
    {
        damage += damageValue;
        mpCost += mpCostValue;
    }
}
