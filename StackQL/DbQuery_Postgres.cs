using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace StackQL
{
    public partial class DbQuery
    {
        private NpgsqlCommand postgresCommand(NpgsqlConnection sqlConnection, int limit, NpgsqlTransaction sqlTransaction)
        {
            List<string> command = new List<string>();
            DbParams args = new DbParams("T");

            // SELECT
            command.Add("SELECT");
            if (_select.Count > 0)
            {
                command.Add(Db.RenderArray(args, _select));
            }
            else
            {
                command.Add("*");
            }

            // FROM
            command.Add("FROM [" + _table + "]");

            // WHERE
            if (_where != null)
            {
                command.Add("WHERE " + _where.Render(args));
            }

            // GROUP BY
            if (_groupBy.Count > 0)
            {
                command.Add("GROUP BY " + Db.RenderArray(args, _groupBy));
            }

            // ORDER BY
            if (_orderBy.Count > 0)
            {
                command.Add("ORDER BY " + Db.RenderArray(args, _orderBy));
            }

            // LIMIT
            if (limit > 0)
            {
                command.Add("LIMIT " + limit);
            }

            // RUN
            NpgsqlCommand sqlCommand = sqlConnection.CreateCommand();
            if (_timeoutInSecs > 0)
            {
                sqlCommand.CommandTimeout = _timeoutInSecs;
            }

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

            return sqlCommand;
        }

        public int QueryScalar(NpgsqlConnection sqlConnection, NpgsqlTransaction sqlTransaction = null)
        {
            if (_select.Count == 0)
            {
                _select.Add(Db.Func("COUNT", DbColumn.ALL));
            }

            DataTable table = QueryTable(sqlConnection, sqlTransaction);
            foreach (DataRow row in table.Rows)
            {
                if (!row.IsNull(0))
                {
                    return Convert.ToInt32(row[0]);
                }
                break;
            }
            return 0;
        }

        public DataTable QueryTable(NpgsqlConnection sqlConnection, NpgsqlTransaction sqlTransaction = null)
        {
            DataTable dataTable = new DataTable();
            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter())
            {
                adapter.SelectCommand = postgresCommand(sqlConnection, _limit, sqlTransaction);
                adapter.Fill(dataTable);
            }
            return dataTable;
        }

        public DataRow QueryFirst(NpgsqlConnection sqlConnection, NpgsqlTransaction sqlTransaction = null)
        {
            DataTable dataTable = new DataTable();
            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter())
            {
                adapter.SelectCommand = postgresCommand(sqlConnection, 1, sqlTransaction);
                adapter.Fill(dataTable);
            }
            foreach (DataRow row in dataTable.Rows)
            {
                return row;
            }
            return null;
        }
    }
}
