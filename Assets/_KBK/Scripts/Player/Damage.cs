using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";
    //생명게이지
    private float initHp = 100f;
    public float currHp;

    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    void Start()
    {
        currHp = initHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == bulletTag)
        {
            Destroy(other.gameObject);

            currHp -= 0.5f;
            Debug.Log("Player HP = " + currHp.ToString());

            if(currHp <= 0)
            {
                PlayerDie();
            }
        }
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
