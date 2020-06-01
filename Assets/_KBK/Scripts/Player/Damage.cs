using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    //생명게이지
    private float initHp = 100f;
    public float currHp;
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
        Debug.Log("Player Die!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
