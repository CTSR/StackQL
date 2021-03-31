using MySqlConnector;
using Npgsql;
using System;
using System.Collections.Generic;

namespace StackQL
{
    public class DbInsert
    {
        private string _table;
        private List<DbColumn> _cols;
        private List<object> _vals;

        public DbInsert(string table)
        {
            _table = table;
            _cols = new List<DbColumn>();
            _vals = new List<object>();
        }

        public DbInsert ColumnValue(string column, object value)
        {
            _cols.Add(new DbColumn(column));
            _vals.Add(value);
            return this;
        }

        public DbInsert ColumnValue(DbColumn column, object value)
        {
            _cols.Add(column);
            _vals.Add(value);
            return this;
        }

        public int Insert(NpgsqlConnection sqlConnection, NpgsqlTransaction sqlTransaction = null)
        {
            if (_cols == null || _vals == null)
            {
                throw new Exception("未指定插入資料");
            }
            if (_cols.Count != _vals.Count)
            {
                throw new Exception("插入資料與欄位數量不一致");
            }

            // RUN
            NpgsqlCommand sqlCommand = sqlConnection.CreateCommand();
            DbParams args = new DbParams("T");
            sqlCommand.CommandText = string.Format(
                "INSERT INTO {0} ({1}) VALUES({2})",
                _table,
                Db.RenderArray(args, _cols),
                Db.RenderArray(args, _vals)
            );
            if (sqlTransaction != null)
            {
                sqlCommand.Transaction = sqlTransaction;
            }

            Dictionary<string, object> argsAll = args.DumpAll();
            foreach (string key in argsAll.Keys)
            {
                sqlCommand.Parameters.AddWithValue(key, argsAll[key]);
            }

            return sqlCommand.ExecuteNonQuery();
        }

        public int Insert(MySqlConnection sqlConnection, MySqlTransaction sqlTransaction = null)
        {
            if (_cols == null || _vals == null)
            {
                throw new Exception("未指定插入資料");
            }
            if (_cols.Count != _vals.Count)
            {
                throw new Exception("插入資料與欄位數量不一致");
            }

            // RUN
            DbParams args = new DbParams("T");
            MySqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = string.Format(
                "INSERT INTO {0} ({1}) VALUES({2})",
                _table,
                Db.RenderArray(args, _cols),
                Db.RenderArray(args, _vals)
            );
            if (sqlTransaction != null)
            {
                sqlCommand.Transaction = sqlTransaction;
            }

            Dictionary<string, object> argsAll = args.DumpAll();
            foreach (string key in argsAll.Keys)
            {
                object value = argsAll[key];
                if(value is byte[])
                {
                    MySqlParameter blobParameter = new MySqlParameter(key, MySqlDbType.Blob, ((byte[])value).Length);
                    blobParameter.Value = value;
                    sqlCommand.Parameters.Add(blobParameter);
                }
                else
                {
                    sqlCommand.Parameters.AddWithValue(key, argsAll[key]);
                }
            }

            return sqlCommand.ExecuteNonQuery();
        }
    }
}
