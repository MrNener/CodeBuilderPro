using System.Configuration;

namespace MyProject.Helper.Bases
{
    public abstract class BaseSqlHelperConf
    {
        protected string ConnectionStr { get; set; }

        public   BaseSqlHelperConf() {
            ConnectionStr = ConfigurationManager.ConnectionStrings["dbConnect"].ConnectionString;
        }
        public BaseSqlHelperConf(string name)
        {
            ConnectionStr = ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
        public void ChangeConnectionSource(string name)
        {
            ConnectionStr= ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}