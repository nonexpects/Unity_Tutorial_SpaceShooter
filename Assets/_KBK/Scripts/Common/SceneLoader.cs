using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Canvas Group 컴포넌트 저장할 변수
    public CanvasGroup fadeCg;
    //Fade In 처리 시간
    [Range(0.5f, 2.0f)]
    public float fadeDuration = 1f;

    //호출할 씬과 씬 로드 방식을 저장할 딕셔너리
    public Dictionary<string, LoadSceneMode> loadScenes = new Dictionary<string, LoadSceneMode>();

    //호출할 씬의 정보 설정
    void InitSceneInfo()
    {
        //호출할 씬의 정보를 딕셔너리에 추가
        loadScenes.Add("Level1", LoadSceneMode.Additive);
        loadScenes.Add("Play", LoadSceneMode.Additive);
    }

    //코루틴으로 Start함수 호출
    IEnumerator Start()
    {
        InitSceneInfo();

        //처음 알파값 설정
        fadeCg.alpha = 1f;

        //여러개 씬을 코루틴으로 호출
        foreach(var _loadScene in loadScenes)
        {
            yield return StartCoroutine(LoadScene(_loadScene.Key, _loadScene.Value));
        }

        //Fade In 함수 호출
        StartCoroutine(Fade(0f));
    }

    IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        //비동기 방식으로 씬 로드하고 로드 완료될 때까지 대기
        yield return SceneManager.LoadSceneAsync(sceneName, mode);

        //호출된 씬 활성화
        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(loadedScene);
    }

    //Fade In/Out 시키는 함수
    IEnumerator Fade(float finalAlpha)
    {
        //라이트맵 깨지는거 방지 위해 스테이지 씬 활성화
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level1"));
        fadeCg.blocksRaycasts = true;

        //절대값 함수로 백분율 계산
        float fadeSpeed = Mathf.Abs(fadeCg.alpha - finalAlpha)/fadeDuration;

        //알파값 조정
        while(!Mathf.Approximately(fadeCg.alpha, finalAlpha))
        {
            //MoveTowards 함수는 Lerp함수와 동일한 함수로 알파값 보간함
            fadeCg.alpha = Mathf.MoveTowards(fadeCg.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            yield return null;
        }

        fadeCg.blocksRaycasts = false;

        //Fade In이 완료된 후 SceneLoader 씬은 삭제(Unload)
        SceneManager.UnloadSceneAsync("SceneLoader");

    }
}
