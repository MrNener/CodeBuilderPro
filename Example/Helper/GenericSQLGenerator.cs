using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
namespace MyProject.Helper
{
    public static class GenericSQLGenerator
    {
        public static T ToModel<T>(DataRow row) where T : class,new()
        {
            var prList = GetPropertyInfo<T>(row.Table);
            var item = new T();
            prList.ForEach(p =>
            {
                if (row[p.Name] != DBNull.Value)
                {
                    p.SetValue(item, row[p.Name], null);
                }
            });
            return item;
        }
        public static List<PropertyInfo> GetPropertyInfo<T>(DataTable dt) where T : class,new()
        {
            Type type = typeof(T);
            var prList = new List<PropertyInfo>();
            Array.ForEach(type.GetProperties(), p =>
            {
                if (dt.Columns.Contains(p.Name))
                { 
                    prList.Add(p);
                }
            });
            return prList;
        }
        public static List<T> ToList<T>(DataTable dt) where T : class,new()
        {
            var prList=GetPropertyInfo<T>(dt);
            List<T> ls = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new T();
                prList.ForEach(p =>
                {
                    if (row[p.Name] != DBNull.Value)
                    {
                        p.SetValue(item, row[p.Name], null);
                    }
                });
                ls.Add(item);
            }
            return ls;
        }
        public static string GetWhereStr<T>(T entity, string tableName, out List<SqlParameter> list, params string[] fields) where T : new()
        {
            StringBuilder sbu = new StringBuilder();
            list = new List<SqlParameter>();

            sbu.Append("");
            sbu.Append("select * from [" + tableName + "] where (1=1)");
            if (fields != null)
            {
                //遍历每一个要生成SQL的字段，取出内容
                foreach (string field in fields)
                {
                    object value = entity.GetType().GetProperty(field).GetValue(entity, null);
                    if (value is int || value is double || value is decimal || value is double || value is long || value is float)
                    {

                        sbu.AppendFormat(" and ([{0}]=@{0})", field);
                        list.Add(new SqlParameter("@" + field + "", value));

                    }
                    else if (value is DateTime)
                    {
                        sbu.AppendFormat(" and ([{0}]=@{0})", field);
                        list.Add(new SqlParameter("@" + field + "", Convert.ToDateTime(value)));

                    }
                    else if (value is Guid)
                    {
                        sbu.AppendFormat(" and ([{0}]=@{0})", field);
                        list.Add(new SqlParameter("@" + field + "", new Guid(value.ToString())));

                    }
                    else if (value is Boolean)
                    {
                        sbu.AppendFormat(" and ([{0}]=@{0})", field);
                        list.Add(new SqlParameter("@" + field + "", Convert.ToBoolean(value)));

                    }
                    else if (value is String || value is Char)
                    {
                        sbu.AppendFormat(" and ([{0}]=@{0})", field);
                        list.Add(new SqlParameter("@" + field + "", Convert.ToString(value)));

                    }
                    else
                    {
                        sbu.AppendFormat(" and ([{0}]=@{0})", field);
                        list.Add(new SqlParameter("@" + field + "",CommonExtend.ToDBValue(value)));
                    }
                }
            }
            return (sbu.ToString());
        }
    }
}