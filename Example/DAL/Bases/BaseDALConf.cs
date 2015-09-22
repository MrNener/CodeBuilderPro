using MyProject.Helper.Bases;
using System.Data.Common;

namespace MyProject.DAL.Bases
{
    public abstract class BaseDALConf<C, P, H>
        where C : DbConnection, new()
        where P : DbParameter, new()
        where H : ISqlHelper<C, P>, new()
    {
        protected ISqlHelper<C, P> SqlHelper { get; set; }

        public BaseDALConf(H helper)
        {
            this.SqlHelper = helper;
        }
        public BaseDALConf()
        {
            this.SqlHelper = new H();
        }
    }
}
