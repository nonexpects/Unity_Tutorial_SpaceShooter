using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";
    //생명게이지
    private float initHp = 100f;
    public float currHp;
    //BloodScreen 텍스쳐를 저장하기 위한 변수
    public Image bloodScreen;

    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    //HP Bar Image를 저장하기 위한 변수
    public Image hpBar;

    //생명 게이지의 처음 색상(녹색)
    private readonly Color initColor = new Vector4(0, 1.0f, 0f, 1f);
    private Color currColor;

    void Start()
    {
        currHp = initHp;

        //생명 게이지의 초기 색상 설정
        hpBar.color = initColor;
        currColor = initColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == bulletTag)
        {
            Destroy(other.gameObject);

            //혈흔 효과 표현할 코루틴 함수 호출
            StartCoroutine(ShowBloodScreen());
            currHp -= 0.5f;
            Debug.Log("Player HP = " + currHp.ToString());

            //생명 게이지의 색상 및 크기 변경 함수 호출
            DisplayHpBar();

            if(currHp <= 0)
            {
                PlayerDie();
            }
        }
    }

    private void DisplayHpBar()
    {
        if ((currHp / initHp) > 0.5f)
        {
            currColor.r = (1 - (currHp / initHp)) * 2f;
        }
        else // 생명수치가 0%일 때는 노란색에서 빨간색으로 변경
            currColor.g = (currHp / initHp) * 2f;

        //HPBar 색상 변경
        hpBar.color = currColor;
        //HPBar 크기 변경
        hpBar.fillAmount = (currHp / initHp);
    }

    private void PlayerDie()
    {
        OnPlayerDie();
        //Debug.Log("Player Die!");
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        //
        //for (int i = 0; i < enemies.Length; i++)
        //{
        //    enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
    }
    
    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스처의 알파값 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //BloodScreen 텍스처의 색상 모두 0으로 변경
        bloodScreen.color = Color.clear;

    }
}
