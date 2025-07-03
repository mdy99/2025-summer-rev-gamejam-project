using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // 플레이어의 스프라이트 렌더러
    private Rigidbody2D playerRigidbody; // 플레이어의 Rigidbody2D 컴포넌트
    private LineRenderer lineRenderer; // 그래플링 훅을 위한 LineRenderer 컴포넌트
    public CoolDownDisplay coolDownDisplay; // 그래플링 훅 쿨타임 표시를 위한 CoolDownDisplay 컴포넌트

    private float moveSpeed = 5f; // 플레이어 이동 속도

    private Vector2 targetPosition; // 플레이어가 이동할 목표 위치
    private bool isMoving = false; // 플레이어가 이동 중인지 여부

    private float backgroundWidth = 80f; // 배경의 너비 (예시 값, 실제 게임에 맞게 조정 필요)
    private float backgroundHeight = 50f; // 배경의 높이 (예시 값, 실제 게임에 맞게 조정 필요)
    private float minX, minY, maxX, maxY;// 배경의 최소 x 좌표

    private bool isGrappling = false; // 그래플링 훅 사용 여부
    public float grapplingSpeed = 20f; // 그래플링 훅 이동 속도
    
    private float grappleCooldown = 1.2f; // 그래플링 훅 쿨타임
    private float lastGrappleTime = -Mathf.Infinity; // 마지막 그래플링 훅 사용 시간

    private bool isDead = false; // 플레이어가 죽었는지 여부

    Animator animator; // 플레이어의 애니메이터 컴포넌트

    void OnEnable()
    {
        BarUpdater.OnPlayerDead += Die; // 플레이어가 죽었을 때 이벤트 핸들러 등록
    }

    void OnDisable()
    {
        BarUpdater.OnPlayerDead -= Die; // 플레이어가 죽었을 때 이벤트 핸들러 등록        
    }

    public void SpeedUpForDuration(float duration, float speedMultiplier)
    {
        if(isDead) return; // 플레이어가 죽었으면 속도 증가 중지
        moveSpeed *= speedMultiplier; // 이동 속도 증가
        StartCoroutine(ResetSpeedAfterDuration(duration, speedMultiplier)); // 일정 시간 후에 속도 원래대로 되돌리기

    }

    private IEnumerator ResetSpeedAfterDuration(float duration, float speedMultiplier = 1f)
    {
        yield return new WaitForSeconds(duration); // 지정된 시간 동안 대기
        moveSpeed /= speedMultiplier; // 이동 속도를 원래대로 되돌림
    }

    void Die(){
        if(isDead) return; // 이미 죽었으면 아무 작업도 하지 않음
        Debug.Log("Player is dead!"); // 플레이어가 죽었을 때 디버그 메시지 출력
        isDead = true; // 플레이어가 죽었음을 표시
        animator.SetTrigger("isDead"); // 애니메이터의 죽음 트리거를 설정
        animator.SetBool("isMoving", false); // 이동 중이 아닐 때 애니메이터의 이동 상태를 false로 설정

        playerRigidbody.velocity = Vector2.zero; // 플레이어의 속도를 0으로 설정하여 이동 중지
        isMoving = false; // 이동 중지
        isGrappling = false; // 그래플링 훅 사용 중지
        lineRenderer.positionCount = 0; // LineRenderer 비활성화
    }

    void Start()
    {
        animator = GetComponent<Animator>(); // 플레이어의 애니메이터 컴포넌트를 가져옴
        playerRigidbody = GetComponent<Rigidbody2D>(); // 플레이어의 Rigidbody2D 컴포넌트를 가져옴
        spriteRenderer = GetComponent<SpriteRenderer>(); // 플레이어의 스프라이트 렌더러를 가져옴
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        // 배경의 최소/최대 좌표 계산
        minX = -backgroundWidth / 2f; // 배경의 최소 x 좌표
        maxX = backgroundWidth / 2f; // 배경의 최대 x 좌표
        minY = -backgroundHeight / 2f; // 배경의 최소 y 좌표
        maxY = backgroundHeight / 2f; // 배경의 최대 y 좌표
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead) return; // 플레이어가 죽었으면 업데이트 중지
        if(WaveManager.Instance.CurrentState == WaveState.RewardTime) return;        

        if(Input.GetMouseButton(1) && !isGrappling) // 마우스 우클릭
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z); // 카메라의 z축 위치를 사용하여 2D 공간으로 변환
            targetPosition = Camera.main.ScreenToWorldPoint(mousePos); // 마우스 위치를 월드 좌표로 변환
            // 목표 위치가 배경의 범위를 벗어나지 않도록 제한
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX); // x축은 배경의 최소/최대 x 좌표로 제한
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY); // y축은 배경의 최소/최대 y 좌표로 제한

            isMoving = true; // 이동 시작
        }
        if(Input.GetKeyDown(KeyCode.Space)&& isMoving && Time.time - lastGrappleTime > grappleCooldown){
            if(!isGrappling){
                isGrappling = true; // 그래플링 훅 사용 시작
                lastGrappleTime = Time.time; // 그래플링 훅 사용 시간 기록
                coolDownDisplay.StartCoolDown(grappleCooldown); // 그래플링 훅 쿨타임 시작
                animator.SetBool("isGrappling", true); // 애니메이터의 그래플링 훅 상태를 true로 설정
            }
            else{
                isGrappling = false; // 그래플링 훅 사용 중지
            }
        }

    }

    void FixedUpdate()
    {
        if(isDead) return; // 플레이어가 죽었으면 업데이트 중지
        if(!isMoving) {
            animator.SetBool("isMoving", false); // 이동 중이 아닐 때 애니메이터의 이동 상태를 false로 설정
            return;
        } // 이동 중이 아닐 때는 아무 작업도 하지 않음

        Vector2 currentPosition = playerRigidbody.position; // 현재 플레이어 위치
        Vector2 direction = (targetPosition - currentPosition).normalized; // 목표 위치로 향하는 방향 벡터

        float currentSpeed = isGrappling ? grapplingSpeed : moveSpeed; // 그래플링 훅 사용 중이면 그래플링 속도, 아니면 일반 이동 속도
        Vector2 newPosition = currentPosition + direction * currentSpeed * Time.fixedDeltaTime; // 새로운 위치 계산

        if(isGrappling)
        {
            // 그래플링 훅을 사용 중일 때는 LineRenderer를 사용하여 그래플링 훅을 시각적으로 표시
            lineRenderer.positionCount = 2; // 두 점으로 구성된 선
            lineRenderer.SetPosition(0, currentPosition); // 시작점은 현재 플레이어 위치
            lineRenderer.SetPosition(1, targetPosition); // 끝점은 목표 위치
        }
        else
        {
            lineRenderer.positionCount = 0; // 그래플링 훅을 사용하지 않을 때는 LineRenderer를 비활성화
        }

        if(Vector2.Distance(newPosition, targetPosition) < 0.2f)
        {
            playerRigidbody.MovePosition(targetPosition); // 목표 위치에 도달하면 정확히 이동
            isMoving = false; // 이동 완료
            isGrappling = false; // 그래플링 훅 사용 중지
            animator.SetBool("isGrappling", false); // 애니메이터의 그래플링 훅 상태를 false로 설정
            playerRigidbody.velocity = Vector2.zero; // 속도를 0으로 설정하여 이동 중지
            animator.SetBool("isMoving", false); // 이동 중이 아닐 때 애니메이터의 이동 상태를 false로 설정
            return;
        }
        playerRigidbody.MovePosition(newPosition); // 플레이어 위치 업데이트
        // 이동 중인데 그래플링이 아님 → 걷는 애니메이션
        if (isMoving && !isGrappling)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    void LateUpdate()
    {
        if(isDead) return; // 플레이어가 죽었으면 업데이트 중지
        spriteRenderer.flipX = targetPosition.x > playerRigidbody.position.x; // 목표 위치에 따라 스프라이트 방향을 뒤집음
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(isDead) return; // 플레이어가 죽었으면 충돌 처리 중지
        if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Obstacle"))
        {
            isMoving = false; // 장애물에 충돌하면 이동 중지
            isGrappling = false; // 그래플링 훅 사용 중지
            playerRigidbody.velocity = Vector2.zero; // 속도를 0으로 설정하여 이동 중지
            lineRenderer.positionCount = 0; // LineRenderer 비활성화
            animator.SetBool("isMoving", false); // 이동 중이 아닐 때 애니메이터의 이동 상태를 false로 설정
        }
    }
}
