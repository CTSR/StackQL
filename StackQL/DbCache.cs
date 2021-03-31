using MySqlConnector;
using System.Collections.Generic;

namespace StackQL
{
    /// <summary>
    /// 資料庫快取工具
    /// </summary>
    public class DbCache
    {
        public DbCache(MySqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        /// <summary>
        /// 快取指定 primaryKey 的實體化物件
        /// </summary>
        /// <param name="queryDelegate">查詢方法(sqlConnection, sqlTransation, primaryKey)</param>
        /// <param name="key">primaryKey</param>
        public object DefaultCache(QueryOneDelegate queryDelegate, string key)
        {
            return Cache(_sqlConnection, queryDelegate, key);
        }

        /// <summary>
        /// 快取指定 primaryKey 的實體化物件
        /// </summary>
        /// <param name="queryDelegate">查詢方法(sqlConnection, sqlTransation, primaryKey)</param>
        /// <param name="key">primaryKey</param>
        public object Cache(MySqlConnection sqlConnection, QueryOneDelegate queryDelegate, string key)
        {
            // primaryKey 不得為空
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            lock(_cacheLock)
            {
                if(_cacheMap.ContainsKey(key))
                {
                    // 快取命中，直接回傳物件
                    return _cacheMap[key];
                }
                else
                {
                    // 快取失敗，查詢 DB 並實體化物件
                    object data = null;
                    if(sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        data = queryDelegate(sqlConnection, null, key);
                    }
                    else
                    {
                        sqlConnection.Open();
                        try
                        {
                            data = queryDelegate(sqlConnection, null, key);
                        }
                        finally
                        {
                            sqlConnection.Close();
                        }
                    }

                    // 將物件加入快取 (物件不存在時為 null)
                    if (data != null)
                    {
                        _cacheMap.Add(key, data);
                    }
                    return data;
                }
            }
        }

        /// <summary>
        /// 快取指定 primaryKey 的實體化物件
        /// </summary>
        /// <param name="queryDelegate">查詢方法(sqlConnection, sqlTransation, primaryKey)</param>
        /// <param name="keyDb">primaryKey</param>
        /// <param name="keyCache">cache Key</param>
        public object DefaultCache(QueryOneDelegate queryDelegate, string keyDb, string keyCache)
        {
            return Cache(_sqlConnection, queryDelegate, keyDb, keyCache);
        }

        /// <summary>
        /// 快取指定 primaryKey 的實體化物件
        /// </summary>
        /// <param name="queryDelegate">查詢方法(sqlConnection, sqlTransation, primaryKey)</param>
        /// <param name="keyDb">primaryKey</param>
        /// <param name="keyCache">cache Key</param>
        public object Cache(MySqlConnection sqlConnection, QueryOneDelegate queryDelegate, string keyDb, string keyCache)
        {
            // primaryKey 與 cache Key 不得為空
            if (string.IsNullOrEmpty(keyDb) || string.IsNullOrEmpty(keyCache))
            {
                return null;
            }

            lock (_cacheLock)
            {
                if (_cacheMap.ContainsKey(keyCache))
                {
                    // 快取命中，直接回傳物件
                    return _cacheMap[keyCache];
                }
                else
                {
                    // 快取失敗，查詢 DB 並實體化物件
                    sqlConnection.Open();
                    object data = null;
                    try
                    {
                        data = queryDelegate(sqlConnection, null, keyDb);
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }

                    // 將物件加入快取 (物件不存在時為 null)
                    if (data != null)
                    {
                        _cacheMap.Add(keyCache, data);
                    }
                    return data;
                }
            }
        }

        /// <summary>
        /// 檢查指定 primaryKey 已快取的實體化物件
        /// </summary>
        /// <param name="key">primaryKey</param>
        public object CheckCache(string key)
        {
            // primaryKey 不得為空
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            lock (_cacheLock)
            {
                if (_cacheMap.ContainsKey(key))
                {
                    // 快取命中，直接回傳物件
                    return _cacheMap[key];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 傳回已使用的快取容量
        /// </summary>
        public int GetCapacity()
        {
            return _cacheMap.Count;
        }

        /// <summary>
        /// 大量更新快取
        /// </summary>
        /// <param name="queryDelegate">查詢方法(sqlConnection, sqlTransation, primaryKeys)</param>
        /// <param name="keys">primaryKeys</param>
        public void UpdateCache(QuerySomeDelegate queryDelegate, IEnumerable<string> keys)
        {
            lock (_cacheLock)
            {
                // 移除現有的快取
                foreach(string key in keys)
                {
                    _cacheMap.Remove(key);
                }

                // 從 DB 查詢並實體化物件
                _sqlConnection.Open();
                Dictionary<string, object> dataMap;
                try
                {
                    dataMap = queryDelegate(_sqlConnection, null, keys);
                }
                finally
                {
                    _sqlConnection.Close();
                }

                // 加入快取
                if(dataMap != null)
                {
                    foreach (string key in dataMap.Keys)
                    {
                        _cacheMap.Add(key, dataMap[key]);
                    }
                }
            }
        }

        /// <summary>
        /// 移除現有的快取
        /// </summary>
        public void RemoveCache(string keyCache)
        {
            lock (_cacheLock)
            {
                _cacheMap.Remove(keyCache);
            }
        }

        /// <summary>
        /// 清除所有快取
        /// </summary>
        public void ClearCache()
        {
            lock (_cacheLock)
            {
                _cacheMap.Clear();
            }
        }

        public delegate object QueryOneDelegate(MySqlConnection sqlConnection, MySqlTransaction sqlTransaction, string key);
        public delegate Dictionary<string, object> QuerySomeDelegate(MySqlConnection sqlConnection, MySqlTransaction sqlTransaction, IEnumerable<string> keys);

        private MySqlConnection _sqlConnection;

        // 快取鎖
        private object _cacheLock = new object();

        // 快取表
        private Dictionary<string, object> _cacheMap = new Dictionary<string, object>();
    }
}
