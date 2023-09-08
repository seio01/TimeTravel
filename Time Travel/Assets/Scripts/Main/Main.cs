using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Main : MonoBehaviour
{
    public GameObject explanationPanel;
    // Start is called before the first frame update

    private bool isSave;
    public int isBanned;
    long bannedTime;
    public TMP_Text bannedText;

    public Button Player2;
    public Button Player3;
    public Button Player4;
    public Button makeRoom;
    public Button enterRoom;

    void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
        isSave = PlayerPrefs.HasKey("isBanned");
        if (isSave == false)
        {
            PlayerPrefs.SetInt("isBanned", 0);
        }
        else
        {
            isBanned = PlayerPrefs.GetInt("isBanned");
            if (isBanned == 1)
            {
                bannedTime = long.Parse(PlayerPrefs.GetString("bannedTime"));
                bannedText.gameObject.SetActive(true);
                Player2.interactable = false;
                Player3.interactable = false;
                Player4.interactable = false;
                makeRoom.interactable = false;
                enterRoom.interactable = false;
            }
            else
            {
                Player2.interactable = true;
                Player3.interactable = true;
                Player4.interactable = false;
                makeRoom.interactable = true;
                enterRoom.interactable = true;
            }
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isBanned == 1)
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            long currentTime = (long)timeSpan.TotalSeconds;
            //확인용으로 1분, 나중에 3600초로 수정.
            if (currentTime >= bannedTime + 60)
            {
                PlayerPrefs.DeleteKey("bannedTime");
                PlayerPrefs.SetInt("isBanned", 0);
                isBanned = 0;
                bannedText.gameObject.SetActive(false);
            }
        }
    }

    public void ShowExplanation()
    {
        explanationPanel.SetActive(true);
    }

    public void gameExit()
    {
        Application.Quit();
    }
}