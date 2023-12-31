using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyTimer : MonoBehaviour
{
    public RoomManager rmanager;
    public TMP_Text timeText;
    public float time;

    public bool timerSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        time = 15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!rmanager.setTimer)
            return;
        time -= Time.deltaTime;
        timeText.text = string.Format("{0:F0}��", time);
        if (time <= 5)
        {
            if (!timerSound)
            {
                timerSound = true;
                SoundManager.instance.SoundPlayer("5Timer");
            }
            
        }

        if (time <= 0)
        {
            timerSound = false;
            rmanager.infoText.SetActive(false);
            rmanager.setTimer = false;
            rmanager.LeaveRoom();
        }
        

    }


}
