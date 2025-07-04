using UnityEngine;

public static class SoundManagerLoader
{
    public static void EnsureExists()
    {
        if (SoundManager.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("SoundManager");
            if (prefab != null)
            {
                GameObject go = Object.Instantiate(prefab);
                go.name = "SoundManager";
            }
            else
            {
                Debug.LogError("Resources/SoundManager 프리팹이 없습니다!");
            }
        }
    }
}
