using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Usman.CodeBlocks.SQLiteManager
{
    public class SQLite
    {
        public static string[] numeric_types = new string[] { "Int16", "Int32", "Int64", "double", "long", "decimal", "float" };
        public static string[] integer_types = new string[] { "int16", "int32", "int64" };
        public static string[] decimal_types = new string[] { "double", "long", "decimal", "float", "single" };

        public static int Insert(string tableName, Object obj,bool autoId)
        {
            var cmd = GetInsertStringWithParams(tableName, obj);
            return SQLiteHelper.ExecuteQuery(cmd.Query,autoId, false, cmd.sParams);
        }

        public static int Insert<T>(object obj, bool autoId)
        {
            string tableName = typeof(T).Name;
            var cmd = GetInsertStringWithParams(tableName, obj);
            return SQLiteHelper.ExecuteQuery(cmd.Query, autoId, false, cmd.sParams);
        }

        public static Object Update(string tableName,Object newObj)
        {
            var cmd = GetUpdateStringWithParams(tableName,newObj);
            int ret = SQLiteHelper.ExecuteQuery(cmd.Query, false, false, cmd.sParams);
            if (ret > 0)
                return newObj;

            return null;
        }
        public static Object Update<T>(object newObj)
        {
            string tableName = typeof(T).Name;
            var cmd = GetUpdateStringWithParams(tableName, newObj);
            int ret = SQLiteHelper.ExecuteQuery(cmd.Query, false, false, cmd.sParams);
            if (ret > 0)
                return newObj;

            return null;
        }

        private static string GetWhereClause(Dictionary<string, object> _filters)
        {
            int i = 0;
            string where = "";
            if (_filters != null)
            {
                foreach (var item in _filters)
                {
                    if (item.Value != null)
                    {
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            if (i > 0 && i < _filters.Count)
                                where += string.Format(" and {0} {1}", item.Key, numeric_types.ToList().Contains(item.Value.GetType().Name) ? " = " + item.Value : " like '%" + item.Value + "%' ");
                            else
                                where += string.Format("{0} {1}", item.Key, numeric_types.ToList().Contains(item.Value.GetType().Name) ? " = " + item.Value : " like '%" + item.Value + "%' ");
                        }
                    }

                    i++;
                }
            }

            return where;
        }

        public static int Delete<T>(Dictionary<string,object> filters)
        {
            Type _type = typeof(T);
            string tableName = _type.Name;
            string query = "DELETE FROM " + tableName;
            string condition = GetWhereClause(filters);
            if (!string.IsNullOrEmpty(condition))
                query += " where " + condition;
            
            
            int ret = SQLiteHelper.ExecuteQuery(query, false, false);

            return ret;
        }

        public static int Delete(string tableName, string colName, string colValue)
        {
            string query = "DELETE FROM " + tableName + " where " + colName + " = " + colValue;
            int ret = SQLiteHelper.ExecuteQuery(query, false, false);

            return ret;
        }

        public static int ClearTable(string tableName)
        {
            string query = "DELETE FROM " + tableName;
            return SQLiteHelper.ExecuteQuery(query, false, false);
        }

        public static bool Exists(string tableName, Object obj)
        {
            //Type _type = obj.GetType();
            string query = "";
            query = CreateScalarQuery("Count",tableName, obj, "", new DateTime(1, 1, 1), new DateTime(1, 1, 1));
            var rec = SQLiteHelper.ExecuteScalar(query, false);
            if (rec != null)
            {
                return Convert.ToBoolean(Convert.ToInt32(rec));
            }

            return false;

        }

        public static bool Exists(string tableName, int id)
        {
            string idField = GetIDField(tableName);

            string query = "SELECT COUNT(*) FROM " + tableName + " WHERE " + idField + " =" + id;
            var rec = SQLiteHelper.ExecuteScalar(query, false);
            if (rec != null)
            {
                return Convert.ToBoolean(Convert.ToInt32(rec));
            }

            return false;

        }

        public static int GetMaxID(string tableName)
        {
            try
            {
                var obj = SQLiteHelper.ExecuteScalar("SELECT COUNT(*) FROM " + tableName, false);
                if (Convert.ToInt32(obj) > 0)
                    obj = SQLiteHelper.ExecuteScalar("SELECT MAX(" + GetIDField(tableName) + ") FROM " + tableName, false);

                return Convert.ToInt32(obj);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static List<T> GetListByDates<T>(Object obj, string dateColumn, DateTime startDate, DateTime endDate)
        {
            try
            {
                Type _type = typeof(T);
                string tableName = _type.Name;

                string query = "";

                if (obj == null)
                    query = "SELECT * FROM " + tableName;
                else
                    query = CreateQuery(_type, _type.Name,obj, dateColumn, startDate, endDate);

                var list = GetList<T>(_type.Name,query);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<T> GetList<T>(Dictionary<string, object> filters = null)
        {
            try
            {
                Type _type = typeof(T);
                T retObj = Activator.CreateInstance<T>();
                string query = "SELECT * FROM [" + _type.Name + "]";
                string condition = GetWhereClause(filters);
                if (!string.IsNullOrEmpty(condition))
                    query += " where " + condition;

                var list = GetList<T>(_type.Name, query);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static T Get<T>(Object obj)
        {
            Type _type = typeof(T);
            T retObj = Activator.CreateInstance<T>();
            string query = "";
            if (obj == null)
                query = "SELECT * FROM " + _type.Name;
            else
                query = CreateQuery(_type,_type.Name, obj,"", new DateTime(1, 1, 1), new DateTime(1, 1, 1));

            var list = GetList<T>(_type.Name,query);
            if (list == null)
                return default(T);

            if (list.Count > 0)
                retObj = list.FirstOrDefault();

            return retObj;

        }

        public static T Get<T>(Dictionary<string,object> filters)
        {
            Type _type = typeof(T);
            string tableName = _type.Name;
            T retObj = Activator.CreateInstance<T>();
            string query = "SELECT * FROM " + tableName;
            string condition = GetWhereClause(filters);
            if (!string.IsNullOrEmpty(condition))
                query += " where " + condition;

            var list = GetList<T>(tableName, query);
            if (list == null)
                return default(T);

            if (list.Count > 0)
                retObj = list.FirstOrDefault();

            return retObj;

        }

        public static object RunQuery(string query)
        {
            return SQLiteHelper.ExecuteScalar(query, true);
        }

        static string CreateQuery(Type _type,string tableName, Object obj, string dateCol, DateTime fromDate, DateTime toDate)
        {
            string query = "Select * from " + tableName + " ";
            List<string> dict = new List<string>();
            if (obj != null)
            {
                foreach (var prop in _type.GetProperties())
                {
                    if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name.ToLower() == "float" || prop.PropertyType.Name.ToLower() == "decimal" || prop.PropertyType.Name.ToLower() == "number" || prop.PropertyType.Name.ToLower() == "double")
                    {
                        var val = Convert.ToInt32(prop.GetValue(obj, null));
                        if (val > 0)
                        {
                            dict.Add(prop.Name + " = " + val.ToString());
                        }
                    }
                    else
                    {
                        var val = prop.GetValue(obj, null);
                        if (val != null)
                        {


                            if (!string.IsNullOrEmpty(val.ToString()))
                            {
                                if (prop.PropertyType.Name == "DateTime")
                                {
                                    if (prop.Name == dateCol && fromDate.Year > 1 && toDate.Year > 1)
                                    {
                                        dict.Add(prop.Name + " >= '" + fromDate.ToString("dd/MMM/yyyy"));
                                        dict.Add(prop.Name + " <= '" + toDate.ToString("dd/MMM/yyyy"));
                                    }
                                    else
                                    {
                                        var dt = Convert.ToDateTime(val);
                                        if (dt.Year > 1)
                                            dict.Add("Date(" + prop.Name + ") = '" + dt.ToString("yyyy-MM-dd") + "'");
                                    }

                                }
                                else if (prop.PropertyType.Name != "Boolean")
                                {
                                    dict.Add(prop.Name + " = '" + val.ToString() + "'");
                                }
                            }
                        }
                    }
                }

                if (dict.Count > 0)
                    query = query + "WHERE {0}";
                string clause = string.Join(" AND ", dict);
                query = string.Format(query, clause);
            }

            return query;
        }

        static string CreateScalarQuery(string scalarFunction, string tableName, Object obj, string dateCol, DateTime fromDate, DateTime toDate)
        {
            

            string query = string.Format("Select {0}(*) from {1} ",scalarFunction,tableName);
            List<string> dict = new List<string>();
            if (obj != null)
            {
                Type _type = obj.GetType();
                foreach (var prop in _type.GetProperties())
                {
                    if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name.ToLower() == "float" || prop.PropertyType.Name.ToLower() == "decimal" || prop.PropertyType.Name.ToLower() == "number" || prop.PropertyType.Name.ToLower() == "double")
                    {
                        var val = Convert.ToInt32(prop.GetValue(obj, null));
                        if (val > 0)
                        {
                            dict.Add(prop.Name + " = " + val.ToString());
                        }
                    }
                    else if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Date")
                    {
                        var val = prop.GetValue(obj, null);
                        if (val != null)
                        {
                            if (prop.Name == dateCol && fromDate.Year > 1 && toDate.Year > 1)
                            {
                                dict.Add(prop.Name + " >= '" + fromDate.ToString("dd/MMM/yyyy"));
                                dict.Add(prop.Name + " <= '" + toDate.ToString("dd/MMM/yyyy"));
                            }
                            else
                            {
                                var dt = Convert.ToDateTime(val);
                                if (dt.Year > 1)
                                    dict.Add("Date(" + prop.Name + ") = '" + dt.ToString("yyyy-MM-dd") + "'");
                            }
                        }                        
                    }
                    else
                    {
                        var val = prop.GetValue(obj, null);
                        if (val != null) { dict.Add(prop.Name + " = '" + val.ToString() + "'"); }
                    }
                }

                if (dict.Count > 0)
                    query = query + "WHERE {0}";
                string clause = string.Join(" AND ", dict);
                query = string.Format(query, clause);
            }

            return query;
        }

        public static List<T> GetList<T>(string tableName,string query)
        {
            try
            {
                Type _type = typeof(T);
                if (string.IsNullOrEmpty(query))
                    query = "SELECT * FROM " + tableName;

                DataTable table = SQLiteHelper.ExecuteTable(query,false, null);
                return ToList<T>(table);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ObservableCollection<T> GetObservableCollection<T>()
        {
            try
            {
                Type _type = typeof(T);
                string tableName = _type.Name;
                //if (tableName.EndsWith("y"))
                //    tableName = tableName.Remove(tableName.Length - 1, 1) + "ies";
                //else
                //    tableName += "s";

                DataTable table = SQLiteHelper.ExecuteTable("SELECT * FROM " + tableName, false);
                return ToObservableCollection<T>(table);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<T> ToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row);
                    data.Add(item);
                }
            }
            return data;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(DataTable dt)
        {
            ObservableCollection<T> data = new ObservableCollection<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        if (!dr.IsNull(column.ColumnName))
                        {
                            if (column.DataType.Name == "Int64" || column.DataType.Name == "Int32")
                            {
                                if (pro.PropertyType.Name != "Boolean")
                                    pro.SetValue(obj, Convert.ToInt32(dr[column.ColumnName]), null);
                                else
                                    pro.SetValue(obj, Convert.ToBoolean(Convert.ToInt32(dr[column.ColumnName])), null);
                            }                                
                            else
                            {
                                if (pro.PropertyType.Name == "DateTime")
                                    pro.SetValue(obj, Convert.ToDateTime(dr[column.ColumnName]), null);
                                else if (pro.PropertyType.Name == "Boolean")
                                {
                                    if (dr.IsNull(column.ColumnName))
                                        pro.SetValue(obj, Convert.ToBoolean(0), null);
                                    else
                                        pro.SetValue(obj, Convert.ToBoolean(Convert.ToInt32(dr[column.ColumnName])), null);
                                }
                                else
                                    pro.SetValue(obj, dr[column.ColumnName], null);
                            }

                        }
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        public static string GetInsertString(Object obj)
        {
            Type t = obj.GetType();

            string tableName = t.Name;
            //if (tableName.EndsWith("y"))
            //    tableName = tableName.Remove(tableName.Length-1,1) + "ies";
            //else
            //    tableName += "s";

            string query = "INSERT INTO " + tableName + "(<FIELDS>) VALUES(<VALUES>)";
            SQLiteDataReader reader = SQLiteHelper.ExecuteReader("select * from " + tableName, false);
            DataTable dt = reader.GetSchemaTable();

            reader.Close();
            query = query.Replace("<TABLENAME>", t.Name);

            string fields = "";
            string values = "";
            string idfield = "";
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToBoolean(row["iskey"]))
                    idfield = row["columnname"].ToString();

                if (!Convert.ToBoolean(row["IsAutoIncrement"]) || !Convert.ToBoolean(row["iskey"]))
                {
                    fields += row["columnname"].ToString() + ",";

                    PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                    if (row["datatypename"].ToString().ToLower() == "varchar" || row["datatypename"].ToString().ToLower() == "nvarchar" || row["datatypename"].ToString().ToLower() == "datetime")
                        values += "'" + property.GetValue(obj, null) + "',";
                    else
                    {
                        if (row["datatypename"].ToString().ToLower() == "bit" || row["datatypename"].ToString().ToLower() == "boolean")
                            values += Convert.ToInt32(property.GetValue(obj, null)) + ",";
                        else
                            values += property.GetValue(obj, null) + ",";
                    }
                }
            }

            fields = fields.Remove(fields.Length - 1, 1);
            values = values.Remove(values.Length - 1, 1);

            query = query.Replace("<FIELDS>", fields);
            query = query.Replace("<VALUES>", values);

            return query;
            //if (returnID)
            //{
            //    query += ";select last_insert_rowid() from " + tableName;
            //    return query;
            //}
            //else
            //    return query;
        }

        public static string GetUpdateString(Object obj)
        {
            Type t = obj.GetType();
            string tableName = t.Name;
            string query = "UPDATE " + tableName + " SET <PARAMS> WHERE <IDFIELD> = <VALUE>";
            SQLiteDataReader reader = SQLiteHelper.ExecuteReader("select * from " + tableName, false);
            DataTable dt = reader.GetSchemaTable();

            reader.Close();

            query = query.Replace("<TABLENAME>", tableName);
            string parameters = "";
            string idfield = "";
            string idvalue = "";

            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToBoolean(row["iskey"]))
                    idfield = row["columnname"].ToString();

                if (!Convert.ToBoolean(row["IsAutoIncrement"]) || !Convert.ToBoolean(row["iskey"]))
                {
                    PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                    if (row["datatypename"].ToString().ToLower() == "varchar" || row["datatypename"].ToString().ToLower() == "nvarchar" || row["datatypename"].ToString().ToLower() == "datetime")
                        parameters += property.GetValue(obj, null) != null ? row["columnname"].ToString() + " = '" + property.GetValue(obj, null).ToString() + "', " : row["columnname"].ToString() + " = '', ";
                    else if (row["datatypename"].ToString().ToLower() == "bit" || row["datatypename"].ToString().ToLower() == "boolean")
                        parameters += row["columnname"].ToString() + " = " + Convert.ToInt32(property.GetValue(obj, null)) + ", ";
                    else if (row["datatypename"].ToString().ToLower() == "blob")
                    {
                        var data = property.GetValue(obj, null);
                        if (data != null)
                        {
                            byte[] array = (byte[])data;
                            string myString = System.Text.Encoding.UTF8.GetString(array, 0, array.Length);
                            parameters += row["columnname"].ToString() + " = '" + myString + "', ";
                        }
                        
                    }                        
                    else
                        parameters += row["columnname"].ToString() + " = " + property.GetValue(obj, null) + ", ";
                }
            }

            idvalue = t.GetProperty(idfield).GetValue(obj, null).ToString();
            parameters = parameters.Trim();
            parameters = parameters.Remove(parameters.Length - 1, 1);
            query = query.Replace("<PARAMS>", parameters);
            query = query.Replace("<IDFIELD>", idfield);
            query = query.Replace("<VALUE>", idvalue);

            return query;
        }

        public static DatabaseQuery GetInsertStringWithParams(string tableName, object obj)
        {
            Type t = obj.GetType();
            DatabaseQuery dbQuery = new DatabaseQuery();

            string query = "INSERT INTO [" + tableName + "](<FIELDS>) VALUES(<VALUES>)";
            SQLiteDataReader reader = SQLiteHelper.ExecuteReader("select * from [" + tableName + "]", false);
            DataTable dt = reader.GetSchemaTable();

            reader.Close();
            query = query.Replace("<TABLENAME>", t.Name);

            string fields = "";
            string values = "";
            string idfield = "";
            List<SQLiteParameter> sParams = new List<SQLiteParameter>();

            foreach (DataRow row in dt.Rows)
            {
                if (!Convert.ToBoolean(row["IsAutoIncrement"]))
                {
                    string paramName = "@" + row["columnname"].ToString();
                    PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                    var paramValue = property.GetValue(obj, null);

                    sParams.Add(new SQLiteParameter(paramName, paramValue));
                    fields += row["columnname"].ToString() + ",";
                    values += paramName + ",";
                }
            }

            fields = fields.Remove(fields.Length - 1, 1);
            values = values.Remove(values.Length - 1, 1);

            query = query.Replace("<FIELDS>", fields);
            query = query.Replace("<VALUES>", values);

            dbQuery.Query = query;
            dbQuery.sParams = sParams;

            return dbQuery;
            
        }

        public static DatabaseQuery GetInsertStringWithParams(string tableName, object obj, SQLiteConnection connection)
        {
            Type t = obj.GetType();
            DatabaseQuery dbQuery = new DatabaseQuery();

            string query = "INSERT INTO [" + tableName + "](<FIELDS>) VALUES(<VALUES>)";
            SQLiteDataReader reader = SQLiteHelper.ExecuteReader("select * from [" + tableName + "]", false,connection);
            DataTable dt = reader.GetSchemaTable();

            reader.Close();
            query = query.Replace("<TABLENAME>", tableName);

            string fields = "";
            string values = "";
            string idfield = "";
            List<SQLiteParameter> sParams = new List<SQLiteParameter>();

            foreach (DataRow row in dt.Rows)
            {
                if (!Convert.ToBoolean(row["IsAutoIncrement"]))
                {
                    string paramName = "@" + row["columnname"].ToString();
                    PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                    var paramValue = property.GetValue(obj, null);

                    sParams.Add(new SQLiteParameter(paramName, paramValue));
                    fields += row["columnname"].ToString() + ",";
                    values += paramName + ",";
                }
            }

            fields = fields.Remove(fields.Length - 1, 1);
            values = values.Remove(values.Length - 1, 1);

            query = query.Replace("<FIELDS>", fields);
            query = query.Replace("<VALUES>", values);

            dbQuery.Query = query;
            dbQuery.sParams = sParams;

            return dbQuery;

        }

        public static int UpdateSyncResult(string tableName, string serverIdField, string entityId, string syncId, SQLiteConnection connection = null)
        {
            try
            {
                string query = "Update " + tableName + " SET " + serverIdField + " = " + entityId + ", IsUpdated = 0, SyncId = " + syncId + " WHERE Id = " + syncId;
                query = query.Replace("<IDFIELD>", GetIDField(tableName));
                int n = SQLiteHelper.ExecuteQuery(query, false, false, null);
                return n;
            }
            catch (Exception ex)
            {
                return 0;
            }
            
        }

        public static string GetIDField(string tableName, SQLiteConnection connection = null)
        {
            string idField = "";
            try
            {
                SQLiteDataReader reader = SQLiteHelper.ExecuteReader("select * from " + tableName, false, connection);
                DataTable dt = reader.GetSchemaTable();

                reader.Close();                
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToBoolean(row["iskey"]))
                    {
                        idField = row["columnname"].ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
            }
            

            return idField;
        }

        public static DatabaseQuery GetUpdateStringWithParams(string tableName,object obj)
        {
            Type t = obj.GetType();

            DatabaseQuery dbQuery = new DatabaseQuery();

            string query = "UPDATE [" + tableName + "] SET <PARAMS> WHERE <IDFIELD> = <VALUE>";
            SQLiteDataReader reader = SQLiteHelper.ExecuteReader("select * from " + tableName, false);
            DataTable dt = reader.GetSchemaTable();

            reader.Close();

            query = query.Replace("<TABLENAME>", tableName);
            string parameters = "";
            List<string> ParamsList = new List<string>();
            List<SQLiteParameter> sParams = new List<SQLiteParameter>();
            string idfield = "";
            string idvalue = "";

            foreach (DataRow row in dt.Rows)
            {
                string paramName = "@" + row["columnname"].ToString();
                PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                var paramValue = property.GetValue(obj, null);

                sParams.Add(new SQLiteParameter(paramName,paramValue));

                if (Convert.ToBoolean(row["iskey"]))
                    idfield = row["columnname"].ToString();
                else
                {
                    parameters += row["columnname"].ToString() + " = " + paramName + ",";
                }
            }

            //idvalue = t.GetProperty(idfield).GetValue(obj, null).ToString();
            parameters = parameters.Trim();
            parameters = parameters.Remove(parameters.Length - 1, 1);
            query = query.Replace("<PARAMS>", parameters);
            query = query.Replace("<IDFIELD>", idfield);
            query = query.Replace("<VALUE>", "@" + idfield);

            dbQuery.Query = query;
            dbQuery.sParams = sParams;
            
            return dbQuery;
        }

        public static DatabaseQuery GetUpdateStringWithParams(string tableName, object obj, SQLiteConnection connection)
        {
            Type t = obj.GetType();

            DatabaseQuery dbQuery = new DatabaseQuery();

            string query = "UPDATE [" + tableName + "] SET <PARAMS> WHERE <IDFIELD> = <VALUE>";
            SQLiteDataReader reader = SQLiteHelper.ExecuteReader("select * from " + tableName, false,connection);
            DataTable dt = reader.GetSchemaTable();

            reader.Close();

            query = query.Replace("<TABLENAME>", tableName);
            string parameters = "";
            List<string> ParamsList = new List<string>();
            List<SQLiteParameter> sParams = new List<SQLiteParameter>();
            string idfield = "";
            string idvalue = "";

            foreach (DataRow row in dt.Rows)
            {
                string paramName = "@" + row["columnname"].ToString();
                PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                var paramValue = property.GetValue(obj, null);

                sParams.Add(new SQLiteParameter(paramName, paramValue));

                if (Convert.ToBoolean(row["iskey"]))
                    idfield = row["columnname"].ToString();
                else
                {
                    parameters += row["columnname"].ToString() + " = " + paramName + ",";
                }
            }

            //idvalue = t.GetProperty(idfield).GetValue(obj, null).ToString();
            parameters = parameters.Trim();
            parameters = parameters.Remove(parameters.Length - 1, 1);
            query = query.Replace("<PARAMS>", parameters);
            query = query.Replace("<IDFIELD>", idfield);
            query = query.Replace("<VALUE>", "@" + idfield);

            dbQuery.Query = query;
            dbQuery.sParams = sParams;

            return dbQuery;
        }

        public static string GetDeleteString(Object obj)
        {
            Type t = obj.GetType();
            string tableName = t.Name;

            string query = "DELETE FROM <TABLENAME> WHERE <IDFIELD> = <VALUE>";
            query = query.Replace("<TABLENAME>", tableName);
            string idField = GetIDField(tableName);

            string idValue = t.GetProperty(idField).GetValue(obj, null).ToString();
            query = query.Replace("<IDFIELD>", idField);
            query = query.Replace("<VALUE>", idValue);

            return query;
        }
    }

    public class DatabaseQuery
    {
        public string Query { get; set; }
        public List<SQLiteParameter> sParams { get; set; }
    }
}
