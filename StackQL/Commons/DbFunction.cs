using System.Collections.Generic;

namespace StackQL
{
    public class DbFunction : DbElement
    {
        private string _func;
        private object[] _args;

        public DbFunction(string func, params object[] args)
        {
            _func = func;
            _args = args;
        }

        override public string Render(DbParams args)
        {
            return _func + "(" + Db.RenderArray(args, _args) + ")";
        }

        static public DbFunction NOW = new DbFunction("NOW");
    }
}
