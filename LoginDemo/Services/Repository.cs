using LoginDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LoginDemo.Services
{
    public class Repository 
    {
        string connectionstring = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        public async Task<ResponseModel<string>> login(LoginViewModel user)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            if (user != null)
            {
                string md5_password = md5_string(user.Password);

                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = string.Format("Select * FROM tbl_Users WHERE password = '{0}' and Email='{1}'", md5_password,user.Email);
                    cmd.Connection = conn;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    await Task.Run(()=> da.Fill(dt));

                    if (dt.Rows.Count > 0)
                    {
                        response.Data = JsonConvert.SerializeObject(dt);
                        response.resultCode = 200;
                    }
                    else
                    {
                        response.message = "User Not Found!";
                        response.resultCode = 500;
                    }

                   
                }
            }
            return response;
        }

        public async Task<ResponseModel<string>> Register(LoginViewModel user)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            if (user != null)
            {
                string md5_password = md5_string(user.Password);



                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = string.Format("Insert INTO tbl_Users(Email,Password,Reg_Date) VALUES('{0}','{1}','{2}')", user.Email, md5_password, DateTime.Now.ToString());
                    cmd.Connection = conn;

                    conn.Open();
                    var result = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                    if (result == 1) //row changes in the database - successfull
                    {
                        response.message = "User has been registered!";
                        response.resultCode = 200;
                    }
                    else
                    {
                        response.message = "Unable to register User!";
                        response.resultCode = 500;
                    }

                }
            }
            return response;
        }

        public string md5_string(string password)
        {
            string md5_password = string.Empty;
            using (MD5 hash = MD5.Create())
            {
                md5_password = string.Join("", hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("x2")));
            }

            return md5_password;
        }
    }
}