using MyProject.IDAL.Bases;
using MyProject.Models;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MyProject.DAL {
public  class  system_roleDAL :BaseDALConf<SqlConnection, SqlParameter, SqlServerHelper>, Isystem_roleDAL{

        public system_roleDAL() : base(new SqlServerHelper()) { }
        public  system_role ToModel(DataRow row) {
            return GenericSQLGenerator.ToModel<system_role>(row);
        }

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <param name="model">system_role类的对象</param>
        /// <returns>object 主键</returns>
        object IBaseDAL< system_role >.Insert(system_role model) {
            object obj;
           obj = this.SqlHelper.ExecuteScalar(@"INSERT INTO [system_role]([id], [code], [name]) VALUES(@id, @code, @name); SELECT @@IDENTITY AS Id ;"
                        ,new SqlParameter("@id", model.id)
                        ,new SqlParameter("@code", model.code)
                        ,new SqlParameter("@name", model.name)
                  );
          return obj;
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>删除是否成功</returns>
        bool IBaseDAL< system_role >.DeleteById(System.String id) {
            int rows = this.SqlHelper.ExecuteNonQuery("DELETE FROM [system_role] WHERE [id] = @id", new SqlParameter("@id", id));
            return rows > 0;
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="model">system_role类的对象</param>
        /// <returns>更新是否成功</returns>
        bool IBaseDAL< system_role >.Update(system_role model) {
            int count = this.SqlHelper.ExecuteNonQuery("UPDATE [system_role] SET [code]=@code, [name]=@name WHERE [id]=@id;"
                        ,new SqlParameter("@id", model.id)
                        ,new SqlParameter("@code", model.code)
                        ,new SqlParameter("@name", model.name)
            );
        return count > 0;
        }

        /// <summary>
        /// 获得一条记录
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>system_role类的对象</returns>
        system_role IBaseDAL< system_role >.GetById(System.String id) {
            DataTable dt = this.SqlHelper.ExecuteDataTable("SELECT [id], [code], [name] FROM [system_role] WHERE [id]=@id", new SqlParameter("@id", id));
            if (dt.Rows.Count > 1) {
                throw new Exception("more than 1 row was found");
            }
            else if (dt.Rows.Count <= 0) {
                return null;
            }
            DataRow row = dt.Rows[0];
            system_role model = ToModel(row);
            return model;
        }

        /// <summary>
        /// 获得所有记录
        /// </summary>
        /// <returns>system_role类的对象的枚举</returns>
        IEnumerable<system_role>  IBaseDAL< system_role >.ListAll() {
            DataTable dt = this.SqlHelper.ExecuteDataTable("SELECT [id], [code], [name] FROM [system_role];");
             return Helper.GenericSQLGenerator.ToList<system_role>(dt);
        }

        /// <summary>
        /// 通过条件获得满足条件的记录
        /// </summary>
        /// <param name="model">system_role类的对象</param>
        /// <param name="whereStr">其他的sql 语句  </param>
        /// <param name="fields">需要的条件的字段名</param>
        /// <returns>满足条件的记录</returns>
         IEnumerable<system_role> IBaseDAL< system_role >.ListByWhere(system_role model,string whereStr, params string[] fields)
         {
             List<SqlParameter> lsParameter = new List<SqlParameter>();
             string str = Helper.GenericSQLGenerator.GetWhereStr<system_role>(model, "system_role", out lsParameter, fields);
             if(whereStr!=null&&whereStr.Trim().Length>0){str=str+" and "+whereStr;}
             SqlParameter[] sqlparm = lsParameter.ToArray();
             DataTable dt = this.SqlHelper.ExecuteDataTable(str, sqlparm);
             return Helper.GenericSQLGenerator.ToList<system_role>(dt);
         }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page">页数（从1开始计数）</param>
        /// <param name="num">每页个数（从1开始计数）</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="isDesc">是否降序</param>
        /// <param name="whereArr">查询条件：例如ID=1,NAME='ADMIN'</param>
        /// <returns></returns>
        public IEnumerable<system_role> ListByPage(int page = 1, int num = 10, string orderBy = "id", bool isDesc = true, params string[] whereArr)
        {
            string whereStr = "";
            List<string> ls = new List<string>();
            foreach (var v in whereArr) { if (v != null && v != "") { ls.Add(v); } }
            whereArr = ls.ToArray();
            if (num < 1 || page < 1) { return null; }
            if (whereArr != null && whereArr.Length > 0) { whereStr = " and a." + string.Join(" and a.", whereArr); }
            if (isDesc) { orderBy += " desc"; }
            DataTable dt = this.SqlHelper.ExecuteDataTable(string.Format(@"SELECT b.* FROM ( SELECT  a.*, ROW_NUMBER () OVER (ORDER BY a.{0} ) AS RowNumber FROM  [system_role] AS a WHERE (1 = 1) {1}) AS b WHERE  RowNumber BETWEEN {2} AND {3} ORDER BY b.{0}" , orderBy, whereStr, ((page-1) * num + 1), page * num));
            return Helper.GenericSQLGenerator.ToList<system_role>(dt);
        }
    }
}
