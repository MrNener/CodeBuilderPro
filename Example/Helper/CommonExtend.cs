using System;

namespace MyProject.Helper
{
    public static  class CommonExtend
    {
        /// <summary>
        /// null 转换为DBNull
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToDBValue( object value)
        {
            return value == null ? DBNull.Value : value;
        }
        /// <summary>
        /// DBNull转换为null
        /// </summary>
        /// <param name="dbValue"></param>
        /// <returns></returns>
        public static object FromDBValue( object dbValue)
        {
            return dbValue == DBNull.Value ? null : dbValue;
        }
    }
}