using System.Data;
using System.Linq;
using System.Text;

namespace CodeBuilderPro
{
    public enum SQLType
    {
        MySql=2,
        MsSql=1
    }
    public class CreateCodeHelper
    {
        public static int sqltype = 1;//数据库类型 1 sql server ；2 mysql
        public static string leftStr = "["; //左定界符
        public static string rightStr = "]";//右定界符

        #region GetDataTypeName 进行可空类型处理
        /// <summary>
        ///进行可空类型处理
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string GetDataTypeName(DataColumn column)
        {
            //如果列允许为NULL，并且在.NET中的类型不能为空（值类型）
            if (column.AllowDBNull && column.DataType.IsValueType)
            {
                return column.DataType + "?";
            }
            else
            {
                return column.DataType.ToString();
            }
        }
        #endregion
        #region GetDataTypeNameString 进行可空类型处理，String为空要特殊处理
        /// <summary>
        /// 进行可空类型处理，String为空要特殊处理
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string GetDataTypeNameString(DataColumn column)
        {
            //如果列允许为NULL，并且在.NET中的类型不能为空（值类型）
            if (column.AllowDBNull)
            {
                return column.DataType + "?";
            }
            else
            {
                return column.DataType.ToString();
            }
        }
        #endregion
        #region GetColumnNames 以数组形式返回列名
        /// <summary>
        /// 以数组形式返回列名
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string[] GetColumnNames(DataTable dt)
        {
            string[] colNames = new string[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn dataCol = dt.Columns[i];
                colNames[i] = dataCol.ColumnName;
            }
            return colNames;
        }
        #endregion
        #region GetColumnNamesUpdate 以数组形式返回"列名=@列名"
        /// <summary>
        /// 以数组形式返回"列名=@列名"
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string[] GetColumnNamesUpdate(DataTable dt)
        {
            string[] colNames = new string[dt.Columns.Count - 1];
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                DataColumn dataCol = dt.Columns[i];
                colNames[i - 1] = leftStr + dataCol.ColumnName + rightStr + "=@" + dataCol.ColumnName;
            }
            return colNames;
        }
        #endregion
        #region GetSqlParameter 得到 new SqlParameter
        /// <summary>
        /// 得到 new SqlParameter
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        private static void GetSqlParameter(DataTable dt, StringBuilder sb, string hasNamespace, bool isNullId)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            string heP = helper;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i == 0 && isNullId == true) { continue; }
                if (GetDataTypeNameString(dt.Columns[i]).IndexOf("?") > 0)
                {
                    sb.AppendLine(hasNamespace + "                    ,new " + heP + "Parameter(\"@" + dt.Columns[i].ColumnName + "\", CommonExtend.ToDBValue(model." + dt.Columns[i].ColumnName + "))");
                }
                else
                {
                    sb.AppendLine(hasNamespace + "                    ,new " + heP + "Parameter(\"@" + dt.Columns[i].ColumnName + "\", model." + dt.Columns[i].ColumnName + ")");
                }
            }
        }
        #endregion
        #region CreateModelCode 生成Model带命名空间
        /// <summary>
        /// 生成Model带命名空间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="strNamespace"></param>
        /// <returns></returns>
        public StringBuilder CreateModelCode(string tableName, DataTable dt, string strNamespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("namespace " + strNamespace + ".Models {");
            sb.AppendLine("    public  class  " + tableName + " {");
            foreach (DataColumn col in dt.Columns)
            {
                sb.AppendLine("        public " + GetDataTypeName(col) + " " + col.ColumnName + " {get; set;}");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb;
        }
        #endregion
        #region CreateModelCode 生成Model不带命名空间
        /// <summary>
        /// 生成Model不带命名空间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public StringBuilder CreateModelCode(string tableName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public class  " + tableName + " {");

            foreach (DataColumn col in dt.Columns)
            {
                sb.AppendLine("    public " + GetDataTypeName(col) + " " + col.ColumnName + " {get; set;}");
            }
            sb.AppendLine("}");
            return sb;
        }
        #endregion
        #region CreateDALCode 生成DAL不带命名空间
        /// <summary>
        /// 生成DAL不带命名空间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public StringBuilder CreateDALCode(string tableName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            string cName = ":MsSqlDALConf";
            if (sqltype == (int)SQLType.MySql)
            {
                cName = ":MySqlDALConf";
            }
            sb.AppendLine("public  class  " + tableName + "DAL " + cName + ", I" + tableName + "DAL{");
            sb.AppendLine("");
            sb.AppendLine("        public " + tableName + "DAL() : base() { }");
            sb.AppendLine("        public " + tableName + "DAL(string name) : base(name) { }");
            GetAllDAL(tableName, dt, sb,"");
            sb.AppendLine("}");
            return sb;
        }

        private void GetAllDAL(string tableName, DataTable dt, StringBuilder sb,string otherStr="")
        {
            CreateToModel(tableName, dt, sb, otherStr);
            CreateInsert(tableName, dt, sb, otherStr);
            CreateDeleteById(tableName, dt, sb, otherStr);
            CreateUpdate(tableName, dt, sb, otherStr);
            CreateGetById(tableName, dt, sb, otherStr);
            CreateListAll(tableName, dt, sb, otherStr);
            CreateListByWhere(tableName, sb, otherStr);
            CreateListByPage(tableName, dt, sb, otherStr);
        }


        #endregion
        #region CreateDALCode 生成DAL带命名空间
        /// <summary>
        /// 生成DAL带命名空间
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="strNamespace"></param>
        /// <returns></returns>
        public StringBuilder CreateDALCode(string tableName, DataTable dt, string strNamespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using "+ strNamespace + ".IDAL.Bases;");
            sb.AppendLine("using " + strNamespace + ".DAL.Bases;");
            sb.AppendLine("using "+ strNamespace + ".Models;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Data;");
            string sqluse = sqltype == 2 ? "using MySql.Data.MySqlClient;" : "using System.Data.SqlClient;";
            sb.AppendLine(sqluse);
            sb.AppendLine("");
            sb.AppendLine("namespace " + strNamespace + ".DAL {");
            string cName = ":MsSqlDALConf";
            if (sqltype == (int)SQLType.MySql)
            {
                cName = ":MySqlDALConf";
            }
            sb.AppendLine("public  class  " + tableName + "DAL " + cName + ", I"+ tableName + "DAL{");
            sb.AppendLine("");
            sb.AppendLine("        public "+ tableName + "DAL() : base() { }");
            sb.AppendLine("        public " + tableName + "DAL(string name) : base(name) { }");

            GetAllDAL(tableName, dt, sb, "    ");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb;
        }
        #endregion
        #region CreateToModel 生成ToModel
        /// <summary>
        /// 生成ToModel
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateToModel(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {

            sb.AppendLine(hasNamespace + "    public  " + tableName + " ToModel(DataRow row) {");
            sb.AppendLine(hasNamespace + "        return GenericSQLGenerator.ToModel<" + tableName + ">(row);");
            sb.AppendLine(hasNamespace + "    }");//ToModel
            sb.AppendLine("");
        }
        #endregion
        #region CreateListAll 生成ListAll

        /// <summary>
        /// 生成ListAll
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateListAll(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 获得所有记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <returns>" + tableName + "类的对象的枚举</returns>");
            sb.AppendLine(hasNamespace + "    IEnumerable<" + tableName + ">  IBaseDAL< "+ tableName + " >.ListAll() {");
            string[] colNames = GetColumnNames(dt);
             colNames = (from a in colNames select (leftStr + a + rightStr)).ToArray();
            sb.AppendLine(hasNamespace + "        DataTable dt = this.SqlHelper.ExecuteDataTable(\"SELECT " + string.Join(", ", colNames) + " FROM " + leftStr + tableName + rightStr + ";\");");
            sb.AppendLine(hasNamespace + "         return Helper.GenericSQLGenerator.ToList<" + tableName + ">(dt);");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion
        #region CreateInsert生成Insert
        /// <summary>
        /// 生成Insert
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateInsert(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string[] colNames = GetColumnNames(dt);
            string[] colNamesTemp = new string[colNames.Length];
            for (int i = 0; i < colNamesTemp.Length; i++)
            {
                colNamesTemp[i] = leftStr + colNames[i] + rightStr;
            }
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 插入一条记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"model\">" + tableName + "类的对象</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>object 主键</returns>");
            sb.AppendLine(hasNamespace + "    object IBaseDAL< "+tableName+" >.Insert(" + tableName + " model) {");
            sb.AppendLine(hasNamespace + "        object obj;");
            if ((!(dt.PrimaryKey?[0].AutoIncrement) )?? false)
            {
                sb.AppendLine(hasNamespace + "       obj = this.SqlHelper.ExecuteScalar(@\"INSERT INTO " + leftStr + tableName + rightStr + "(" + string.Join(", ", colNamesTemp) + ") VALUES(@" + string.Join(", @", colNames) + "); SELECT @@IDENTITY AS Id ;\"");
                GetSqlParameter(dt, sb, hasNamespace, false);
                sb.AppendLine(hasNamespace + "              );");
            }
            else
            {
                var ls = ((string[])colNames.Clone()).ToList();
                ls.Remove(dt.PrimaryKey[0].ColumnName);
                string[] nullIdNames =ls.ToArray();
                ls.Clear();
                ls = ((string[])colNamesTemp.Clone()).ToList();
                ls.Remove(leftStr + dt.PrimaryKey[0].ColumnName + rightStr);
                string[] nullIdNamesTemp = ls.ToArray();
                sb.AppendLine(hasNamespace + "          obj =this.SqlHelper.ExecuteScalar(@\"INSERT INTO " + leftStr + tableName + rightStr + "(" + string.Join(", ", nullIdNamesTemp) + ") VALUES(@" + string.Join(", @", nullIdNames) + "); SELECT @@IDENTITY AS Id ;\"");
                GetSqlParameter(dt, sb, hasNamespace, true);
                sb.AppendLine(hasNamespace + "              );");
            }
            sb.AppendLine(hasNamespace + "      return obj;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion
        #region CreateUpdate 生成Update
        /// <summary>
        /// 生成Update
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateUpdate(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 更新一条记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"model\">" + tableName + "类的对象</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>更新是否成功</returns>");
            sb.AppendLine(hasNamespace + "    bool IBaseDAL< "+tableName+" >.Update(" + tableName + " model) {");
            sb.AppendLine(hasNamespace + "        int count = this.SqlHelper.ExecuteNonQuery(\"UPDATE " + leftStr + tableName + rightStr + " SET " + string.Join(", ", GetColumnNamesUpdate(dt)) + " WHERE " + leftStr + dt.Columns[0].ColumnName + rightStr + "=@" + dt.Columns[0].ColumnName + ";\"");
            GetSqlParameter(dt, sb, hasNamespace, false);
            sb.AppendLine(hasNamespace + "        );");
            sb.AppendLine(hasNamespace + "    return count > 0;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion
        #region CreateGetById 生成GetById
        /// <summary>
        /// 生成GetById
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateGetById(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            string heP = helper;
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 获得一条记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"Id\">主键</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>" + tableName + "类的对象</returns>");
            sb.AppendLine(hasNamespace + "    " + tableName + " IBaseDAL< "+tableName+" >.GetById(" + dt.Columns[0].DataType + " " + dt.Columns[0].ColumnName + ") {");
            string[] colNames = GetColumnNames(dt);
            for (int i = 0; i < colNames.Length; i++)
            {
                colNames[i] = leftStr + colNames[i] + rightStr;
            }
            sb.AppendLine(hasNamespace + "        DataTable dt = this.SqlHelper.ExecuteDataTable(\"SELECT " + string.Join(", ", colNames) + " FROM " + leftStr + tableName + rightStr + " WHERE " + leftStr + dt.Columns[0].ColumnName + rightStr + "=@" + dt.Columns[0].ColumnName + "\", new " + heP + "Parameter(\"@" + dt.Columns[0].ColumnName + "\", " + dt.Columns[0].ColumnName + "));");
            sb.AppendLine(hasNamespace + "        if (dt.Rows.Count > 1) {");
            sb.AppendLine(hasNamespace + "            throw new Exception(\"more than 1 row was found\");");
            sb.AppendLine(hasNamespace + "        }");
            sb.AppendLine(hasNamespace + "        else if (dt.Rows.Count <= 0) {");
            sb.AppendLine(hasNamespace + "            return null;");
            sb.AppendLine(hasNamespace + "        }");
            sb.AppendLine(hasNamespace + "        DataRow row = dt.Rows[0];");
            sb.AppendLine(hasNamespace + "        " + tableName + " model = ToModel(row);");
            sb.AppendLine(hasNamespace + "        return model;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion
        #region CreateDeleteById 生成DeleteById
        /// <summary>
        /// 生成DeleteById
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateDeleteById(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            string heP = helper;
            helper = "Helper." + helper;
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 删除一条记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"Id\">主键</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>删除是否成功</returns>");
            sb.AppendLine(hasNamespace + "    bool IBaseDAL< "+tableName+" >.DeleteById(" + dt.Columns[0].DataType + " " + dt.Columns[0].ColumnName + ") {");
            sb.AppendLine(hasNamespace + "        int rows = this.SqlHelper.ExecuteNonQuery(\"DELETE FROM " + leftStr + tableName + rightStr + " WHERE " + leftStr + dt.Columns[0].ColumnName + rightStr + " = @" + dt.Columns[0].ColumnName + "\", new " + heP + "Parameter(\"@" + dt.Columns[0].ColumnName + "\", " + dt.Columns[0].ColumnName + "));");
            sb.AppendLine(hasNamespace + "        return rows > 0;");
            sb.AppendLine(hasNamespace + "    }");
            sb.AppendLine("");
        }
        #endregion

        public string CreateIDAL(string tableName)
        {
            return CreateIDAL(tableName, "");
        }
        public string CreateIDAL(string tableName,   string hasNamespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using "+ hasNamespace + "IDAL.Bases;");
            sb.AppendLine("using " + hasNamespace + "Models;");
            sb.AppendLine("namespace "+hasNamespace+"IDAL");
            sb.AppendLine("{");
            sb.AppendLine("     public interface I"+tableName+"DAL : IBaseDAL<"+tableName+">");
            sb.AppendLine("     {}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 生成ListByPage
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateListByPage(string tableName, DataTable dt, StringBuilder sb, string hasNamespace)
        {
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 分页查询");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"page\">页数（从1开始计数）</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"num\">每页个数（从1开始计数）</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"orderBy\">排序条件</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"isDesc\">是否降序</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"whereArr\">查询条件：例如ID=1,NAME='ADMIN'</param>");
            sb.AppendLine(hasNamespace + "    /// <returns></returns>");
            sb.AppendLine(hasNamespace + "    public IEnumerable<" + tableName + "> ListByPage(int page = 1, int num = 10, string orderBy = \"" + dt.Columns[0].ColumnName + "\", bool isDesc = true, params string[] whereArr)");
            sb.AppendLine(hasNamespace + "    {");
            sb.AppendLine(hasNamespace + "        string whereStr = \"\";");
            sb.AppendLine(hasNamespace + "        List<string> ls = new List<string>();");
            sb.AppendLine(hasNamespace + "        foreach (var v in whereArr) { if (v != null && v != \"\") { ls.Add(v); } }");
            sb.AppendLine(hasNamespace + "        whereArr = ls.ToArray();");
            sb.AppendLine(hasNamespace + "        if (num < 1 || page < 1) { return null; }");
            string pagetemp1 = sqltype == 2 ? "        if (whereArr != null && whereArr.Length > 0) { whereStr = \" and \" + string.Join(\" and \", whereArr); }" : "        if (whereArr != null && whereArr.Length > 0) { whereStr = \" and a.\" + string.Join(\" and a.\", whereArr); }";
            sb.AppendLine(hasNamespace + pagetemp1);
            sb.AppendLine(hasNamespace + "        if (isDesc) { orderBy += \" desc\"; }");
            string pagetemp2 = sqltype == 2 ? "        DataTable dt = this.SqlHelper.ExecuteDataTable(string.Format(@\"SELECT * FROM " + leftStr + tableName + rightStr + " WHERE (1=1) {0} ORDER BY {1}  LIMIT {2}, {3};\" , whereStr,orderBy,  ((page -1)* num), num));" : "        DataTable dt = this.SqlHelper.ExecuteDataTable(string.Format(@\"SELECT b.* FROM ( SELECT  a.*, ROW_NUMBER () OVER (ORDER BY a.{0} ) AS RowNumber FROM  " + leftStr + tableName + rightStr + " AS a WHERE (1 = 1) {1}) AS b WHERE  RowNumber BETWEEN {2} AND {3} ORDER BY b.{0}\" , orderBy, whereStr, ((page-1) * num + 1), page * num));";
            sb.AppendLine(hasNamespace + pagetemp2);
            sb.AppendLine(hasNamespace + "        return Helper.GenericSQLGenerator.ToList<"+tableName+">(dt);");
            sb.AppendLine(hasNamespace + "    }");
        }
        #region CreateListByWhere 生成ListByWhere
        /// <summary>
        /// 生成ListByWhere
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sb"></param>
        /// <param name="hasNamespace"></param>
        public void CreateListByWhere(string tableName, StringBuilder sb, string hasNamespace)
        {
            sb.AppendLine(hasNamespace + "    /// <summary>");
            sb.AppendLine(hasNamespace + "    /// 通过条件获得满足条件的记录");
            sb.AppendLine(hasNamespace + "    /// </summary>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"model\">" + tableName + "类的对象</param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"whereStr\">其他的sql 语句  </param>");
            sb.AppendLine(hasNamespace + "    /// <param name=\"fields\">需要的条件的字段名</param>");
            sb.AppendLine(hasNamespace + "    /// <returns>满足条件的记录</returns>");
            sb.AppendLine(hasNamespace + "     IEnumerable<" + tableName + "> IBaseDAL< "+tableName+" >.ListByWhere(" + tableName + " model,string whereStr, params string[] fields)");
            sb.AppendLine(hasNamespace + "     {");
            string sqlpar = sqltype == 2 ? "MySqlParameter" : "SqlParameter";
            sb.AppendLine(hasNamespace + "         List<" + sqlpar + "> lsParameter = new List<" + sqlpar + ">();");
            sb.AppendLine(hasNamespace + "         string str = Helper.GenericSQLGenerator.GetWhereStr<" + tableName + ">(model, \"" + tableName + "\", out lsParameter, fields);");
            sb.AppendLine(hasNamespace + "         if(whereStr!=null&&whereStr.Trim().Length>0){str=str+\" and \"+whereStr;}");
            sb.AppendLine(hasNamespace + "         " + sqlpar + "[] sqlparm = lsParameter.ToArray();");
            sb.AppendLine(hasNamespace + "         DataTable dt = this.SqlHelper.ExecuteDataTable(str, sqlparm);");
            sb.AppendLine(hasNamespace + "         return Helper.GenericSQLGenerator.ToList<" + tableName + ">(dt);");
            sb.AppendLine(hasNamespace + "     }");
            sb.AppendLine("");
        }
        #endregion
    }
}
