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
        PV.RPC("setReady", RpcTarget.AllBuffered, index);
    }


    public void setNewPlayerToReadyMe()
    {
        PV.RPC("setReady", RpcTarget.OthersBuffered, roomManagerScript.localPlayerIndex);
    }


    [PunRPC]
    void setReady(int index)
    {
        roomManagerScript.playerListImg[index].transform.transform.Find("Ready Text").gameObject.SetActive(true);
        roomManagerScript.readyCounts++;
        if (roomManagerScript.readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)  //나중에 masterClient만 start하도록 수정.
        {
            roomManagerScript.StartGame();
        }
    }
}
