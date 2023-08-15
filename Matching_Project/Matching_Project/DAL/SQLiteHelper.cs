using HIKDataTool;
using Matching_Project;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Usman.CodeBlocks.SQLiteManager
{
    public class SQLiteHelper
    {
        static SQLiteConnection con = new SQLiteConnection();
        static SQLiteCommand cmd;
        public static string DB_NAME = "MatchingProject.db";
        public static string DB_PATH = Environment.CurrentDirectory + "\\" + DB_NAME;

        public static void Connect()
        {
            try
            {
                string connString = string.Format("Data Source={0};Version=3", DB_PATH);
                con = new SQLiteConnection(connString);
                if (con.State != ConnectionState.Open)
                    con.Open();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }


        }

        public static void DisConnect()
        {
            try
            {
                if (con != null)
                    con.Close();
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
            }
        }

        public static bool CreateTableIfNotExists()
        {
            try
            {
                if (!Directory.Exists(DB_PATH))
                    Directory.CreateDirectory(DB_PATH);

                //DB_PATH = DB_PATH + DB_NAME;

                if (!File.Exists(DB_PATH + DB_NAME))
                {
                
                    SQLiteConnection.CreateFile(DB_PATH + DB_NAME);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public static bool CreateTableIfNotExist<T>(T entity)
        {
            try
            {
                if (con.State != ConnectionState.Open || con.State == ConnectionState.Closed)
                    Connect();

                Type _type = entity.GetType();

                SQLiteCommand command = con.CreateCommand();
                //con.Open();
                command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + _type.Name + "'";
                var name = command.ExecuteScalar();

                // check account table exist or not 
                // if exist do nothing 
                if (name != null && name.ToString() == _type.Name)
                    return true;

                // table not exist, create table and insert 
                string query = "CREATE TABLE [" + _type.Name + "]";
                List<string> cols = new List<string>();
                foreach (var prop in _type.GetProperties())
                {
                    string colName = prop.Name;
                    string dataType = prop.PropertyType.Name;
                    if (SQLite.integer_types.Contains(dataType.ToLower()))
                    {
                        dataType = "INTEGER";
                        if (colName == "ID")
                            dataType += " PRIMARY KEY AUTOINCREMENT";
                    }

                    if (SQLite.decimal_types.Contains(dataType.ToLower()))
                        dataType = "FLOAT";

                    if (dataType == "String" || dataType == "string")
                    {
                        if (colName.ToLower().Contains("address"))
                            dataType = "VARCHAR(500)";
                        else
                            dataType = "VARCHAR(200)";
                    }

                    if (dataType == "DateTime")
                        dataType = "DATETIME";

                    if (dataType == "Byte" || dataType == "byte")
                        dataType = "VARBINARY(500)";

                    string line = colName + " " + dataType;
                    cols.Add(line);
                }
                string joined = string.Join(",", cols);
                query += "(" + joined + ")";
                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Globals.LogError(ex, "SQLiteHelper.CreateTableIfNotExist()", false);
                return false;
            }

        }

        public static bool ClearDatabase()
        {
            try
            {
                if (con.State != ConnectionState.Open || con.State == ConnectionState.Closed)
                    Connect();

                SQLiteCommand command = new SQLiteCommand("Select name From sqlite_master where type='table' and name not in ('sqlite_stat1','sqlite_stat4', 'sqlite_sequence') order by name;", con);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    string tableName = row[0].ToString();
                    string query = "DELETE FROM [" + tableName.ToUpper() + "]";
                    cmd = new SQLiteCommand(query, con);
                    int ret = cmd.ExecuteNonQuery();
                }

                con.Close(); // closes the connection
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static object ExecuteScalar(string query, bool secure, SQLiteConnection connection = null)
        {

            try
            {
                if (con.State != ConnectionState.Open || con.State == ConnectionState.Closed)
                    Connect();

                cmd = new SQLiteCommand(query, con);
                cmd.CommandType = System.Data.CommandType.Text;
                object obj = cmd.ExecuteScalar();

                if (secure)
                    con.Close();

                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool ExecuteScalarBoolean(string query, bool secure, SQLiteConnection connection = null)
        {

            try
            {
                if (con.State != ConnectionState.Open || con.State == ConnectionState.Closed)
                    Connect();

                cmd = new SQLiteCommand(query, con);
                cmd.CommandType = System.Data.CommandType.Text;
                object obj = cmd.ExecuteScalar();

                if (secure)
                    con.Close();

                return Convert.ToBoolean(Convert.ToInt32(obj.ToString()));
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static DataTable ExecuteTable(string query, bool secure, List<SQLiteParameter> paramsList = null, SQLiteConnection connection = null)
        {
            try
            {
                if (con.State != ConnectionState.Open || con.State == ConnectionState.Closed)
                    Connect();

                DataTable dt = new DataTable();
                cmd = new SQLiteCommand(query, con);
                if (paramsList != null)
                    cmd.Parameters.AddRange(paramsList.ToArray());

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dt);

                if (secure)
                    con.Close();

                return dt;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataSet ExecuteDataset(string query, bool secure, List<SQLiteParameter> paramsList = null)
        {
            try
            {
                if (con.State != ConnectionState.Open || con.State == ConnectionState.Closed)
                    Connect();

                DataSet ds = new DataSet();
                cmd = new SQLiteCommand(query, con);
                if (paramsList != null)
                    cmd.Parameters.AddRange(paramsList.ToArray());

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(ds);

                if (secure)
                    con.Close();

                return ds;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SQLiteDataReader ExecuteReader(string query, bool secure, SQLiteConnection connection = null)
        {
            try
            {
                if (con.State != ConnectionState.Open || con.State == ConnectionState.Closed)
                    Connect();

                cmd = new SQLiteCommand(query, con);
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (secure)
                    con.Close();

                return reader;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int ExecuteQuery(string query, bool returnID, bool secure, List<SQLiteParameter> sParams = null)
        {
            try
            {
                if (con.State != ConnectionState.Open || con.State == ConnectionState.Closed)
                    Connect();

                int ret = 0;
                cmd = new SQLiteCommand(con);
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;

                if (sParams != null)
                {
                    cmd.Parameters.AddRange(sParams.ToArray());
                }

                ret = cmd.ExecuteNonQuery();

                if (returnID)
                {
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    ret = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (secure)
                    con.Close();

                return ret;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("database is locked"))
                {
                    con.Close();
                    Connect();
                }                    

                return -1;
            }
        }
    }
}
