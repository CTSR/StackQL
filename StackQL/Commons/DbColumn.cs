namespace StackQL
{
    public class DbColumn : DbElement
    {
        private string _column;
        private string _postfix;

        /// <summary>
        /// *
        /// </summary>
        public static readonly object ALL = new object();

        public DbColumn(string column)
        {
            _column = column;
            _postfix = null;
        }

        public DbColumn(string column, string postfix)
        {
            _column = column;
            _postfix = postfix;
        }

        public DbColumn DESC
        {
            get
            {
                return new DbColumn(_column, "DESC");
            }
        }

        public string Text
        {
            get
            {
                return ToString();
            }
        }

        override public string Render(DbParams args)
        {
            return ToString();
        }

        public override string ToString()
        {
            return _column + ((string.IsNullOrEmpty(_postfix)) ? "" : " " + _postfix);
        }
    }
}
