
using Matching_Project;
using Matching_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usman.CodeBlocks.SQLiteManager;

namespace HIKDataTool.DAL
{
  public class RightTableDAL
    {
        string TABLE_NAME = "RightTable";

        public bool Create(RightTable model)
        {
            try
            {
                int id = SQLite.Insert<RightTable>(model, true); ;
                model.ID = id;
                //if (id > 0)
                //    return true;
                //else
                //    return false;

                return Convert.ToBoolean(id);

            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "RightTableDAL.Create()", true);
                return false;
            }
            
        }
    public bool Update(RightTable model)
        {
            try
            {
                var newmodel = SQLite.Update<RightTable>(model);
                if(newmodel != null)
                
                    return true;
                
                else
                
                    return false;
                

            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "RightTableDAL,Update()", true);
                return false;
            }
        }
    
    public bool Delete(int id)
        {
            try
            {
                Dictionary<string, object> _filters = new Dictionary<string, object>();
                _filters.Add("ID", id);
                int n = SQLite.Delete<RightTable>(_filters);
                return Convert.ToBoolean(n);
            }
            catch (Exception ex)
            {
                Globals.LogError(ex, "RightTableDAL,Update()", true);
                return false;
                
            }
        }
   
    public List<RightTable> Read(RightTable model)
        {
            try
            {
                var list = SQLite.GetList<RightTable>();
                return list;
            }
            catch (Exception ex)
            {

                Globals.LogError(ex, "RightTableDAL,Update()", true);
                return null;
            }
        }

    }

}














