using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using TMPro;
using UnityEngine.UI;
using MySql.Data;
using MySql.Data.MySqlClient;

public class adminLogin : MonoBehaviour
{
    public static MySqlConnection SqlConn;
    static string ipAddress = "183.96.251.147";
    string db_id = "root";
    string db_pw = "sm1906";
    string db_name = "timetravel";

    public TMP_InputField IDField;
    public TMP_InputField pwdField;

    public Button loginButton;
    public TMP_Text inCorrectText;
    public GameObject problemAddModifyPanel;
    // Start is called before the first frame update
    void Start()
    {
        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);
        SqlConn = new MySqlConnection(strConn);
        SqlConn.Open();
        inCorrectText.gameObject.SetActive(false);
        loginButton.onClick.AddListener(loginToSQLServer);
    }

    void loginToSQLServer()
    {
        string id = IDField.text;
        string pwd = pwdField.text;
        string loginQuery = string.Format("select * from admin where adminID='{0}' and adminPWD='{1}';", id, pwd);
        DataTable queryData = selectRequest(loginQuery);
        if (queryData == null)
        {
            return;
        }
        else if (queryData.Rows.Count == 0)
        {
            inCorrectText.gameObject.SetActive(true);
        }
        else
        {
            problemAddModifyPanel.SetActive(true);
            this.gameObject.SetActive(false);
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
            GameObject canvas = GameObject.Find("Canvas");
            GameObject canNotConnectServerPanel = canvas.transform.GetChild(19).gameObject;
            canNotConnectServerPanel.SetActive(true);
            return null;
        }
    }

}
