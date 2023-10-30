using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

public class problemAddModifyMaker : MonoBehaviour
{
    public static MySqlConnection SqlConn;

    static string ipAddress = "183.96.251.147";
    string db_id = "root";
    string db_pw = "sm1906";
    string db_name = "timetravel";

    public TMP_Dropdown dynastySelection;
    public TMP_InputField addedProblem;
    public TMP_InputField selection1;
    public TMP_InputField selection2;
    public TMP_InputField selection3;
    public TMP_InputField selection4;
    public TMP_Dropdown problemCategory;
    public TMP_Dropdown answer;
    public TMP_Dropdown haveHint;
    public TMP_InputField hint;

    public TMP_Dropdown problemNum;

    public Button addProblemButton;
    public Button modifyProblemButton;
    public Button addButton;

    string dynastyText;
    string problemNumText;
    string problemText;
    string problemCategoryText;
    string selection1Text;
    string selection2Text;
    string selection3Text;
    string selection4Text;
    string haveHintText;
    string hintText;

    string answerText;
    int currentDynastyProblemNumCount = 0;

    void Awake()
    {
        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);
        SqlConn = new MySqlConnection(strConn);
        SqlConn.Open();
    }

    void Start()
    {
        dynastySelection.onValueChanged.AddListener(delegate { setProblemNumOption();  });
        problemCategory.onValueChanged.AddListener(delegate { makeSelectionChange(); });
        haveHint.onValueChanged.AddListener(delegate { setHintInputPanel(); });
        addButton.onClick.AddListener(addProblemToDatabase);
        addProblemButton.onClick.AddListener(setPanelToAddProblem);
        modifyProblemButton.onClick.AddListener(setPanelToModifyProblem);
        setProblemNumOption();
        //이후 구현해야 할 부분

        //<문제 수정>
        //문제 번호 dropDown 값이 바뀌었을 때, DB의 problem 테이블의 본문 column의 값이 NULL이면 문제 inputField setActive(false)시키고 해당 문제의 이미지 보여줌
        //null이 아니면 문제 text 출력하는 inputField에 값 읽어오기.
        //나머지 inputField와 dropDown에도 값 읽어오기.

        //수정하기 버튼 누르면 해당 값을 DB에 저장.

    }


    void setProblemNumOption()
    {
        int optionNum = problemNum.options.Count;
        for (int i = 0; i < optionNum; i++)
        {
            problemNum.options.RemoveAt(0);
        }
        TMP_Text selectedLabel = problemNum.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        string query = setQuery();
        DataTable dynasty = selectRequest(query);
        currentDynastyProblemNumCount = dynasty.Rows.Count;
        if (isManageMenetTypeAddProblem() == true)
        {
            problemNum.interactable = false;
            string newProblemNum = (currentDynastyProblemNumCount + 1).ToString();
            addOptionAtProblemNum(newProblemNum);

            selectedLabel.text = newProblemNum;
        }
        else
        {
            problemNum.interactable = true;
            for (int i = 1; i <= currentDynastyProblemNumCount; i++)
            {
                addOptionAtProblemNum(i.ToString());
            }
            selectedLabel.text = "1";
        }
        problemNum.value = 0;
    }

    bool isManageMenetTypeAddProblem()
    {
        if (addProblemButton.GetComponent<RectTransform>().sizeDelta == new Vector2(280, 80))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void addOptionAtProblemNum(string valueText)
    {
        TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
        newData.text = valueText;
        problemNum.options.Add(newData);
    }

    void setHintInputPanel()
    {
        if (problemCategory.value == 0)
        {
            hint.interactable = false;
        }
        else
        {
            if (haveHint.value == 0)
            {
                hint.interactable = false;
            }
            else
            {
                hint.interactable = true;
            }
        }
    }

    string setQuery()
    {
        string query;
        if (dynastySelection.value == 0)
        {
            query = "select ID from problem where 시대 = '고조선'";
        }
        else if (dynastySelection.value == 1)
        {
            query = "select ID from problem where 시대 = '삼국시대'";
        }
        else if (dynastySelection.value == 2)
        {
            query = "select ID from problem where 시대 = '고려'";
        }
        else if (dynastySelection.value == 3)
        {
            query = "select ID from problem where 시대 = '조선시대'";
        }
        else
        {
            query = "select ID from problem where 시대 = '근대이후'";
        }
        return query;
    }

    void addProblemToDatabase()
    {
        if (inputFieldAllFilled() == true)
        {
            getTextFromDropDownsAndInputFields();
            string insertQueryToProblem = string.Format("insert into problem values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}') ;", dynastyText, problemNumText, problemText, problemCategoryText, haveHintText, hintText);
            //insertOrUpdateRequest(insertQueryToProblem);
            string insertQueryToAnswer = string.Format("insert into answer values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');", dynastyText, problemNumText, selection1Text, selection2Text, selection3Text, selection4Text, answerText);
            //insertOrUpdateRequest(insertQueryToAnswer);
            setProblemNumOption();
            Debug.Log("문제 추가 성공");
        }
        else
        {
            Debug.Log("채워지지 않은 inputField가 있어 문제를 추가할 수 없습니다.\n");
        }
    }

    bool inputFieldAllFilled()
    {
        if (problemCategory.value == 0)
        {
            if (addedProblem.text != "" && selection1.text != "" && selection2.text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (addedProblem.text != "" && selection1.text != "" && selection2.text != "" && selection3.text != "" && selection4.text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    void getTextFromDropDownsAndInputFields()
    {
        dynastyText = dynastySelection.options[dynastySelection.value].text;
        problemNumText = problemNum.options[problemNum.value].text;
        problemCategoryText = problemCategory.options[problemNum.value].text;
        problemText = addedProblem.text;
        haveHintText = haveHint.options[haveHint.value].text;
        hintText = hint.text;

        selection1Text = selection1.text;
        selection2Text = selection2.text;
        selection3Text = selection3.text;
        selection4Text = selection4.text;
        answerText = (answer.value+1).ToString();
        Debug.Log(answerText);
    }

    void makeSelectionChange()
    {
        if (problemCategory.value == 0)
        {
            selection1.text = "o";
            selection2.text = "x";
            selection1.interactable = false;
            selection2.interactable = false;
            selection3.interactable = false;
            selection4.interactable = false;
            hint.interactable = false;
            haveHint.value = 0;
        }
        else
        {
            selection1.text = "";
            selection2.text = "";
            selection1.interactable = true;
            selection2.interactable = true;
            selection3.interactable = true;
            selection4.interactable = true;
        }
    }

    public static DataTable selectRequest(string query)
    {
        try
        {

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = query;


            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    public static void insertOrUpdateRequest(string query)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand(query);
            sqlCommand.Connection = SqlConn;
            sqlCommand.CommandText = query;
            sqlCommand.ExecuteNonQuery();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void setPanelToAddProblem()
    {
        addProblemButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(802, 380);
        addProblemButton.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 80);
        modifyProblemButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(832, 260);
        modifyProblemButton.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 80);
        addProblemButton.GetComponent<Image>().color = new Color(209 / 255f, 72 / 255f, 89 / 255f, 255/255f);
        modifyProblemButton.GetComponent<Image>().color = new Color(118 / 255f, 118 / 255f, 118 / 255f, 200/255f);
        setProblemNumOption();

    }

    void setPanelToModifyProblem()
    {
        addProblemButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(832, 380);
        addProblemButton.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 80);
        modifyProblemButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(802, 260);
        modifyProblemButton.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 80);
        addProblemButton.GetComponent<Image>().color = new Color(118 / 255f, 118 / 255f, 118 / 255f, 200/255f);
        modifyProblemButton.GetComponent<Image>().color = new Color(209 / 255f, 72 / 255f, 89 / 255f, 255/255f);
        setProblemNumOption();
    }
}
