using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseSymbol : MonoBehaviour
{
    public enum SymbolType
    {
        Dot,   // 모스 부호의 점 (.)
        Dash   // 모스 부호의 대시 (ㅡ)
    }
    public SymbolType symbolType; // 모스 부호의 종류 (점 또는 대시)
}
