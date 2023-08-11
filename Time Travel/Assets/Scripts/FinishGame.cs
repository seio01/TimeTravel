using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class FinishGame : MonoBehaviour
{
    public PhotonView PV;

    public GameObject endPanel;
    public GameObject endGameText;
    public string winner;
    public TMP_Text winnerName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            winner = collision.gameObject.name.ToString();
            StartCoroutine(EndGameRoutine());
            
        }
    }

    public void ShowEndPanel()
    {
        PV.RPC("ShowEndPanelToOthers", RpcTarget.All, winner);
    }

    [PunRPC]
    public void ShowEndPanelToOther(string winner)
    {
        endPanel.SetActive(true);
        winnerName.text = winner + " ´Ô ½Â¸®¸¦ ÃàÇÏÇÕ´Ï´Ù!";
        GameManager.instance.isOver = true;
    }


    IEnumerator EndGameRoutine()
    {
        endGameText.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        endGameText.SetActive(false);
        ShowEndPanel();
        
    }
}
