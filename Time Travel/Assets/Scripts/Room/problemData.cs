using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

public class problemData : MonoBehaviour
{
    public static problemData instance;
    public DataTable dynasty1;
    public DataTable dynasty2;
    public DataTable dynasty3;
    public DataTable dynasty4;
    public DataTable dynasty5;
    public DataTable answer1;
    public DataTable answer2;
    public DataTable answer3;
    public DataTable answer4;
    public DataTable answer5;
    public static MySqlConnection SqlConn;

    static string ipAddress = "183.96.251.147";
    string db_id = "root";
    string db_pw = "sm1906";
    string db_name = "timetravel";

    public GameObject canNotConnectServerPanel;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Invoke("connectServer", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static DataTable selectQuery(string query)
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
            return null;
        }
    }

    void getAllProblemAndAnswerDatas()
    {
        dynasty1 = selectQuery("select * from problem where 시대='고조선'");
        dynasty2=selectQuery("select * from problem where 시대='삼국시대'");
        dynasty3 = selectQuery("select * from problem where 시대='고려'");
        dynasty4 = selectQuery("select * from problem where 시대='조선시대'");
        dynasty5 = selectQuery("select * from problem where 시대='근대이후'");
        answer1 = selectQuery("select * from answer where 시대='고조선'");
        answer2 = selectQuery("select * from answer where 시대='삼국시대'");
        answer3 = selectQuery("select * from answer where 시대='고려'");
        answer4 = selectQuery("select * from answer where 시대='조선시대'");
        answer5 = selectQuery("select * from answer where 시대='근대이후'");
        if (dynasty1 == null)
        {
            return;
        }
    }

    void connectServer()
    {
        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);
        MySqlConnection conn = new MySqlConnection(strConn);

        SqlConn = new MySqlConnection(strConn);
        try
        {
            SqlConn.Open();
            getAllProblemAndAnswerDatas();
        }
        catch
        {
            canNotConnectServerPanel.SetActive(true);
        }
    }
}
