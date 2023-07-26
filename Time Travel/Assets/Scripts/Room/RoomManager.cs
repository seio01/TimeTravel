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
        Debug.Log("maxplayer" + PhotonNetwork.CurrentRoom.MaxPlayers);
        Debug.Log("nickname" + PhotonNetwork.LocalPlayer.NickName);

        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerList[i].gameObject.SetActive(true);
        }

        playerList[0].GetComponentInChildren<TMP_Text>().text = PhotonNetwork.LocalPlayer.NickName;
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)
                StartGame();
        }

        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if(playerList[i].GetComponentInChildren<TMP_Text>().text == "")
            {
                playerList[i].GetComponentInChildren<TMP_Text>().text = newPlayer.NickName;
                break;
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        
    }

    public void PickItems()
    {
        //아이템 뽑기
        //아이템 뽑기 완료하면 자동 레디되게
        readyText.SetActive(true);
        readyCounts++;
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
