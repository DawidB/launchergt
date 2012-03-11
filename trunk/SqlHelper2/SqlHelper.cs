namespace SqlHelperLib
{
    public sealed class SqlHelper
    {
        private static SqlHelperBase _instance = null;
        private static readonly object _padlock = new object();

        SqlHelper()
        {
        }

        public static SqlHelperBase Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new SqlHelperBase();
                    return _instance;
                }
            }
        }        
    }
}
