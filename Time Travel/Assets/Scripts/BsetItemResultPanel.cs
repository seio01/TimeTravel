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
        resultText.text = "Åë½Å Áß...\n";
        StartCoroutine(WaitFor3Seconds());
        if (RpcManager.instance.currentTurnUsedItemOfLocalPlayer != "")
        {
            RpcManager.instance.useItemOfLocalPlayer();
        }
        if (RpcManager.instance.currentTurnUsedItemOfLocalPlayer == "Ä«µå»©¾Ñ±â")
        {
            int controlPlayerCardNum = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder].Count;
            int stealCardIndex = Random.Range(0, controlPlayerCardNum);
            RpcManager.instance.cardSteal(GameManager.instance.localPlayerIndexWithOrder, stealCardIndex);
        }
    }

    IEnumerator WaitFor3Seconds()
    {
        yield return new WaitForSeconds(1.5f);
        RpcManager.instance.setResultText();
        yield return new WaitForSeconds(1.5f);
        this.gameObject.SetActive(false);
    }

}
