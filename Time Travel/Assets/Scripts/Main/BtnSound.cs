using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnSound : MonoBehaviour
{
    public bool isVolumeContoller;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { OnClick(); });
    }
    public void CheckVolumeController()
    {
        isVolumeContoller = true;
    }

    public void OnClick()
    {
        
        SoundManager.instance.SoundPlayer("Button");
        if(isVolumeContoller)
            SoundManager.instance.ChangeBGMPlayerVolume();
    }
}
