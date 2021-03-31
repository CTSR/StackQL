using MySqlConnector;
using Npgsql;
using System;
using System.Collections.Generic;

namespace StackQL
{
    public class DbUpdate
    {
        private string _table;
        private List<SetItem> _sets;
        private DbEquation _where;

        private class SetItem : DbElement
        {
            public DbColumn column;
            public object value;

            public SetItem(DbColumn column, object value)
            {
                this.column = column;
                this.value = value;
            }

            public override string Render(DbParams args)
            {
                if (value is DbElement)
                {
                    return column.Render(args) + "=(" + (value as DbElement).Render(args) + ")";
                }
                else
                {
                    string key = args.Add(value);
                    return column.Render(args) + "=@" + key;
                }
            }
        }

        public DbUpdate(string table)
        {
            _table = table;
            _sets = new List<SetItem>();
            _where = null;
        }

        public DbUpdate Set(DbColumn column, object value)
        {
            _sets.Add(new SetItem(column, value));
            return this;
        }

        public DbUpdate Set(string column, object value)
        {
            _sets.Add(new SetItem(new DbColumn(column), value));
            return this;
        }

        /// <summary>
        /// 當 condition 成立時才執行 set
        /// </summary>
        public DbUpdate Set(DbColumn column, object value, bool condition)
        {
            if (condition)
            {
                _sets.Add(new SetItem(column, value));
            }
            return this;
        }

        /// <summary>
        /// 當 condition 成立時才執行 set
        /// </summary>
        public DbUpdate Set(string column, object value, bool condition)
        {
            if (condition)
            {
                _sets.Add(new SetItem(new DbColumn(column), value));
            }
            return this;
        }

        public DbUpdate Where(DbEquation where)
        {
            _where = where;
            return this;
        }

        public int Update(NpgsqlConnection sqlConnection, NpgsqlTransaction sqlTransaction = null)
        {
            if (_sets.Count == 0)
            {
                throw new Exception("未指定寫入資料");
            }

            List<string> command = new List<string>();
            DbParams args = new DbParams("T");

            // INSERT
            command.Add("UPDATE " + _table);

            // COLUMNS
            command.Add("SET " + Db.RenderArray(args, _sets));

            // WHERE
            if (_where != null)
            {
                if (!_where.Ignored)
                {
                    command.Add("WHERE " + _where.Render(args));
                }
            }

            // RUN
            NpgsqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = string.Join(" ", command);
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

        public int Update(MySqlConnection sqlConnection, MySqlTransaction sqlTransaction = null)
        {
            if (_sets.Count == 0)
            {
                throw new Exception("未指定寫入資料");
            }

            List<string> command = new List<string>();
            DbParams args = new DbParams("T");

            // INSERT
            command.Add("UPDATE " + _table);

            // COLUMNS
            command.Add("SET " + Db.RenderArray(args, _sets));

            // WHERE
            if (_where != null)
            {
                command.Add("WHERE " + _where.Render(args));
            }

            // RUN
            MySqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = string.Join(" ", command);
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
    }
}
