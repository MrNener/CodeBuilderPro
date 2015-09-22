using MyProject.Helper;
using System.Data.SqlClient;

namespace MyProject.DAL.Bases
{
    /// <summary>
    /// SqlServer
    /// </summary>
    public abstract class MsSqlDALConf : BaseDALConf<SqlConnection, SqlParameter, MsSqlHelper>
    {
        protected MsSqlDALConf(string name) : base(new MsSqlHelper(name))
        {
        }
        protected MsSqlDALConf() : base()
        {
        }
    }
}
