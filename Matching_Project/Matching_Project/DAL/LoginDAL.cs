using Matching_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usman.CodeBlocks.SQLiteManager;

namespace Matching_Project.DAL
{
  public  class LoginDAL
    {
        string TABLE_NAME = "LogData";

        public bool Create(UserData model)
        {
            try
            {
                int id = SQLite.Insert<UserData>(model, true); ;
                model.ID = id;
                //if (id > 0)
                //    return true;
                //else
                //    return false;

                return Convert.ToBoolean(id);

            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LogDataDAL.Create()", true);
                return false;
            }

        }
        public bool Update(LogData model)
        {
            try
            {
                var newmodel = SQLite.Update<LogData>(model);
                if (newmodel != null)

                    return true;

                else

                    return false;


            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LogDataDAL,Update()", true);
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                Dictionary<string, object> _filters = new Dictionary<string, object>();
                _filters.Add("ID", id);
                int n = SQLite.Delete<LogData>(_filters);
                return Convert.ToBoolean(n);
            }
            catch (Exception ex)
            {
                Globals.LogError(ex, "LogDataDAL,Update()", true);
                return false;

            }
        }

        public List<LogData> Read(LogData model)
        {
            try
            {
                var list = SQLite.GetList<LogData>();
                return list;
            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LogDataDAL,Update()", true);
                return null;
            }
        }

    }
}
