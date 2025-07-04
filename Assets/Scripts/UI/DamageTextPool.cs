using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;


public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool Instance { get; private set; } // 싱글톤 인스턴스

    public GameObject damageTextPrefab; // 피해 텍스트 프리팹
    private int poolSize = 20; // 풀의 크기

    private Queue<GameObject> damageTextPool = new Queue<GameObject>(); // 피해 텍스트 풀

    void Awake()
    {
        Instance = this; // 싱글톤 인스턴스 설정
        for(int i = 0; i < poolSize; i++) {
            GameObject damageText = Instantiate(damageTextPrefab);
            damageText.SetActive(false); // 초기에는 비활성화
            damageTextPool.Enqueue(damageText);
        }
    }

    public GameObject GetText(Vector3 position,int damage){
        GameObject obj = damageTextPool.Count > 0 ? damageTextPool.Dequeue() : Instantiate(damageTextPrefab);
        obj.transform.position = position; // 위치 설정
        obj.SetActive(true); // 활성화

        obj.GetComponent<DamageText>().Init(damage);
        return obj;
    }

    public void ReturnText(GameObject damageText)
    {
        damageText.SetActive(false); // 비활성화
        damageTextPool.Enqueue(damageText); // 풀에 반환
    }

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject damageText = Instantiate(damageTextPrefab);
            damageText.SetActive(false); // 초기에는 비활성화
            damageTextPool.Enqueue(damageText);
        }
    }

    public GameObject GetDamageText()
    {
        if (damageTextPool.Count > 0)
        {
            GameObject damageText = damageTextPool.Dequeue();
            damageText.SetActive(true);
            return damageText;
        }
        else
        {
            Debug.LogWarning("Damage Text Pool is empty! Consider increasing the pool size.");
            return null;
        }
    }

    public void ReturnDamageText(GameObject damageText)
    {
        damageText.SetActive(false);
        damageTextPool.Enqueue(damageText);
    }
}