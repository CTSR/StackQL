using System.Collections.Generic;

namespace StackQL
{
    public partial class DbQuery : DbElement
    {
        private string _table;
        private int _limit;
        private int _timeoutInSecs;
        private List<DbElement> _select;
        private List<DbElement> _orderBy;
        private List<DbElement> _groupBy;
        private DbEquation _where;

        public DbQuery(string table)
        {
            _table = table;
            _limit = 0;
            _timeoutInSecs = 0;
            _select = new List<DbElement>();
            _orderBy = new List<DbElement>();
            _groupBy = new List<DbElement>();
            _where = null;
        }

        public DbQuery Limit(int limit)
        {
            _limit = limit;
            return this;
        }

        public DbQuery Select(params DbElement[] select)
        {
            _select.AddRange(select);
            return this;
        }

        public DbQuery OrderBy(params DbElement[] orderBy)
        {
            _orderBy.AddRange(orderBy);
            return this;
        }

        public DbQuery GroupBy(params DbElement[] groupBy)
        {
            _groupBy.AddRange(groupBy);
            return this;
        }

        public DbQuery Where(DbEquation where)
        {
            _where = where;
            return this;
        }

        public DbQuery TimeoutInSecs(int timeoutInSecs)
        {
            _timeoutInSecs = timeoutInSecs;
            return this;
        }

        public override string Render(DbParams args)
        {
            List<string> command = new List<string>();

            // SELECT
            command.Add("(SELECT");
            if (_select.Count > 0)
            {
                command.Add(Db.RenderArray(args, _select));
            }
            else
            {
                command.Add("*");
            }

            // FROM
            command.Add("FROM " + _table);

            // WHERE
            if (_where != null)
            {
                if(!_where.Ignored)
                {
                    command.Add("WHERE " + _where.Render(args));
                }
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
            if (_limit > 0)
            {
                command.Add("LIMIT " + _limit);
            }
            command.Add(")");

            return string.Join(" ", command);
        }
    }
}
