using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    //셰이크 효과 줄 카메라의 Transform 저장할 변수
    public Transform shakeCamera;
    //회전시킬 것인지 판단할 변수
    public bool shakeRotate = false;
    //초기좌표와 회전값 저장할 변수
    private Vector3 originPos;
    private Quaternion originRot;

    // Start is called before the first frame update
    void Start()
    {
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f,
        float magnitudeRot = 0.1f)
    {
        //지나간 시간 누적할 변수
        float passTime = 0f;
        while(passTime < duration)
        {
            //불규칙한 위치 산출
            Vector3 shakePos = Random.insideUnitSphere;
            //카메라 위치 변경
            shakeCamera.localPosition = shakePos * magnitudePos;

            if(shakeRotate)
            {
                //불규칙한 회전값을 펄린 노이즈 함수를 이용해 추출
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0f));

                //카메라 회전값 변경
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            // 진동시간 누적
            passTime += Time.deltaTime;

            yield return null;
        }
        //진동이 끝난 후 카메라의 초깃값으로 설정
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }
}
