using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2.Common
{
    public class DBHelper
    {
        public static string connStr = "Server=localhost;Database=stockmanager;Uid=root;Pwd=;CharSet=utf8;";

        public static IDbConnection connectionFactory()
        {
            IDbConnection db = new MySql.Data.MySqlClient.MySqlConnection(connStr);
            return db;

        }

        public dynamic Query<T>(string sql)
        {
            //var db = connectionFactory();
            //db.Open();

            object workers = null;
            using (var connection = connectionFactory())
            {
                workers = Dapper.SqlMapper.Query<T>(connection, sql);
            }

            return workers;
        }

        public dynamic QuerySingle<T>(string sql)
        {
            //var db = connectionFactory();
            //db.Open();
            object result= null;

            using (var connection = connectionFactory())
            {
                result = Dapper.SqlMapper.QuerySingleOrDefault<T>(connection, sql);
            }

            return result;
        }

        public int Execute(string sql)
        {
            sql = sql.Replace(@"\r", "");
            sql = sql.Replace(@"\n", "");

            int result = 0;
            using (var connection = connectionFactory())
            {
                result = Dapper.SqlMapper.Execute(connection, sql);
            }

            return result;
        }
    }
}
