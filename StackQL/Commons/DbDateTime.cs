using System;
using System.Data;

namespace StackQL
{
    /// <summary>
    /// 資料庫時間類別
    /// </summary>
    public class DbDateTime
    {
        /// <summary>
        /// 未設定
        /// </summary>
        static public DateTime EMPTY_TIME = new DateTime(1900, 1, 1);

        /// <summary>
        /// 無限期
        /// </summary>
        static public DateTime INFINITE_TIME = new DateTime(9999, 12, 31);

        /// <summary>
        /// 目前時間
        /// </summary>
        static public readonly object SERVER_NOW = new object();

        /// <summary>
        /// 將 Unix 時間戳記轉換成 DateTime
        /// </summary>
        static public DateTime FromEpochMillis(ulong epochMillis)
        {
            return EPOCH.AddMilliseconds(epochMillis);
        }

        /// <summary>
        /// 將 DateTime 轉換成 Unix 時間戳記
        /// </summary>
        static public ulong ToEpochMillis(DateTime? timeUtc)
        {
            if(timeUtc == null)
            {
                return 0;
            }
            if(timeUtc > EPOCH)
            {
                return (ulong)((((DateTime)timeUtc) - EPOCH).TotalMilliseconds);
            }
            else
            {
                return 0;
            }
        }
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 將 DB row 轉換成 DateTime
        /// </summary>
        static public DateTime? FromDb(DataRow row, string column)
        {
            if(row.IsNull(column))
            {
                return null;
            }
            return DateTime.SpecifyKind(Convert.ToDateTime(row[column]), DateTimeKind.Utc);
        }
    }
}
