using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;

public class forcedStartTimer : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text infoText;

    public GameObject forcedOutPanel;
    public float time = 60f;

    bool check;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    void OnEnable()
    {
        time = 30f;
        check = false;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        timeText.text = string.Format("{0:F0}초", time);
        infoText.text = "시간이 지나면 자동으로 시작됩니다.\n 다른 사람이 들어와 레디를 하면 시간이 갱신됩니다.";
        if (time <= 0)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount==1)
            {
                forcedOutPanel.SetActive(true);
                timeText.text = "";
                if (check == false)
                {
                    Invoke("leaveRoom", 1.5f);
                    check = true;
                }
            }
            else if (RoomManager.instance.readyCounts != PhotonNetwork.CurrentRoom.MaxPlayers && RoomManager.instance.readyCounts == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                RoomManager.instance.StartGame();
                timeText.gameObject.SetActive(false);
            }
            else
            {
                infoText.text = "마지막으로 들어온 사람이 카드 뽑기를 기다리는 중입니다.\n";
                timeText.text = "";
            }
        }
    }

    void leaveRoom()
    {
        RoomManager.instance.LeaveRoom();
    }
}
