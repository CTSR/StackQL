using System.Collections;
using System.Collections.Generic;

namespace StackQL
{
    public class Db
    {
        /// <summary>
        /// 輸出指令文字
        /// </summary>
        static public string Render(DbParams args, object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else if (value is byte[])
            {
                string key = args.Add(value);
                return "@" + key;
            }
            else if (value is DbElement)
            {
                return (value as DbElement).Render(args);
            }
            else if ((value is IEnumerable) && !(value is string))
            {
                return RenderArray(args, value as IEnumerable);
            }
            else if (value.Equals(DbColumn.ALL))
            {
                return "*";
            }
            else if (value.Equals(DbDateTime.SERVER_NOW))
            {
                return "NOW()";
            }
            else
            {
                string key = args.Add(value);
                return "@" + key;
            }
        }

        /// <summary>
        /// 輸出指令文字
        /// </summary>
        static public string RenderArray(DbParams args, IEnumerable values)
        {
            List<string> items = new List<string>();
            foreach (object value in (values as IEnumerable))
            {
                items.Add(Render(args, value));
            }
            return string.Join(",", items);
        }

        static public DbQuery Query(string table)
        {
            return new DbQuery(table);
        }

        static public DbInsert Insert(string table)
        {
            return new DbInsert(table);
        }

        static public DbUpdate Update(string table)
        {
            return new DbUpdate(table);
        }

        static public DbDelete Delete(string table)
        {
            return new DbDelete(table);
        }

        /// <summary>
        /// [column postfix]
        /// </summary>
        static public DbColumn Col(string column, string postfix = null)
        {
            return new DbColumn(column, postfix);
        }

        /// <summary>
        /// func(args)
        /// </summary>
        static public DbFunction Func(string func, params object[] args)
        {
            return new DbFunction(func, args);
        }

        /// <summary>
        /// op value1
        /// </summary>
        static public DbEquation Eq(DbEquation.Op op, object value1, bool condition = true)
        {
            return new DbEquation(op, value1, condition);
        }

        /// <summary>
        /// value1 op value2
        /// </summary>
        static public DbEquation Eq(object value1, DbEquation.Op op, object value2, bool condition = true)
        {
            return new DbEquation(value1, op, value2, condition);
        }

        /// <summary>
        /// [column] = value2
        /// </summary>
        static public DbEquation EqColEqVal(string column, object value2, bool condition = true)
        {
            return new DbEquation(new DbColumn(column), DbEquation.Op.Eq, value2, condition);
        }

        /// <summary>
        /// [column] = value2
        /// </summary>
        static public DbEquation EqColEqVal(DbColumn column, object value2, bool condition = true)
        {
            return new DbEquation(column, DbEquation.Op.Eq, value2, condition);
        }

        /// <summary>
        /// (eq1) AND (eq2) AND ...
        /// </summary>
        static public DbEquation EqAnds(params DbEquation[] eqs)
        {
            DbEquation eqAnds = null;
            foreach (DbEquation eq in eqs)
            {
                if(eq == null)
                {
                    continue;
                }
                if (eq.Ignored)
                {
                    continue;
                }
                if (eqAnds == null)
                {
                    eqAnds = eq;
                }
                else
                {
                    eqAnds = new DbEquation(eqAnds, DbEquation.Op.And, eq);
                }
            }
            return eqAnds;
        }

        /// <summary>
        /// (eq1) AND (eq2) AND ...
        /// </summary>
        static public DbEquation EqAnds(IEnumerable<DbEquation> eqs)
        {
            DbEquation eqAnds = null;
            foreach (DbEquation eq in eqs)
            {
                if (eq == null)
                {
                    continue;
                }
                if (eq.Ignored)
                {
                    continue;
                }
                if (eqAnds == null)
                {
                    eqAnds = eq;
                }
                else
                {
                    eqAnds = new DbEquation(eqAnds, DbEquation.Op.And, eq);
                }
            }
            return eqAnds;
        }

        /// <summary>
        /// (eq1) OR (eq2) OR ...
        /// </summary>
        static public DbEquation EqOrs(params DbEquation[] eqs)
        {
            DbEquation eqOrs = null;
            foreach (DbEquation eq in eqs)
            {
                if (eq == null)
                {
                    continue;
                }
                if (eq.Ignored)
                {
                    continue;
                }
                if (eqOrs == null)
                {
                    eqOrs = eq;
                }
                else
                {
                    eqOrs = new DbEquation(eqOrs, DbEquation.Op.Or, eq);
                }
            }
            return eqOrs;
        }

        /// <summary>
        /// (eq1) OR (eq2) OR ...
        /// </summary>
        static public DbEquation EqOrs(IEnumerable<DbEquation> eqs)
        {
            DbEquation eqOrs = null;
            foreach (DbEquation eq in eqs)
            {
                if (eq == null)
                {
                    continue;
                }
                if (eq.Ignored)
                {
                    continue;
                }
                if (eqOrs == null)
                {
                    eqOrs = eq;
                }
                else
                {
                    eqOrs = new DbEquation(eqOrs, DbEquation.Op.Or, eq);
                }
            }
            return eqOrs;
        }
    }
}
