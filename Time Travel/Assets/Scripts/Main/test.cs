using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using MySql.Data; 
using MySql.Data.MySqlClient;
public class test : MonoBehaviour
{
    public static MySqlConnection SqlConn;

    static string ipAddress= "183.96.251.147";
    string db_id = "root";
    string db_pw = "sm1906";
    string db_name = "timetravel";
    // Start is called before the first frame update
    void Start()
    {
        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress , db_id, db_pw, db_name);
        MySqlConnection conn = new MySqlConnection(strConn);

        SqlConn=new MySqlConnection(strConn);
        string query = "insert into user values('noonsong', '5678', 'admin')";
        OnInsertOrUpdateRequest(query);
        query = "select * from user";
        DataTable dataTable = OnSelectRequest(query);
        foreach (DataRow row in dataTable.Rows)
        {
            Debug.Log(row["user_id"].ToString() +"  "+ row["user_pwd"].ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static DataTable OnSelectRequest(string query)
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

    public static void OnInsertOrUpdateRequest(string query)
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
