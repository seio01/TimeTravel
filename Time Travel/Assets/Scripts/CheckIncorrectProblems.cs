using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Data;

public class CheckIncorrectProblems : MonoBehaviour
{

    public List<GameObject> pages;
    public int currentPage = 0;
    public int totalPage;
    public Button leftBtn;
    public Button rightBtn;

    public GameObject noIncorrectText;

    public problem problemScript;
    public problemGraph problemGraphScript;

    public GameObject problemPrefab;
    public GameObject selection1;
    public GameObject selection2;
    public GameObject selection3;
    public GameObject selection4;

    public Sprite problemImg;
    public Canvas incorrectProblemPanel;

    DataRow currentProblem;
    DataRow currentAnswer;

    /*string dynasty;
    string problemType;*/
    int problemID;


    void Awake()
    {
        //problemScript = problem.instance;
        problemGraphScript = problem.instance.problemScript;

    }


    void Start()
    {
        
        leftBtn.onClick.AddListener(() => { PrevPage(); });
        rightBtn.onClick.AddListener(() => { NextPage(); });
    }

    public void PrevPage()
    {
        SoundManager.instance.SoundPlayer("FlipPage");

        if (currentPage == 0)
        {
            return;
        }

        pages[currentPage].SetActive(false);
        currentPage--;
        pages[currentPage].SetActive(true);
        if (currentPage == 0)
        {
            leftBtn.interactable = false;
            rightBtn.interactable = true;
        }
        else
        {
            rightBtn.interactable = true;
            leftBtn.interactable = true;
        }

    }

    public void NextPage()
    {
        SoundManager.instance.SoundPlayer("FlipPage");
        if (currentPage == totalPage - 1)
        {
            return;
        }
        pages[currentPage].SetActive(false);
        currentPage++;
        pages[currentPage].SetActive(true);
        if (currentPage == totalPage - 1)
        {
            rightBtn.interactable = false;
            leftBtn.interactable = true;
        }
        else
        {
            rightBtn.interactable = true;
            leftBtn.interactable = true;
        }

    }

    public void CloseIncorrectProblemPanel()
    {
        SoundManager.instance.SoundPlayer("Button");
        incorrectProblemPanel.gameObject.SetActive(false);
    }


    public void ShowIncorrectProblems(int playerIndex)
    {
        int totalIncorrectProblems = GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty1.Count + GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty2.Count
            + GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty3.Count + GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty4.Count
            + GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty5.Count;

        if (totalIncorrectProblems == 0)
        {
            noIncorrectText.SetActive(true);
            return;
        }

        incorrectProblemPanel.gameObject.SetActive(true);
        totalPage = totalIncorrectProblems;
        if (totalPage <= 1)
        {
            leftBtn.interactable = false;
            rightBtn.interactable = false;
        }

        ShowIncorrectProblemsFromDynasty1(playerIndex);
        ShowIncorrectProblemsFromDynasty2(playerIndex);
        ShowIncorrectProblemsFromDynasty3(playerIndex);
        ShowIncorrectProblemsFromDynasty4(playerIndex);
        ShowIncorrectProblemsFromDynasty5(playerIndex);

    }

    void ShowIncorrectProblemsFromDynasty1(int playerIndex)
    {
        for(int i = 0; i < GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty1.Count; i++)
        {
            problemID = GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty1[i];
            currentProblem = problemData.instance.dynasty1.Rows[problemID - 1];
            currentAnswer = problemData.instance.answer1.Rows[problemID - 1];
            SetProblems(i);

        }
    }

    void ShowIncorrectProblemsFromDynasty2(int playerIndex)
    {
        for (int i = 0; i < GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty2.Count; i++)
        {
            problemID = GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty2[i];
            currentProblem = problemData.instance.dynasty2.Rows[problemID - 1];
            currentAnswer = problemData.instance.answer2.Rows[problemID - 1];
            SetProblems(i);

        }
    }

    void ShowIncorrectProblemsFromDynasty3(int playerIndex)
    {
        for (int i = 0; i < GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty3.Count; i++)
        {
            problemID = GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty3[i];
            currentProblem = problemData.instance.dynasty3.Rows[problemID - 1];
            currentAnswer = problemData.instance.answer3.Rows[problemID - 1];
            SetProblems(i);

        }
    }

    void ShowIncorrectProblemsFromDynasty4(int playerIndex)
    {
        for (int i = 0; i < GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty4.Count; i++)
        {
            problemID = GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty4[i];
            currentProblem = problemData.instance.dynasty4.Rows[problemID - 1];
            currentAnswer = problemData.instance.answer4.Rows[problemID - 1];
            SetProblems(i);

        }
    }

    void ShowIncorrectProblemsFromDynasty5(int playerIndex)
    {
        for (int i = 0; i < GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty5.Count; i++)
        {
            problemID = GameManager.instance.player[playerIndex].incorrectProblemsFromDynasty5[i];
            currentProblem = problemData.instance.dynasty5.Rows[problemID - 1];
            currentAnswer = problemData.instance.answer5.Rows[problemID - 1];
            SetProblems(i);

        }
    }

    void SetProblems(int index)
    {
        string curDynasty = currentProblem["시대"].ToString();
        int curProblemID = int.Parse(currentProblem["ID"].ToString());
        GameObject problemPage = Instantiate(problemPrefab, incorrectProblemPanel.transform.GetChild(0).gameObject.transform);
        problemPage.name = curDynasty + " " + curProblemID;
        if (index != 0)
            problemPage.SetActive(false);

        problemPage.transform.GetChild(0).GetComponent<TMP_Text>().text = curDynasty;
        if (currentProblem["본문"] == DBNull.Value)
        {
            SetProblemImg(curDynasty);
            problemPage.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = problemImg;
            problemPage.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
        }
        else
        {
            problemPage.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = null;
            problemPage.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(850, 350);
            problemPage.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_InputField>().text = currentProblem["본문"].ToString();
        }
        selection1 = problemPage.transform.GetChild(2).GetChild(0).gameObject;
        selection2 = problemPage.transform.GetChild(2).GetChild(1).gameObject;
        selection1.SetActive(true);
        selection2.SetActive(true);

        if (currentProblem["유형"].ToString() == "ox")
        {
            TMP_Text selectionText = selection1.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = "o";
            CompareWithCorrectAnswer(1, selection1);
            selectionText = selection2.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = "x";
            CompareWithCorrectAnswer(2, selection2);
        }
        else
        {
            selection3 = problemPage.transform.GetChild(2).GetChild(2).gameObject;
            selection4 = problemPage.transform.GetChild(2).GetChild(3).gameObject;
            selection3.SetActive(true);
            selection4.SetActive(true);
            setSelectionText(problemPage.transform.GetChild(2).GetChild(0).gameObject, "선택지1");
            CompareWithCorrectAnswer(1, selection1);
            setSelectionText(problemPage.transform.GetChild(2).GetChild(1).gameObject, "선택지2");
            CompareWithCorrectAnswer(2, selection2);
            setSelectionText(problemPage.transform.GetChild(2).GetChild(2).gameObject, "선택지3");
            CompareWithCorrectAnswer(3, selection3);
            setSelectionText(problemPage.transform.GetChild(2).GetChild(3).gameObject, "선택지4");
            CompareWithCorrectAnswer(4, selection4);
        }
        pages.Add(problemPage);
    }

    void CompareWithCorrectAnswer(int selectionNum, GameObject selection)
    {

        int correctAnswer = int.Parse(currentAnswer["답"].ToString());
        if (correctAnswer == selectionNum)
        {
            selection.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
            selection.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            selection.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.black;
            selection.transform.GetChild(1).gameObject.SetActive(false);
        }
            
    }

    void SetProblemImg(string dynasty)
    {
        Sprite[] dynastyImageGraph = null;

        switch (dynasty)
        {
            case "고조선":
                dynastyImageGraph = problemGraphScript.dynasty1;
                break;
            case "삼국시대":
                dynastyImageGraph = problemGraphScript.dynasty2;
                break;
            case "고려":
                dynastyImageGraph = problemGraphScript.dynasty3;
                break;
            case "조선시대":
                dynastyImageGraph = problemGraphScript.dynasty4;
                break;
            case "근대이후":
                dynastyImageGraph = problemGraphScript.dynasty5;
                break;
            default: break;
        }
        problemImg = dynastyImageGraph[problemID - 1];
    }

    void setSelectionText(GameObject selection, string selectionName)
    {
        TMP_Text selectionText = selection.transform.GetChild(0).GetComponent<TMP_Text>();
        selectionText.text = currentAnswer[selectionName].ToString();
    }

}
