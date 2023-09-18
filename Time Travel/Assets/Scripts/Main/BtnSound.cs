using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnSound : MonoBehaviour
{
    public bool isBGMController;
    public bool isSFXController;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { OnClick(this.gameObject); });
    }


    void OnEnable()
    {
        if (isBGMController && !SoundManager.instance.bgmSoundOn)
            this.GetComponent<Image>().sprite = SoundManager.instance.bgmOff;
        else if (isSFXController && !SoundManager.instance.sfxSoundOn)
            this.GetComponent<Image>().sprite = SoundManager.instance.sfxOff;

    }

    public void OnClick(GameObject soundBtn)
    {
        StartCoroutine(OnClickRoutine(soundBtn));
        
    }

    IEnumerator OnClickRoutine(GameObject soundBtn)
    {
        SoundManager.instance.SoundPlayer("Button");

        yield return new WaitForSeconds(0.2f);
        if (isBGMController)
            SoundManager.instance.ChangeBGMPlayerVolume(soundBtn);
        else if (isSFXController)
        {
            SoundManager.instance.ChangeSFXPlayerVolume(soundBtn);
        }
    }
}
