using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    // 상수: 줌 관련 설정
    [Header("Zoom Settings")]
    float zoomSpeed = 2.0f; // 한번 줌 입력 정도(줌 스피드)
    float minZoom = 2.0f; // 최소 카메라 사이즈
    float maxZoom = 20.0f; // 최대 카메라 사이즈
    private float zoomResetSpeed = 5.0f; // 줌 리셋 속도
    private float zoomFollowSpeed = 10.0f; // 줌 따라가기 속도
    [SerializeField] private float defaultZoomSize = 9.3f; // 기본 카메라 크기

    // 변수: 줌 관련
    private float targetZoomSize; // 목표 카메라 크기
    private bool isZoomResetting = false; // 줌 리셋 중인지 여부
    
    // 컴포넌트
    public GameObject player; // 플레이어 오브젝트 (필요시 사용)
    private Camera cam; // 카메라 컴포넌트
    

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>(); // Get the main camera
        targetZoomSize = defaultZoomSize; // Initialize target zoom size
    }

    void Update()
    {
        ControlZoomSet(); // 줌 값 설정
        UpdateZoom(); // 카메라 크기 업데이트
          // 카메라 위치를 플레이어 위치에 맞춰 보간
        Vector3 targetCameraPos = new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetCameraPos, Time.deltaTime * zoomFollowSpeed);   
    }

    private void ControlZoomSet(){
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        bool hasScrollInput = Mathf.Abs(scrollInput) > Mathf.Epsilon;

        // 스크롤 입력이 있으면 줌 리셋 중지
        if(hasScrollInput) isZoomResetting = false; 
        ZoomReset(); // 줌 리셋 처리

        if (!hasScrollInput) return; //------------------------------

        // 카메라 크기를 마우스 스크롤 입력에 따라 조정
        float newSize = cam.orthographicSize - scrollInput * zoomSpeed;

        // 목표 크기를 최소/최대 범위로 제한
        targetZoomSize = Mathf.Clamp(newSize, minZoom, maxZoom); 
        // ----------------------------------------------------------
    }

    private void UpdateZoom(){
        if(Math.Abs(targetZoomSize - cam.orthographicSize) < Mathf.Epsilon) return;

        cam.orthographicSize = Mathf.MoveTowards(
            cam.orthographicSize, targetZoomSize, Time.deltaTime * zoomSpeed);
    }

    private void ZoomReset()
    {   // 휠 더블 클릭 시 줌 리셋
        if(DoubleClickFunc.IsDoubleClick(2)){
            isZoomResetting = true; // 줌 리셋 시작
        }
        if(isZoomResetting){
            // 줌 리셋 중이면 목표 크기를 기본 크기로 설정
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize, defaultZoomSize, Time.deltaTime * zoomResetSpeed);

            // 목표 크기가 기본 크기와 같아지면 줌 리셋 종료
            if(Mathf.Abs(cam.orthographicSize- defaultZoomSize)< Mathf.Epsilon){
                cam.orthographicSize = defaultZoomSize; // 정확히 기본 크기로 설정
                isZoomResetting = false; // 줌 리셋 종료
            }
        }    
    }

}
