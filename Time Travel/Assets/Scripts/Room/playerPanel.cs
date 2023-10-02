using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;

public class playerPanel : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public RoomManager roomManagerScript;
    public TMP_Text playerName;
    public TMP_Text MeTextObject;
    public GameObject edge;


    // Start is called before the first frame update

    void Start()
    {
        if (playerName.text == PhotonNetwork.LocalPlayer.NickName)
        {
            MeTextObject.gameObject.SetActive(true);
            edge.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setReadyToOther(string playerName)
    {
        PV.RPC("setReady", RpcTarget.AllViaServer, playerName);
    }

    [PunRPC]
    void setReady(string readyPlayerName)
    {
        if (playerName.text == readyPlayerName)
        {
            transform.GetChild(2).gameObject.SetActive(true);
            roomManagerScript.readyCounts++;
            if (roomManagerScript.readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                Invoke("StartGameWithTimer", 1f);
            }
        }

    }

    void StartGameWithTimer()
    {
        roomManagerScript.StartGame();
    }

    [PunRPC]
    void setReadyToNewPlayer(int index, string recieverNickName)
    {
        if (PhotonNetwork.LocalPlayer.NickName != recieverNickName)
        {
            return;
        }
        roomManagerScript.playerListImg[index].transform.transform.Find("Ready Text").gameObject.SetActive(true);
        roomManagerScript.readyCounts++;
        if (roomManagerScript.readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers) 
        {
            roomManagerScript.StartGame();
        }
    }
}
