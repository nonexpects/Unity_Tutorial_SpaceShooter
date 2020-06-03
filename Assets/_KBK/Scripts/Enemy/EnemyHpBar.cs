using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    //Canvas를 렌더링하는 카메라
    private Camera uiCamera;
    //UI용 최상위 캔버스
    private Canvas canvas;
    //부모 RectTransform 컴포넌트
    private RectTransform rectParent;
    //자신 RectTransform 컴포넌트
    private RectTransform rectHp;

    //HPbar 이미지의 위치 조절할 오프셋
    [HideInInspector] public Vector3 offset = Vector3.zero;
    //추적할 대상의 Transform 컴포넌트
    [HideInInspector] public Transform targetTr;
    // targetTr과 offset은 EnemyDamage의 SetHpBar에서 설정함
    
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //월드 좌표를 스크린 좌표로 변환
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);
        //카메라 뒤쪽 영역(180도 회전)일 때 좌푯값 변경
        if(screenPos.z < 0f)
        {
            screenPos *= -1f;
        }
        //RectTransform 좌푯값 전달받을 변수
        var localPos = Vector2.zero;
        //스크린 좌표를 RectTransform 기준 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        //생명 게이지 위치 변경
        rectHp.localPosition = localPos;
    }
}
