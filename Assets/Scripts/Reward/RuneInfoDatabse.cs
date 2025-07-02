using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Runes/RuneDatabase")]
public class RuneInfoDatabase : ScriptableObject
{
    public List<RuneInfo> runes; // 룬 정보 리스트

    public Dictionary<string, RuneInfo> ToDictionary()
    {
        var dict = new Dictionary<string, RuneInfo>();
        foreach (var rune in runes)
        {
            dict[rune.runeCode] = rune; // 룬 코드로 룬 정보를 매핑
        }
        return dict;
    }


}
