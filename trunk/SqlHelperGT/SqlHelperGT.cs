namespace SqlHelperGTLib
{
    public sealed class SqlHelperGT
    {
        private static SqlHelperGTBase _instance = null;
        private static readonly object _padlock = new object();

        SqlHelperGT()
        {
        }

        public static SqlHelperGTBase Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new SqlHelperGTBase();
                    return _instance;
                }
            }
        }
    }
}
