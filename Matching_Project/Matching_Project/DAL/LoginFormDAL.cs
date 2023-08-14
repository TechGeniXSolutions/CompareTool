using CommunityToolkit.Mvvm.ComponentModel;
using Matching_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usman.CodeBlocks.SQLiteManager;

namespace Matching_Project.DAL
{
   public class LoginFormDAL:ObservableObject
    {
        string TABLE_NAME = "LoginForm";

        public bool Create(LoginFormData model)
        {
            try
            {
                int id = SQLite.Insert<LoginFormData>(model, true); ;
                model.ID = id;
                //if (id > 0)
                //    return true;
                //else
                //    return false;

                return Convert.ToBoolean(id);

            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LoginFormDAL.Create()", true);
                return false;
            }

        }
        public bool Update(LoginFormData model)
        {
            try
            {
                var newmodel = SQLite.Update<LoginFormData>(model);
                if (newmodel != null)

                    return true;

                else

                    return false;


            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LoginFormDAL,Update()", true);
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                Dictionary<string, object> _filters = new Dictionary<string, object>();
                _filters.Add("ID", id);
                int n = SQLite.Delete<LoginFormData>(_filters);
                return Convert.ToBoolean(n);
            }
            catch (Exception ex)
            {
                Globals.LogError(ex, "LoginFormDAL,Update()", true);
                return false;

            }
        }

        public List<LoginFormData> Read(LoginFormData model)
        {
            try
            {
                var list = SQLite.GetList<LoginFormData>();
                return list;
            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LoginFormDAL,Update()", true);
                return null;
            }
        }

    }
}
