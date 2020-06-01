using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;
    private int hitCount = 0;
    private Rigidbody rb;

    //찌그러진 드럼통 메쉬를 저장할 배열
    public Mesh[] meshes;
    //MeshFilter 컴포넌트를 저장할 변수
    private MeshFilter meshFilter;
    //드럼통의 텍스처를 저장할 배열
    public Texture[] textures;
    //MeshRenderer 컴포넌트 저장할 변수
    private MeshRenderer _renderer;

    //AudioSource 컴포넌트 저장할 변수
    private AudioSource _audio;

    //폭발 반경
    public float expRadius = 10f;
    // 폭발음 오디오 클립
    public AudioClip expSfx;

    //Shake 클래스 저장할 변수
    private Shake shake;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        _renderer = GetComponent<MeshRenderer>();
        // 난수 발생시켜 불규칙적인 텍스쳐 사용
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];

        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("BULLET"))
        {
            if(++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    // Update is called once per frame
    void ExpBarrel()
    {
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2f);
        //RigidBody 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 함
        rb.mass = 1f;
        // 위로 솟구치는 힘을 가함
        rb.AddForce(Vector3.up * 1000f);

        //폭발력 생성
        IndirectDamage(transform.position);

        //난수 발생
        int idx = Random.Range(0, meshes.Length);
        //찌그러진 메쉬를 적용
        meshFilter.sharedMesh = meshes[idx];

        _audio.PlayOneShot(expSfx, 1f);

        //셰이크 효과 연출
        StartCoroutine(shake.ShakeCamera(0.1f,0.2f , 0.5f));
    }

    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 11);
        foreach(var coll in colls)
        {
            var _rb = coll.GetComponent<Rigidbody>();
            //드럼통 무게 가볍게
            _rb.mass = 1f;
            //폭발력 전달
            _rb.AddExplosionForce(1200f, pos, expRadius, 1000f);
        }
    }
}
