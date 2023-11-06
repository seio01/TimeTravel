using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Main : MonoBehaviour
{
    public GameObject explanationPanel;
    public Button xButton;
    // Start is called before the first frame update

    private bool isSave;
    public int isBanned;
    long bannedTime;
    string banType;
    public TMP_Text bannedText;

    public Button Player2;
    public Button Player3;
    public Button Player4;
    public Button makeRoom;
    public Button enterRoom;

    public AudioClip mainSceneBGM;
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
                banType = PlayerPrefs.GetString("banType");
                setBannedText();
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
                Player4.interactable = true;
                makeRoom.interactable = true;
                enterRoom.interactable = true;
            }
        }
        SoundManager.instance.bgmPlayer.clip = mainSceneBGM;
        SoundManager.instance.bgmPlayer.Play();
    }

    void Start()
    {
        xButton.onClick.AddListener(checkBannedText);
    }

    // Update is called once per frame
    void Update()
    {
        if (isBanned == 1)
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            long currentTime = (long)timeSpan.TotalSeconds;

            //최종 버전때 수정하기!
            if (banType=="room"  && currentTime >= bannedTime + 600)
            {
                eraseBan();
            }
            if (banType == "game" && currentTime >= bannedTime + 3600)
            {
                eraseBan();
            }
        }
    }

    public void ShowExplanation()
    {
        bannedText.gameObject.SetActive(false);
        explanationPanel.SetActive(true);
    }

    public void gameExit()
    {
        Application.Quit();
    }

    void setBannedText()
    {
        if (banType == "room")
        {
            long banEraseTime = bannedTime + 600;
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime banEraseTimeDT = origin.AddSeconds(banEraseTime).ToLocalTime();
            bannedText.text = "이전에 게임을 종료하여 " + banEraseTimeDT + "까지\n 게임에 참여할 수 없습니다.\n";
        }
        else
        {
            long banEraseTime = bannedTime + 3600;
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime banEraseTimeDT = origin.AddSeconds(banEraseTime).ToLocalTime();
            bannedText.text = "이전에 게임을 종료하여 " + banEraseTimeDT + "까지\n 게임에 참여할 수 없습니다.\n";
        }
    }

    void eraseBan()
    {
        PlayerPrefs.DeleteKey("bannedTime");
        PlayerPrefs.DeleteKey("banType");
        PlayerPrefs.SetInt("isBanned", 0);
        isBanned = 0;
        bannedText.gameObject.SetActive(false);
    }

    void checkBannedText()
    {
        if (isBanned == 1)
        {
            bannedText.gameObject.SetActive(true);
        }
    }
}