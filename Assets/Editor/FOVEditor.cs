using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        //EnemyFOV 클래스 참조
        EnemyFOV fov = (EnemyFOV)target;

        //원주 위의 시작점의 좌표를 계산(주어진 각도의 1/2)
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        //원의 색상을 흰색으로 지정
        Handles.color = Color.white;

        //외곽선만 표현하는 원반 그림
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRange);

        //부채꼴 색상 지정
        Handles.color = new Color(1, 1, 1, 0.2f);

        //채워진 부채꼴 그림
        Handles.DrawSolidArc(fov.transform.position, Vector3.up, fromAnglePos, fov.viewAngle, fov.viewRange);

        //시야각 텍스트로 표시
        Handles.Label(fov.transform.position + (fov.transform.forward * 2f), fov.viewAngle.ToString());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
