using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboInputHandler
{
    public void Add(string symbol) => currentCombo.Add(symbol); // 콤보에 심볼 추가
    public void Clear() {currentCombo.Clear();} // 현재 입력된 콤보 모두 제거
    public string GetCurrentCombo() => string.Join("", currentCombo); // 현재 입력된 콤보 문자열 반환
    public bool IsEmpty() => currentCombo.Count == 0; // 현재 입력된 콤보가 비어있는지 확인

    private List<string> currentCombo = new List<string>(); // 현재 입력된 콤보
    
}
