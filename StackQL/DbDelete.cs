using MySqlConnector;
using Npgsql;
using System.Collections.Generic;

namespace StackQL
{
    public class DbDelete
    {
        private string _table;
        private DbEquation _where;

        public DbDelete(string table)
        {
            _table = table;
            _where = null;
        }

        public DbDelete Where(DbEquation where)
        {
            _where = where;
            return this;
        }

        public int Delete(NpgsqlConnection sqlConnection, NpgsqlTransaction sqlTransaction = null)
        {
            List<string> command = new List<string>();
            DbParams args = new DbParams("T");

            // DELETE FROM
            command.Add("DELETE FROM " + _table);

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

        public int Delete(MySqlConnection sqlConnection, MySqlTransaction sqlTransaction = null)
        {
            List<string> command = new List<string>();
            DbParams args = new DbParams("T");

            // DELETE FROM
            command.Add("DELETE FROM " + _table);

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
