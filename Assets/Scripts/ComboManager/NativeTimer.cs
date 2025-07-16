using System.Runtime.InteropServices;
using UnityEngine;

public static class NativeTimer
{
    [DllImport("TimePluginDll")]public static extern void startTimer(); // 네이티브 타이머 시작
    [DllImport("TimePluginDll")]public static extern char getInputType(); // 네이티브 타이머 경과 시간 가져오기

    public static void StartTimer()
    {
        startTimer(); // 네이티브 타이머 시작
    }
    public static char GetInputType()
    {
        return getInputType(); // 네이티브 타이머 경과 시간 가져오기
    }
}