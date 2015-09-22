using System.Collections.Generic;

namespace namespace MyProject.IDAL.Bases
{
    public   interface IBaseDAL<T> where T :class,new ()
    {
       
        /// <summary>
        /// ToModel
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        T ToModel(System.Data.DataRow row);

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <param name="model">Hlt_CardInfo类的对象</param>
        /// <returns>object 主键</returns>
        object Insert(T model);

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>删除是否成功</returns>
        bool DeleteById(string id);


        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="model">Hlt_CardInfo类的对象</param>
        /// <returns>更新是否成功</returns>
        bool Update(T model);

        /// <summary>
        /// 获得一条记录
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>Hlt_CardInfo类的对象</returns>
        T GetById(string id);


        /// <summary>
        /// 获得所有记录
        /// </summary>
        /// <returns>Hlt_CardInfo类的对象的枚举</returns>
        IEnumerable<T> ListAll();

        /// <summary>
        /// 通过条件获得满足条件的记录
        /// </summary>
        /// <param name="model">Hlt_CardInfo类的对象</param>
        /// <param name="whereStr">其他的sql 语句  </param>
        /// <param name="fields">需要的条件的字段名</param>
        /// <returns>满足条件的记录</returns>
        IEnumerable<T> ListByWhere(T model, string whereStr, params string[] fields);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page">页数（从1开始计数）</param>
        /// <param name="num">每页个数（从1开始计数）</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="isDesc">是否降序</param>
        /// <param name="whereArr">查询条件：例如ID=1,NAME='ADMIN'</param>
        /// <returns></returns>
        IEnumerable<T> ListByPage(int page = 1, int num = 10, string orderBy = "id", bool isDesc = true, params string[] whereArr);
    }
}