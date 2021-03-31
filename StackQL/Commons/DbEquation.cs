using System;

namespace StackQL
{
    public class DbEquation : DbElement
    {
        private Op _op;
        private object _value1;
        private object _value2;

        public enum Op
        {
            Ignore = -1,
            Eq,
            Ne,
            Gt,
            Ge,
            Lt,
            Le,
            Not,
            And,
            Or,
            Like,
            NotLike,
            In,
            NotIn
        }

        public DbEquation(Op op, object value1, bool condition = true)
        {
            if(condition)
            {
                _op = op;
                _value1 = value1;
                _value2 = null;
            }
            else
            {
                _op = Op.Ignore;
                _value1 = null;
                _value2 = null;
            }
        }

        public DbEquation(object value1, Op op, object value2, bool condition = true)
        {
            if (condition)
            {
                _op = op;
                _value1 = value1;
                _value2 = value2;
            }
            else
            {
                _op = Op.Ignore;
                _value1 = null;
                _value2 = null;
            }
        }

        public bool Ignored
        {
            get
            {
                return _op == Op.Ignore;
            }
        }

        override public string Render(DbParams args)
        {
            switch (_op)
            {
                case Op.Ignore: return "";
                case Op.Eq:
                    if(_value2 == null)
                    {
                        return "(" + Db.Render(args, _value1) + " IS NULL)";
                    }
                    else
                    {
                        return "(" + Db.Render(args, _value1) + "=" + Db.Render(args, _value2) + ")";
                    }
                case Op.Ne:
                    if (_value2 == null)
                    {
                        return "(" + Db.Render(args, _value1) + " IS NOT NULL)";
                    }
                    else
                    {
                        return "(" + Db.Render(args, _value1) + "<>" + Db.Render(args, _value2) + ")";
                    }
                case Op.Gt: return "(" + Db.Render(args, _value1) + ">" + Db.Render(args, _value2) + ")";
                case Op.Ge: return "(" + Db.Render(args, _value1) + ">=" + Db.Render(args, _value2) + ")";
                case Op.Lt: return "(" + Db.Render(args, _value1) + "<" + Db.Render(args, _value2) + ")";
                case Op.Le: return "(" + Db.Render(args, _value1) + "<=" + Db.Render(args, _value2) + ")";
                case Op.Not: return "(!" + Db.Render(args, _value1) + ")";
                case Op.And: return "(" + Db.Render(args, _value1) + " AND " + Db.Render(args, _value2) + ")";
                case Op.Or: return "(" + Db.Render(args, _value1) + " OR " + Db.Render(args, _value2) + ")";
                case Op.Like: return "(" + Db.Render(args, _value1) + " LIKE " + Db.Render(args, _value2) + ")";
                case Op.NotLike: return "(" + Db.Render(args, _value1) + " NOT LIKE " + Db.Render(args, _value2) + ")";
                case Op.In: return "(" + Db.Render(args, _value1) + " IN (" + Db.Render(args, _value2) + "))";
                case Op.NotIn: return "(" + Db.Render(args, _value1) + " NOT IN (" + Db.Render(args, _value2) + "))";
                default: throw new Exception("Invalid op");
            }
        }
    }
}
