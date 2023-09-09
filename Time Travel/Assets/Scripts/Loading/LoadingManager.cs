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
        SoundManager.instance.bgmPlayer.Stop();
        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("SampleScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            yield return null;
            time += Time.deltaTime;
            if(operation.progress < 0.9f)
            {
                slider.value = Mathf.Lerp(operation.progress, 1f, time);
                if (slider.value >= operation.progress)
                    time = 0f;
            }
            else
            {
                slider.value = Mathf.Lerp(slider.value, 1f, time);
                if (slider.value >= 0.99f)
                {
                    operation.allowSceneActivation = true;
                }
            }
        }

    }

}
