using System.Data.SqlClient;
using System.Data;
using MyProject.Helper.Bases;

namespace MyProject.Helper
{
    public  class MsSqlHelper: BaseSqlHelperConf, ISqlHelper<SqlConnection ,SqlParameter>
    {
        public MsSqlHelper() : base() { }
        public MsSqlHelper(string name) : base(name) { }


        public object ExecuteScalar(string cmdText, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionStr))
            {
                conn.Open();
                return ExecuteScalar(conn, cmdText, parameters );
            }
        }

        public  DataSet ExecuteDataSet(string cmdText, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(ConnectionStr))
            {
                con.Open();
                return ExecuteDataSet(con, cmdText, parameters);
            }
        }

        public  DataTable ExecuteDataTable(string cmdText, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(ConnectionStr))
            {
                con.Open();
                return ExecuteDataTable(con, cmdText, parameters );
            }
        }

        public  int ExecuteNonQuery(SqlConnection conn, string cmdText,
           params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 执行返回第一行第一列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public  object ExecuteScalar(SqlConnection conn, string cmdText,
            params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }
        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="con"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public  DataTable ExecuteDataTable(SqlConnection con, string cmdText, params SqlParameter[] parameters)
        {
            return ExecuteDataSet(con, cmdText, parameters).Tables[0];
        }
        /// <summary>
        /// 执行返回DataSet
        /// </summary>
        /// <param name="con"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public  DataSet ExecuteDataSet(SqlConnection con, string cmdText, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    adapter.FillSchema(ds, SchemaType.Source);
                    adapter.Fill(ds);
                    return ds;
                }
            }
        }

        public  int ExecuteNonQuery(string cmdText, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionStr))
            {
                conn.Open();
                return ExecuteNonQuery(conn, cmdText, parameters );
            }
        }
    }
}