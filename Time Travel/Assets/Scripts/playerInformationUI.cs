using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class playerInformationUI : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        string playerName = transform.GetChild(0).GetComponent<TMP_Text>().text;
        if (PV.IsMine && PhotonNetwork.LocalPlayer.NickName == playerName)
        {
            transform.GetChild(4).gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void updatePlayerBoardNuml()
    {

    }
}
