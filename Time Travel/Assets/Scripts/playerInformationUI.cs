using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;

public class playerInformationUI : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public TMP_Text playerPositionText;
    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        playerPositionText = transform.GetChild(2).GetComponent<TMP_Text>();
        string playerName = transform.GetChild(0).GetComponent<TMP_Text>().text;
        if (PhotonNetwork.LocalPlayer.NickName == playerName)
        {
            transform.GetChild(5).gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void updatePlayerBoardNum(int playerPosition)
    {
        PV.RPC("updatePlayerBoardNumToOther", RpcTarget.AllViaServer, playerPosition);
    }

    [PunRPC]
    void updatePlayerBoardNumToOther(int playerPosition)
    {
        RpcManager.instance.updatePlayerBoardNum(this, playerPosition);
    }
}
