using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataInfo;

//에셋 메뉴를 등록하기 위한 어트리뷰트
[CreateAssetMenu(fileName = "GameDataSO"
    , menuName = "Create GameData", order = 1)]

public class GameDataObject : ScriptableObject
{
    public int killCount = 0;
    public float hp = 120f;
    public float damage = 25f;
    public float speed = 6f;
    public List<Item> equipItem = new List<Item>(); //취득 아이템
}