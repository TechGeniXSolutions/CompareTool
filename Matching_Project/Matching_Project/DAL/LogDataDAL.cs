﻿using CommunityToolkit.Mvvm.ComponentModel;
using Matching_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usman.CodeBlocks.SQLiteManager;

namespace Matching_Project.DAL
{
   public class LogDataDAL:ObservableObject
    {
        string TABLE_NAME = "LogData";

        public bool Create(LogData model)
        {
            try
            {
                int id = SQLite.Insert<LogData>(model, true); ;
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

                Globals.LogError(ex, "LoginFormDAL,Update()", true);
                return null;
            }
        }
        public bool DeleteEntryFromLogDAL(LogData entryToDelete)
        {
            try
            {
                Dictionary<string, object> _filters = new Dictionary<string, object>();
                _filters.Add("ID", entryToDelete.ID);

                int numRowsDeleted = SQLite.Delete<LogData>(_filters);

                return numRowsDeleted > 0;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                return false;
            }
        }
    }
}
