using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class problem : MonoBehaviour
{
    public Button selection1;
    public Button selection2;
    public Button selection3;
    public Button selection4;
    public Button hintButton;
    public TMP_Text TimeText;
    public TMP_Text dynastyText;
    public GameObject problemImage;
    public GameObject resultPanel;
    public GameObject hintPanel;
    public GameObject problemPassButton;
    public GameObject selectionEraseButton;
    public List<Dictionary<string, object>> problemData;
    public List<Dictionary<string, object>> answerData;

    public int problemID;
    public int prevDynasty;
    string dynasty;
    string problemType;
    string isHaveHint;
    string hintString;
    TMP_Text resultText;

    public problemGraph problemScript;

    int playerPosition;
    bool isPlayerCorrect;

    bool isOtherPlayerUseTimeSteal;

    bool usePassItem;

    public PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        resultText = resultPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        problemID = 1;
        prevDynasty = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnEnable()
    {
        usePassItem = false;
        getPlayerNextPosition();
        resultText = resultPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        Debug.Log(playerPosition);
        if (GameManager.instance.controlPlayer == PhotonNetwork.LocalPlayer)
        {
            RpcManager.instance.setProblemID(playerPosition);
        }
    }

    void OnDisable()
    {
        hintPanel.SetActive(false);
        GameManager.instance.isThisTurnTimeSteal = false;
        if (isPlayerCorrect == true)
        {
            if (GameManager.instance.isUsedBind == true)
            {
                GameManager.instance.isMovableWithBind = true;
            }
            GameManager.instance.MovePlayer();

            if (!usePassItem) //패스했을땐 correctcount증가x
            {
                GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].correctCount++;
                GameManager.instance.UpdateGaugeImg();
            }


        }
        else
        {
            GameManager.instance.finishRound = true;
            GameManager.instance.UISmaller();
        }

    }

    public void setProblemPanel(int problemID, int prevDynasty)
    {
        this.problemID = problemID;
        this.prevDynasty = prevDynasty;
        getInfoFromCSV();
        setImage();
        controlButtons();
        GameManager.instance.testTMP.text = answerData[problemID - 1]["답"].ToString();
        if (problemType == "ox")
        {
            if (GameManager.instance.isThisTurnTimeSteal == true)
            {
                StartCoroutine("setTimer", 8);
            }
            else
            {
                StartCoroutine("setTimer", 15);
            }
        }
        else
        {
            if (GameManager.instance.isThisTurnTimeSteal == true)
            {
                StartCoroutine("setTimer", 15);
            }
            else
            {
                StartCoroutine("setTimer", 30);
            }
        }
    }

    IEnumerator setTimer(int time)
    {
        while (time >= 0)
        {
            TimeText.text = "남은 시간: " + time.ToString() + "초";
            time -= 1;
            yield return new WaitForSeconds(1.0f);
            if (time == 5)
                SoundManager.instance.SoundPlayer("5Timer");
        }
        resultText.text = "틀렸습니다...";
        isPlayerCorrect = false;
        resultPanel.SetActive(true);
    }

    void getInfoFromCSV()
    {
        dynasty = problemData[problemID - 1]["시대"].ToString();
        problemType = problemData[problemID - 1]["유형"].ToString();
        isHaveHint = problemData[problemID - 1]["힌트 여부"].ToString();
        hintString = problemData[problemID - 1]["힌트"].ToString();
        dynastyText.text = dynasty;
    }

    void setImage()
    {
        string graphName = dynasty + problemID;
        Sprite[] dynastyImageGraph = null;
        switch (dynasty)
        {
            case "고조선":
                dynastyImageGraph = problemScript.dynasty1;
                break;
            case "삼국시대":
                dynastyImageGraph = problemScript.dynasty2;
                break;
            case "고려":
                dynastyImageGraph = problemScript.dynasty3;
                break;
            case "조선시대":
                dynastyImageGraph = problemScript.dynasty4;
                break;
            case "근대이후":
                dynastyImageGraph = problemScript.dynasty5;
                break;
            default: break;
        }
        Sprite sprite = dynastyImageGraph[problemID - 1 - prevDynasty];
        problemImage.GetComponent<Image>().sprite = sprite;
        problemImage.GetComponent<Image>().SetNativeSize();
    }

    void controlButtons()
    {
        selection1.gameObject.SetActive(true);
        selection2.gameObject.SetActive(true);
        if (problemType == "ox")
        {
            selection3.gameObject.SetActive(false);
            selection4.gameObject.SetActive(false);
            TMP_Text selectionText = selection1.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = "o";
            selectionText = selection2.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = "x";
        }
        else
        {
            selection3.gameObject.SetActive(true);
            selection4.gameObject.SetActive(true);
            setSelectionText(selection1, "선택지1");
            setSelectionText(selection2, "선택지2");
            setSelectionText(selection3, "선택지3");
            setSelectionText(selection4, "선택지4");
        }
        if (isHaveHint == "o")
        {
            hintButton.gameObject.SetActive(true);
        }
        else
        {
            hintButton.gameObject.SetActive(false);
        }
        showPassButton();
        showSelectionEraseButton();
    }

    void setSelectionText(Button button, string selectionName)
    {
        TMP_Text selectionText = button.transform.GetChild(0).GetComponent<TMP_Text>();
        selectionText.text = answerData[problemID - 1][selectionName].ToString();
    }

    public void selectAnswer(int selectionNum)
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        PV.RPC("selectAnswerToOthers", RpcTarget.AllViaServer, selectionNum);
    }

    [PunRPC]
    public void selectAnswerToOthers(int selectionNum)
    {
        StopCoroutine("setTimer");
        SoundManager.instance.SoundPlayerStop();
        resultPanel.SetActive(true);
        int correctAnswer = int.Parse(answerData[problemID - 1]["답"].ToString());
        if (selectionNum == correctAnswer)
        {
            resultText.text = "정답입니다!";
            isPlayerCorrect = true;
            SoundManager.instance.SoundPlayer("Correct");
        }
        else
        {
            resultText.text = "틀렸습니다...";
            isPlayerCorrect = false;
            SoundManager.instance.SoundPlayer("Wrong");
        }
    }

    public void showPassButton()
    {
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.pass))
        {
            problemPassButton.SetActive(true);
        }
        else
        {
            problemPassButton.gameObject.SetActive(false);
        }
    }

    public void showSelectionEraseButton()
    {
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.erase) && problemType == "4지선다")
        {
            selectionEraseButton.SetActive(true);
        }
        else
        {
            selectionEraseButton.gameObject.SetActive(false);
        }
    }

    public void showHint()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        if (GameManager.instance.currentTurnASetItem == 1)
        {
            return;
        }

        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.hint))
        {
            SoundManager.instance.SoundPlayer("Button1");
            TMP_Text hintText = hintPanel.transform.GetChild(0).GetComponent<TMP_Text>();
            hintText.text = hintString;
            hintPanel.SetActive(true);
            hintButton.gameObject.SetActive(false);
            RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.hint);
            GameManager.instance.currentTurnASetItem = 1;
        }
    }

    public void eraseWrongSelection()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        SoundManager.instance.SoundPlayer("Button1");
        RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.erase);
        selectionEraseButton.gameObject.SetActive(false);
        GameManager.instance.currentTurnASetItem = 1;
        int correctAnswer = int.Parse(answerData[problemID - 1]["답"].ToString());
        int eraseSelection;
        while (true)
        {
            eraseSelection = Random.Range(1, 5);
            if (eraseSelection != correctAnswer)
            {
                break;
            }
        }
        if (eraseSelection == 1)
        {
            selection1.gameObject.SetActive(false);
        }
        else if (eraseSelection == 2)
        {
            selection2.gameObject.SetActive(false);
        }
        else if (eraseSelection == 3)
        {
            selection3.gameObject.SetActive(false);
        }
        else
        {
            selection4.gameObject.SetActive(false);
        }
    }

    public void passProblem()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        if (GameManager.instance.currentTurnASetItem == 1)
        {
            return;
        }
        SoundManager.instance.SoundPlayer("Button1");
        problemPassButton.gameObject.SetActive(false);
        RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.pass);
        GameManager.instance.currentTurnASetItem = 1;
        PV.RPC("passProblemToOthers", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void passProblemToOthers()
    {
        StopCoroutine("setTimer");
        resultPanel.SetActive(true);
        resultText.text = "문제를 패스했습니다. \n";
        string correctAnswer = answerData[problemID - 1]["답"].ToString();
        if (problemType == "ox")
        {
            if (correctAnswer == "1")
            {
                resultText.text += "정답은 o 였습니다.";
            }
            else
            {
                resultText.text += "정답은 x 였습니다.";
            }
        }
        else
        {
            resultText.text += "정답은 " + correctAnswer + "번이었습니다.";
        }
        isPlayerCorrect = true;
        usePassItem = true;
    }

    public void getPlayerNextPosition()
    {
        playerPosition = GameManager.instance.getPlayerNextPosition();
    }
}