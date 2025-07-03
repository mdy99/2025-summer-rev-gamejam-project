using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseTranslator
{
    private Dictionary<string, string> morseToRuneMap = new Dictionary<string, string>
    {
        { ".ㅡ", "A" },
        { "ㅡ...", "B" },
        { "ㅡ.ㅡ.", "C" },
        { "ㅡ..", "D" },
        { ".", "E" },
        { "..ㅡ.", "F" },
        { "ㅡㅡ.", "G" },
        { "....", "H" },
        { "..", "I" },
        { ".ㅡㅡㅡ", "J" },
        { "ㅡ.ㅡ", "K" },
        { ".ㅡ..", "L" },
        { "ㅡㅡ", "M" },
        { "ㅡ.", "N" },
        { "ㅡㅡㅡ", "O" },
        { ".ㅡㅡ.", "P" },
        { "ㅡㅡ.ㅡ", "Q" },
        { ".ㅡ.", "R" },
        { "...", "S" },
        { "ㅡ", "T" },
        { "..ㅡ", "U" },
        { "...ㅡ", "V" },
        { ".ㅡㅡ", "W" },
        { "ㅡ..ㅡ", "X" },
        { "ㅡ.ㅡㅡ", "Y" },
        { "ㅡㅡ..", "Z" },
        { "ㅡㅡㅡㅡㅡ", "0" },
        { ".ㅡㅡㅡㅡ", "1" },
        { "..ㅡㅡㅡ", "2" },
        { "...ㅡㅡ", "3" },
        { "....ㅡ", "4" },
        { ".....", "5" },
        { "ㅡ....", "6" },
        { "ㅡㅡ...", "7" },
        { "ㅡㅡㅡ..", "8" },
        { "ㅡㅡㅡㅡ.", "9" }
    };

    public string TranslateToRune(string morse){
        return morseToRuneMap.TryGetValue(morse, out string rune) ? rune : null;
    }
}
