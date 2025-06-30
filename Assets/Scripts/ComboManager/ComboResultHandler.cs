using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboResultHandler
{
    private ComboUIRenderer comboUIRenderer; // 콤보 UI 렌더러

    public ComboResultHandler(ComboUIRenderer uiRenderer)
    {
        this.comboUIRenderer = uiRenderer; // 콤보 UI 렌더러 초기화
    }

    public void AddRune(string rune)
    {
        comboUIRenderer.AddRuneImage(rune); // 콤보 UI에 룬 이미지 추가
    }
}
