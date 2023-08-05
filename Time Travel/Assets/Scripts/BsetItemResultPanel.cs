using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class BsetItemResultPanel : MonoBehaviour
{
    public TMP_Text resultText;
    public PhotonView PV;
    public TMP_Text test;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        resultText.text = "";
        if (PhotonNetwork.IsMasterClient)
        {
            RpcManager.instance.removeCurrentTurnItems();
        }
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = "";
    }

    void OnEnable()
    {
        RpcManager.instance.setResultText();
        StartCoroutine(WaitFor3Seconds());
        if (RpcManager.instance.currentTurnUsedItemOfLocalPlayer != "")
        {
            RpcManager.instance.useItemOfLocalPlayer();
        }
    }

    IEnumerator WaitFor3Seconds()
    {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
    }

}
