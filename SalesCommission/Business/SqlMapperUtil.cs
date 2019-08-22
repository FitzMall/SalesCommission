using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DapperORM;

namespace SalesCommission.Business
{
    public static class SqlMapperUtil
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["FDServer"].ConnectionString;
        //public static string CustomConnectionString { get; set; }

        public static SqlConnection GetOpenConnection(string connStr = null)
        {
            //Note: changed long if statement to inline If.
            SqlConnection connection = String.IsNullOrEmpty(connStr) ? new SqlConnection(ConnectionString) : new SqlConnection(ConfigurationManager.ConnectionStrings[connStr].ConnectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Stored proc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">The procname.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<T> StoredProcWithParams<T>(string procname, dynamic parms, string conn = null)
        {
            using (SqlConnection connection = GetOpenConnection(conn))
            {
                return connection.Query<T>(procname, (object)parms, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
            }
        }


        /// <summary>
        /// SQL with params.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="conn">The Connection String. Null if you want to use the default ConnectionString.</param>
        /// <returns></returns>
        public static List<T> SqlWithParams<T>(string sql, dynamic parms, string conn = null)
        {
            using (SqlConnection connection = GetOpenConnection(conn))
            {
                return connection.Query<T>(sql, (object)parms).ToList();
            }
        }

        /// <summary>
        /// Insert update or delete SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="conn">The Connection String. Null if you want to use the default ConnectionString.</param>
        /// <returns></returns>
        public static int InsertUpdateOrDeleteSql(string sql, dynamic parms, string conn = null)
        {
            using (SqlConnection connection = GetOpenConnection(conn))
            {
                return connection.Execute(sql, (object)parms);
            }
        }

        /// <summary>
        /// Insert update or delete stored proc.
        /// </summary>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="conn">The Connection String. Null if you want to use the default ConnectionString.</param>
        /// <returns></returns>
        public static int InsertUpdateOrDeleteStoredProc(string procName, dynamic parms, string conn = null)
        {
            using (SqlConnection connection = GetOpenConnection(conn))
            {
                return connection.Execute(procName, (object)parms, commandType: CommandType.StoredProcedure);
            }
        }

    }
}