using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public Image[] playerList;
    public GameObject readyText;
    public int readyCounts;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerList[i].gameObject.SetActive(true);
            
        }
        //자기자신 닉네임 부여//
        CheckReadyCounts();
    }

    void CheckReadyCounts()
    {
        if(readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartGame();
        }
    }
    public void PickItems()
    {
        //아이템 뽑기
        //아이템 뽑기 완료하면 자동 레디되게
        //readyText.SetActive(true);
        //readyCounts++;
    }

    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("SampleScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Main");
    }

    
}
