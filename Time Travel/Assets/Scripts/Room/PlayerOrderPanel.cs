using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;

public class PlayerOrderPanel : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public RoomManager rmanger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateListToOthers(List<int> list)
    {
        PV.RPC("UpdateList", RpcTarget.All, list);
    }

    [PunRPC]
    void UpdateList(List<int> list)
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            rmanger.orderList[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "" + (i + 1) + ". " + PhotonNetwork.PlayerList[list[i]].NickName;
            rmanger.orderList[i].gameObject.SetActive(true);
        }
    }
}
