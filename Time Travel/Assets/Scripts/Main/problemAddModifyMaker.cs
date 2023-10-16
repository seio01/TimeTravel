using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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
    string selection1Text;
    string selection2Text;
    string selection3Text;
    string selection4Text;
    string haveHintText;
    string hintText;
    int currentDynastyProblemNumCount = 0;

    void Awake()
    {
        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);
        SqlConn = new MySqlConnection(strConn);
    }

    void Start()
    {
        dynastySelection.onValueChanged.AddListener(delegate { setProblemNumOption();  });
        problemCategory.onValueChanged.AddListener(delegate { makeSelectionChange(); });
        addButton.onClick.AddListener(addProblemToDatabase);

        setProblemNumOption();
        
        //이후 구현해야 할 부분

        //문제 번호 dropDown 값이 바뀌었을 때, DB의 problem 테이블 problem Text의 값이 NULL이면 문제 inputField setActive()시키고 해당 문제의 이미지 보여줌
        //null이 아니면 inputField에 해당 값 읽어오기.
        //나머지 inputField와 dropDown에도 값 읽어오기.

        //추가하기 버튼 누르면 해당 값을 DB에 저장.
        //수정하기 버튼 누르면 해당 값을 DB에 저장.

    }

    void setProblemNumOption()
    {
        for (int i = 0; i < problemNum.options.Count; i++)
        {
            problemNum.options.RemoveAt(0);
        }
        TMP_Text selectedLabel = problemNum.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        string query = setQuery();
        DataTable dynasty = selectRequest(query);
        currentDynastyProblemNumCount = dynasty.Rows.Count;

        if (addProblemButton.interactable== false)
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
        }
        problemNum.value = 0;
    }

    void addOptionAtProblemNum(string valueText)
    {
        TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
        newData.text = valueText;
        problemNum.options.Add(newData);
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
            string insertQuery = string.Format("insert into problem values({0}, {1}, {2}, {3}, {4}) ;", dynastyText, problemNumText, problemText, haveHintText, hintText);
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
        problemText = addedProblem.text;
        selection1Text = selection1.text;
        selection2Text = selection2.text;
        selection3Text = selection3.text;
        selection4Text = selection4.text;
        haveHintText = haveHint.options[haveHint.value].text;
        hintText = hint.text;
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
            SqlConn.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = query;


            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            SqlConn.Close();

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
            SqlConn.Open();
            sqlCommand.ExecuteNonQuery();
            SqlConn.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
