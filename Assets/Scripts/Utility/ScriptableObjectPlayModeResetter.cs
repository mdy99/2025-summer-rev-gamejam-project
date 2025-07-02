#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[InitializeOnLoad]
public static class ScriptableObjectPlayModeResetter
{
    private static Dictionary<ScriptableObject, string> backups = new();

    static ScriptableObjectPlayModeResetter()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    public static void Register(ScriptableObject obj)
    {
        if (obj == null || backups.ContainsKey(obj)) return;

        string json = JsonUtility.ToJson(obj);
        backups[obj] = json;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // 자동 등록 예시: 프로젝트에 있는 모든 RuneInfoDatabase 등록
            var allInstances = AssetDatabase.FindAssets("t:RuneInfoDatabase");
            foreach (var guid in allInstances)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                Register(asset);
            }
        }

        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            foreach (var kv in backups)
            {
                JsonUtility.FromJsonOverwrite(kv.Value, kv.Key);
                EditorUtility.SetDirty(kv.Key); // 변경 내용 표시
            }
            AssetDatabase.SaveAssets(); // 저장
            backups.Clear();
        }
    }
}
#endif
