using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Slider slider;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("SampleScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            time += Time.time;
            slider.value = time / 10f; //10초 후에 씬 로드
            if(time > 10)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
