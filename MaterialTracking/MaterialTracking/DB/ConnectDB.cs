
using System;
using System.Collections.Generic;

using System.Data;

using System.Data.SqlClient;



namespace MaterialTracking. DB.SQL
{
    public class ConnectDB
    {
        System.Data.SqlClient.SqlConnection con ;
        SqlCommand cmd = new SqlCommand();       
      
        
        //private bool bo=false;
        private bool checkconnection = false;
        private static ConnectDB cnn;
        public  static ConnectDB Connection
        {
            get
            {
                if (cnn == null) cnn = new ConnectDB(); ;
                return cnn;
            }
        }
        //public static ConnectDB ConnectionTo(string Factory=null)
        //{
        //    cnn = new ConnectDB(Factory);
        //    return cnn;
        //}
        public ConnectDB()
        {
           
           
        }
        private string Fac(MyFactory mf)
        {
          
            switch (mf)
            {
                case MyFactory.LVL:
                    return "Data Source=192.168.60.9;Initial Catalog=LIY_ERP;Persist Security Info=True;User ID=lacty;Password=wu0g3tp6";
                case MyFactory.LYV:
                    return "Data Source=192.168.0.1;Initial Catalog=LIY_ERP;Persist Security Info=True;User ID=lacty;Password=wu0g3tp6";
                case MyFactory.LHG:
                    return "Data Source=192.168.30.1;Initial Catalog=LIY_ERP;Persist Security Info=True;User ID=lacty;Password=wu0g3tp6";
                default:
                    return "Data Source=192.168.55.12;Initial Catalog=LIY_ERP;Persist Security Info=True;User ID=lacty;Password=wu0g3tp6";
            }
        }
       //private string Fac(string fac)
       // {
       //     switch (fac)
       //     {
       //         case "LVL":
       //             return "Data Source=192.168.60.9;Initial Catalog=LIY_ERP;Persist Security Info=True;User ID=lacty;Password=wu0g3tp6";
       //         case "LYV":
       //             return "Data Source=192.168.0.1;Initial Catalog=LIY_ERP;Persist Security Info=True;User ID=lacty;Password=wu0g3tp6";
       //         case "LHG":
       //             return "Data Source=192.168.30.1;Initial Catalog=LIY_ERP;Persist Security Info=True;User ID=lacty;Password=wu0g3tp6";
               
       //         default:
       //             return "Data Source=192.168.55.12;Initial Catalog=LIY_ERP;Persist Security Info=True;User ID=lacty;Password=wu0g3tp6";
       //     }
       // }
      
       
        private bool OpenConnection()
        {
            try
            {
               // string scnn = ConnectionString;

                if (con.State == ConnectionState.Closed)
                {
                    //string connectString = Fac(MaterialTracking.DB.StoreLocal.Instant.Factory);
                    string connectString = Fac(DB.StoreLocal.Instant.Myfac);
                    con = new SqlConnection(connectString);
                    //con.ConnectionString = strCnn;
                    con.Open();
                    checkconnection = true;

                }


            }
            catch (Exception  xx)
            {
              
                checkconnection = false;
            }
            return checkconnection;
        }
        private void CloseConnection()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    checkconnection = false;
                }
            }
            catch (Exception ex)
            {                               
                return;

            }
        }
        private bool RefeshConnection()
        {
            //string s =MaterialTracking.DB.StoreLocal.Instant.Factory.ToString();
            if (con == null) con = new SqlConnection(Fac(MaterialTracking.DB.StoreLocal.Instant.Myfac));

            if (con.State == ConnectionState.Closed) 
            {
                if (OpenConnection())
                {
                    checkconnection = true;
                }
                else
                {
                    checkconnection = false;
                }

            }
            else { checkconnection = true; }
            return checkconnection;
        }
       
        public int ExecuteQuery(string mSQL) //Insert, udate, delete
        {
            RefeshConnection();
            if (!checkconnection)
            {
                return 0;
            }
            try
            {
                cmd = con.CreateCommand();
                cmd.CommandText = mSQL;
                return cmd.ExecuteNonQuery();                               
            }
            catch(Exception ex)
            {                                
                return 0;
            }
            finally { CloseConnection(); }
            
        }
        public bool? CheckUser(string username,string password)
        {
           
            try
            {
                bool c = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
                if (!c) return c;

                string sql = $"SELECT App_Users.Lock, app_users.DepID FROM Busers left join App_Users on busers.USERID = App_Users.UserID where busers.USERID ='{username}' and Busers.Pwd='{password}' and App_users.appName ='Material'";

                var dt = FillDataTable(sql); 

                if (dt == null) { return null; }

                if (dt.Rows.Count != 0)
                {
                    var locked = dt.Rows[0]["lock"].ToString();

                    if (locked.ToUpper() == "n".ToUpper())
                    {
                        var material_role = dt.Rows[0]["DepID"].ToString();

                        DB.StoreLocal.Instant.CurrentDep = material_role;

                        sql = "UPDATE App_Users SET LastLogin = Getdate() WHERE (UserID=@userid and App_Users.AppName ='Material')";

                        Update_Parameter(sql, new string[] { username });
                    }
                    else
                    {
                        if(locked == "y")
                        {
                            return null;
                        }
                    }



                    //if (DB.StoreLocal.Instant.List_DepId.Contains(DB.StoreLocal.Instant.Depid))
                    //{
                    //    return true;

                    //}
                    //else return false;

                    return true;
                }
                else return false;
            }
            catch (Exception xx)
            {

                Services.Alert.Msg("Err", xx.Message);

                return null;
            }
           
        }
        public string[] Procedure_Confirm(string Storeprocedure, string[] p, string[] value)
        {
            string [] result = new string[2];
            if (!RefeshConnection()) 
            {
                result[0] = "0";
                result[1] = "Connect fail";
                return result;
            }

            try
            {
                cmd = new SqlCommand(Storeprocedure, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 5000;
                for (int i = 0; i < p.Length; i++)
                {
                    cmd.Parameters.AddWithValue(p[i], value[i]);
                }

               //Create varialbles to get output from sql
                cmd.Parameters.Add("@sst_create", SqlDbType.VarChar, 100);
                cmd.Parameters["@sst_create"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@error_create", SqlDbType.VarChar, 100);
                cmd.Parameters["@error_create"].Direction = ParameterDirection.Output;
                try
                {
                    int i = cmd.ExecuteNonQuery();
                    //Storing the output parameters value in array variables.  
                  
                    result[0] = Convert.ToString(cmd.Parameters["@sst_create"].Value);
                    result[1] = Convert.ToString(cmd.Parameters["@error_create"].Value);
                    // Here we get all 02 values from database in above three variables.  
                    CloseConnection();
                }
                catch (Exception ex)
                {
                    CloseConnection();
                    return null;
                }
            }
            catch (Exception)
            {
                CloseConnection();
                return null;
            }
            return result;
        }
        public DataTable ExeStoreProcedure(string Storeprocedure, string[] p, string[] value)
        {
            DataTable dt = new DataTable();
            RefeshConnection();
          
            try
            {
                cmd = new SqlCommand(Storeprocedure, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 5000;
                for (int i = 0; i < p.Length; i++)
                {
                    cmd.Parameters.AddWithValue(p[i], value[i]);
                }
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                CloseConnection();
                return dt;
            }
            catch (Exception)
            {
                CloseConnection();
                throw;
            }
        }

     
        //public DataSet FillDataSet(string mSQL)
        //{
        //    DataSet dtaSet = new DataSet();

        //    SqlDataAdapter dtaRef;
        ////  System.Data.SqlClient.SqlCommandBuilder cbdRef;
        //    RefeshConnection();
        //    if (checkconnection)
        //    {
        //        try
        //        {
        //            dtaRef = new SqlDataAdapter(mSQL, con);
        //          //  cbdRef = new SqlCommandBuilder(dtaRef);
        //            dtaRef.Fill(dtaSet);

        //        }
        //        catch (Exception)
        //        {
        //            CloseConnection();
                   
        //            dtaSet = null;
        //            return dtaSet;
                   
        //        }
                
        //        CloseConnection();
                
        //    }
        //    return dtaSet;

        //}
        //public DataSet FillDataSet(string tableName, string mSQL)
        //{
        //    DataSet dtaSet = new DataSet();
        //    SqlDataAdapter dtaRef;
        //   // SqlCommandBuilder cbdRef;
        //    RefeshConnection();
        //    if (checkconnection)
        //    {
        //        try
        //        {
        //            dtaRef = new SqlDataAdapter(mSQL, con);
        //          //  cbdRef = new SqlCommandBuilder(dtaRef);
        //            dtaRef.Fill(dtaSet, tableName);
        //        }
        //        catch (Exception)
        //        {
        //            CloseConnection();
                    
        //            dtaSet = null;
        //            return dtaSet;
        //        }
                
        //    }
           
        //    CloseConnection();
        //    return dtaSet;
        //}
      
        public DataTable FillDataTable(string mSQL)
        {
            SqlDataAdapter DA ;
            DataTable dtbl = new DataTable();

            // SqlCommandBuilder CmdBuilder;

            using (var cnn = new SqlConnection(Fac(MaterialTracking.DB.StoreLocal.Instant.Myfac)))
            {
                cnn.Open();
                try
                {
                    using (var cmd = new SqlCommand(mSQL, cnn))
                    {
                        using (DA = new SqlDataAdapter(cmd))
                        {
                            DA.Fill(dtbl);
                        }
                    }

                }
                catch (Exception ex)
                {
                    cnn.Close();
                    Services.DisplayToast.Show.Toast(ex.Message, Class.Style.Error);
                    return null;

                }
                finally { cnn.Close(); };
            }
            return dtbl;
        }
        public DataTable SelectData(string sql, string[] para)
        {
            try
            {
                int star = 0;
                List<string> list = new List<string>();
                foreach (char c in sql)
                {
                    string val = "";
                    star++;

                    if (c == 64)//@
                    {
                        for (int i = star - 1; i < sql.Length; i++)
                        {

                            char ch = Convert.ToChar(sql.Substring(i, 1));
                            if (ch != 44)//,
                            {
                                if (ch != 41)//)
                                {
                                    if (ch != 32)// ' '
                                    {
                                        val += sql.Substring(i, 1).Trim();
                                    }
                                    else
                                    {
                                        list.Add(val);
                                        break;
                                    }

                                }
                                else
                                {
                                    list.Add(val);

                                }

                            }
                            else
                            {
                                list.Add(val);

                            }
                            if (i == sql.Length - 1)
                            {
                                list.Add(val);
                            }
                        }
                    }
                }
                //RefeshConnection();

                DataTable dt = new DataTable();
                using (var cnn = new SqlConnection(Fac(MaterialTracking.DB.StoreLocal.Instant.Myfac)))
                {


                    //if (!checkconnection)
                    //{
                    //    return null;
                    //}
                    try
                    {
                        cnn.Open();
                        cmd = new SqlCommand(sql, cnn);
                        for (int i = 0; i < para.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(list[i], para[i]);
                        }
                        SqlDataAdapter DA = new SqlDataAdapter();

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        DA.SelectCommand = cmd;
                        DA.Fill(dt);
                        cnn.Close();
                    }
                    catch (Exception)
                    {
                        cnn.Close();
                        throw;
                    }
                   
                }
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }
        private int _effect = 0;
        public int Effected
        {
            get { return _effect; }
        }
        public int Update_Parameter(string sql, string[] para)
        {
            int ef = 0;
            int star=0;
            List<string> list = new List<string>();
            foreach ( char c in sql)
            {
                string val = "";
                star++;

                 if (c == 64)
                {
                    for (int i = star - 1; i < sql.Length; i++)
                    {
                        char ch = Convert.ToChar(sql.Substring(i, 1));
                        if (ch != 44)//,
                        {
                            if (ch != 41)//)
                            {
                                if (ch != 32)// ' '
                                {
                                    val += sql.Substring(i, 1).Trim();
                                    
                                }
                                else 
                                {
                                    list.Add(val);
                                    break;
                                }

                            }
                            else
                            {
                                list.Add(val);
                                //goto tiep;
                            }

                        }
                        else
                        {
                            list.Add(val);
                            break;
                        }

                    }
                 }
            }
            RefeshConnection();
            if (!checkconnection)
            {
                return 0;
            }
            cmd = new SqlCommand(sql, con);
            for (int i = 0; i < para.Length; i++)
            {
                cmd.Parameters.AddWithValue(list[i], para[i]);
            }
            try
            {
                ef = cmd.ExecuteNonQuery();

            }
            catch (Exception xx)
            {

                throw;
            }
            finally
            {
                CloseConnection();


            }
            return ef;
        }
        public int Insert_Parameter(string SQL, string[] Para)
        {
            string sql = SQL;
            RefeshConnection();
            if (!checkconnection)
            {
                return 0;
            }
            
            //cmd = con.CreateCommand();
            //cmd.CommandText = sql;
            //cmd.Connection = con;
            cmd = new SqlCommand(sql, con);
            int star = 0;            
            List<string> list = new List<string>();
            foreach (char c in sql)
            {
                string val = "";
                star++;
                if (c == 64)
                {
                    for (int i = star - 1; i < sql.Length; i++)
                    {
                        char ch = Convert.ToChar(sql.Substring(i, 1));
                        if (ch != 44)
                        {
                            if (ch != 41)
                            {
                                val += sql.Substring(i, 1).Trim();
                            }
                            else
                            {
                                list.Add(val);
                                goto tiep;
                            }

                        }
                        else
                        {
                            list.Add(val);
                            break;
                        }

                    }
                }
            }

            tiep: for (int i = 0; i < list.Count; i++)
            {
                cmd.Parameters.AddWithValue(list[i], Para[i]);
            }
            try
            {
                _effect = cmd.ExecuteNonQuery();

            }
            catch (Exception xx)
            {
                
            }
            finally
            {

                CloseConnection();
            }
            return _effect;
        }
        public bool Run_Paramenter(string Sql, string[] ColVal)
        {
            string sql = Sql;
            RefeshConnection();
            if (!checkconnection)
            {
                return false;
            }
            try
            {
                cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Connection = con;
                int j = 0;
                int k = 0;
                SqlParameter para;
                string[] cot=new string[ColVal.Length/2];
                string[] val=new string[ColVal.Length / 2]; ;
                for (int i = 0; i < ColVal.Length; i++)
                {
                    foreach (char c in ColVal[i].Substring(0,1))
                    {
                        if (c==64)
                        {
                            cot[j] = ColVal[i];
                            j++;
                        }
                        else { val[k]=ColVal[i];k++; }
                    }
                   
                }
                for (int i = 0; i < cot.Length ; i++)
                {
                    para = new SqlParameter();
                    para.ParameterName = cot[i];
                    para.Value = val[i];
                    cmd.Parameters.Add(para);
                }
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader .Read())
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { CloseConnection(); }
            // "select userid,username from Busers where userid=?";

        }
       
      public void CreateTable(string sql,string tablename)
        {
            var commandStr = "If not exists(select name from sysobjects where name = '" + tablename + "')" + sql;
            RefeshConnection();
            try
            {
                using (cmd = new SqlCommand(commandStr, con))
                    cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }
            CloseConnection();
        }
       
    }
}
