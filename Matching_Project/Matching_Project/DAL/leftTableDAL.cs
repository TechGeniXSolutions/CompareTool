using Matching_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usman.CodeBlocks.SQLiteManager;

namespace Matching_Project.DAL
{
  public  class leftTableDAL
    {
        string TABLE_NAME = "LeftTable";

        public bool Create(LeftTable model)
        {
            try
            {
                int id = SQLite.Insert<LeftTable>(model, true); ;
                model.ID = id;
                //if (id > 0)
                //    return true;
                //else
                //    return false;

                return Convert.ToBoolean(id);

            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LeftTableDAL.Create()", true);
                return false;
            }

        }
        public bool Update(LeftTable model)
        {
            try
            {
                var newmodel = SQLite.Update<LeftTable>(model);
                if (newmodel != null)

                    return true;

                else

                    return false;


            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LeftTableDAL,Update()", true);
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                Dictionary<string, object> _filters = new Dictionary<string, object>();
                _filters.Add("ID", id);
                int n = SQLite.Delete<LeftTable>(_filters);
                return Convert.ToBoolean(n);
            }
            catch (Exception ex)
            {
                Globals.LogError(ex, "LeftTableDAL,Update()", true);
                return false;

            }
        }

        public List<LeftTable> Read(LeftTable model)
        {
            try
            {
                var list = SQLite.GetList<LeftTable>();
                return list;
            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "LeftTableDAL,Update()", true);
                return null;
            }
        }

    }
}
