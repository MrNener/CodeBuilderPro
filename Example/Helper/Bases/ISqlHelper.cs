using System.Data;
using System.Data.Common;

namespace MyProject.Helper.Bases
{
    public interface ISqlHelper<C,P> 
        where C: DbConnection,new ()
       where P: DbParameter,new ()
    {
        /// <summary>
        /// 执行返回受影响的行数
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string cmdText, params P[] parameters);

        /// <summary>
        /// 执行返回DataSet
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string cmdText, params P[] parameters);

        /// <summary>
        /// 执行返回第一行第一列
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object ExecuteScalar(string cmdText, params P[] parameters);

        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataTable ExecuteDataTable(string cmdText, params P[] parameters);

        /// <summary>
        /// 执行返回受影响的行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteNonQuery(C conn, string cmdText,
           params P[] parameters);

        /// <summary>
        /// 执行返回第一行第一列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object ExecuteScalar(C conn, string cmdText,
            params P[] parameters);


        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="con"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataTable ExecuteDataTable(C con, string cmdText, params P[] parameters);


        /// <summary>
        /// 执行返回DataSet
        /// </summary>
        /// <param name="con"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(C con, string cmdText, params P[] parameters);
    }
}