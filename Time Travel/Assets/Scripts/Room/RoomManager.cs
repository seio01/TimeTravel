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
    public Button leaveButton;
    public int readyCounts;
    public int localPlayerIndex;

    // Start is called before the first frame update
    void Start()
    {

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log(player.NickName);
        }
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerList[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("playerList" + i + PhotonNetwork.PlayerList[i].NickName);
            playerList[i].GetComponentInChildren<TMP_Text>().text = PhotonNetwork.PlayerList[i].NickName;
                
        }
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)
                StartGame();
        }
        Debug.Log(newPlayer.NickName);

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
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (playerList[i].GetComponentInChildren<TMP_Text>().text == otherPlayer.NickName)
            {
                playerList[i].GetComponentInChildren<TMP_Text>().text = "";
                break;
            }
        }
    }

    public void PickItems()
    {
        //아이템 뽑기
        //아이템 뽑기 완료하면 자동 레디되게
        //playerList[localPlayerIndex].transform.transform.Find("Ready Text").gameObject.SetActive(true);
        readyCounts++;
        //leaveButton.interactable = false;
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
