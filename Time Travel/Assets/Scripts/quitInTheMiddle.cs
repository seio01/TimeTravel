using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using System;
using UnityEngine.UI;

public class quitInTheMiddle : MonoBehaviourPunCallbacks
{
    public static quitInTheMiddle instance;
    public TMP_Text testTMP;

    public List<int>outPlayerIndex;
    public Dice diceScript;
    public problem problemScript;
    public GameObject endGameText;
    public GameObject endPanel;

    public GameObject MiddleQuitPanel;
    public Button YesButton;
    public Button NoButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        outPlayerIndex = new List<int>();
    }

    void Start()
    {
        YesButton.onClick.AddListener(gameQuit);
        NoButton.onClick.AddListener(no);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnApplicationQuit();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            GameManager.instance.isOver = true;
            stopCoroutinesAndSetActiveFalse();
            GameManager.instance.isOver = true;
            //���� �÷��̾� �г���
            endPanel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList[0].NickName + " ��\n�¸��� �����մϴ�!";
            StartCoroutine(endGame());
            return;
        }
        if (otherPlayer == GameManager.instance.controlPlayer)
        {
            int index = GameManager.instance.controlPlayerIndexWithOrder;
            if (GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].movingAllowed == true)
            {
                outPlayerIndex.Add(index);
            }
            else
            {
                StartCoroutine(UISmallerRoutine(index));
                outPlayerIndex.Add(index);
                changeControlPlayerIndex();
                stopCoroutinesAndSetActiveFalse();
                GameManager.instance.Invoke("RoundStart", 1);
            }
        }
        else
        {
            for (int i = 0; i < DontDestroyObjects.instance.playerListWithOrder.Count; i++)
            {
                if (otherPlayer.NickName == DontDestroyObjects.instance.playerListWithOrder[i].NickName)
                {
                    outPlayerIndex.Add(i);
                    break;
                }
            }
        }
        GameManager.instance.testTMP.text = ""; 
        for (int i = 0; i < outPlayerIndex.Count; i++)
        {
            GameManager.instance.testTMP.text = outPlayerIndex[i]+"  ";
        }
    }

    IEnumerator UISmallerRoutine(int index)
    {
        float time = 0f;
        bool smaller = true;
        while (smaller)
        {
            GameManager.instance.playerInformationUIs[index].transform.localScale = Vector3.one * (1.3f - time);
            time += Time.deltaTime;
            if (time > 0.3f)
            {
                smaller = false;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator endGame()
    {
        SoundManager.instance.SoundPlayer("ShowPanel1");
        endGameText.transform.GetChild(0).GetComponent<TMP_Text>().text = "��� �÷��̾ ������ �ߴ��Ͽ� �ڵ����� ����˴ϴ�.\n";
        endGameText.SetActive(true);

        yield return new WaitForSeconds(2f);

        endGameText.SetActive(false);
        endPanel.SetActive(true);
        SoundManager.instance.SoundPlayer("Finish");
    }

    public void changeControlPlayerIndex()
    {
        GameManager.instance.controlPlayerIndexWithOrder++;
        if (GameManager.instance.controlPlayerIndexWithOrder == GameManager.instance.initialPlayerNum)
        {
            GameManager.instance.controlPlayerIndexWithOrder = 0;
        }
        GameManager.instance.controlPlayer = DontDestroyObjects.instance.playerListWithOrder[GameManager.instance.controlPlayerIndexWithOrder];
    }

    void stopCoroutinesAndSetActiveFalse()
    {
        diceScript.StopAllCoroutines();
        problemScript.StopAllCoroutines();
        GameManager.instance.StopAllCoroutines();
        GameManager.instance.startRoundPanel.SetActive(false);
        GameManager.instance.itemUsePanel.SetActive(false);
        GameManager.instance.itemUseResultPanel.SetActive(false);
        GameManager.instance.space.SetActive(false);
        RpcManager.instance.diceImg.SetActive(false);
        RpcManager.instance.diceTimer.gameObject.SetActive(false);
    }

    void OnApplicationQuit()
    {
        if (GameManager.instance.isOver == false)
        {
            MiddleQuitPanel.gameObject.SetActive(true);
        }
        else
        {
            return;
        }
    }

    void gameQuit()
    {
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        string bannedTime = ((long)timeSpan.TotalSeconds).ToString();
        PlayerPrefs.SetInt("isBanned", 1);
        PlayerPrefs.SetString("bannedTime", bannedTime);
        Application.Quit();
    }

    void no()
    {
        MiddleQuitPanel.gameObject.SetActive(false);
    }
}
