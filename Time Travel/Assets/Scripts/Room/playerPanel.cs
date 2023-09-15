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

    public void setReadyToOther(int index)
    {
        PV.RPC("setReady", RpcTarget.AllViaServer, index);
    }


    public void setNewPlayerToReadyMe(string name)
    {
        PV.RPC("setReadyToNewPlayer", RpcTarget.Others, roomManagerScript.localPlayerIndex, name);
    }


    [PunRPC]
    void setReady(int index)
    {
        roomManagerScript.playerListImg[index].transform.transform.Find("Ready Text").gameObject.SetActive(true);
        roomManagerScript.readyCounts++;
        if (roomManagerScript.readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)  //나중에 masterClient만 start하도록 수정.
        {
            Invoke("StartGameWithTimer", 1f);
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
        if (roomManagerScript.readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)  //나중에 masterClient만 start하도록 수정.
        {
            roomManagerScript.StartGame();
        }
    }
}
