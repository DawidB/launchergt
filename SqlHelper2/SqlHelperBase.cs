using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SqlHelperLib
{
    public class SqlHelperBase
    {
        protected SqlConnection _sqlConn = null;
        /// <summary>
        /// Accessor which checks if old SqlConnection is closed (if not then it closes connection) and sets new value.
        /// </summary>
        public SqlConnection SqlConn
        {
            get { return _sqlConn; }
            set 
            {
                if (_sqlConn != null)
                {
                    try
                    {
                        if (_sqlConn.State != ConnectionState.Closed)
                            _sqlConn.Close();
                    }
                    catch { }
                }

                _sqlConn = value;
            }
        }

        /// <summary>
        /// When set to true, all errors will be displayed by MessageBox.
        /// </summary>
        public bool ShowErrorMessages { get; set; }

        public void ShowErrorMessage(string _methodName)
        {
            if (ShowErrorMessages)
                MessageBox.Show("Błąd w metodzie " + _methodName + "(). Komunikat:\n\n" + _errorMsg, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected string _errorMsg = "";
        public string ErrorMsg { get { return _errorMsg; } }

        protected string _lastQuery = "";
        public string LastQuery { get { return _lastQuery; } } 


        public SqlHelperBase() 
        {
        }

        /// <summary>
        /// Metoda inicjalizująca połączenie na podstawie 
        /// </summary>
        /// <param name="_sqlConnStr"></param>
        /// <returns></returns>
        public bool Initialize(string _sqlConnStr)
        {
            try
            {
                //sprawdzamy, czy połączenie zostalo zamknięte; jeśli nie - zamykamy je;
                if (_sqlConn != null)
                {
                    try
                    {
                        if (_sqlConn.State != ConnectionState.Closed)
                            _sqlConn.Close();
                    }
                    catch { }
                }

                //tworzymy nową instancję obiektu połączenia;
                _sqlConn = new SqlConnection(_sqlConnStr);

                //sprawdzamy, czy nowe połączenie jest otwarte
                _sqlConn.Open();
                if (_sqlConn.State == ConnectionState.Open)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
                ShowErrorMessage("Initialize");
                return false;
            }
        }

        /// <summary>
        /// Metoda modyfikująca obiekt wejściowy typu SqlDataReader.
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_sqlDataReader"></param>
        public bool ExecuteReader(string _sqlQuery, ref SqlDataReader _sqlDataReader)
        {
            SqlCommand _sqlCommand = null;
            _sqlDataReader = null;
            
            try
            {
                if (_sqlConn.State != ConnectionState.Open)
                    return false;
                
                _sqlCommand = new SqlCommand(_sqlQuery, _sqlConn);
                _sqlCommand.ExecuteNonQuery();
                _sqlDataReader = _sqlCommand.ExecuteReader();
                return true;
            }
            catch(Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ExecuteReader");
                return false;              
            }
        }

        /// <summary>
        /// Metoda modyfikująca obiekt wejściowy typu SqlDataAdapter.
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_sqlDataAdapter"></param>
        public bool ExecuteAdapter(string _sqlQuery, ref SqlDataAdapter _sqlDataAdapter)
        {
            _sqlDataAdapter = null;

            try
            {
                if (_sqlConn.State != ConnectionState.Open)
                    return false;

                _sqlDataAdapter = new SqlDataAdapter(_sqlQuery, _sqlConn);                
                return true;
            }
            catch (Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ExecuteAdapter");
                return false;
            }
        }

        /// <summary>
        /// Metoda modyfikująca obiekt wejściowy typu DataSet.
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_dataSet"></param>
        public bool ExecuteDataSet(string _sqlQuery, ref DataSet _dataSet)
        {
            SqlDataAdapter _sqlDataAdapter = null;

            try
            {
                if (_sqlConn.State != ConnectionState.Open)
                    return false;

                _dataSet = new DataSet();
                _sqlDataAdapter = new SqlDataAdapter(_sqlQuery, _sqlConn);
                _sqlDataAdapter.Fill(_dataSet);
                return true;
            }
            catch (Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ExecuteDataSet");
                return false;
            }
        }

        /// <summary>
        /// Metoda zwracająca tabelę z danymi, zwróconą w wyniku wykonania zapytania.
        /// Opcjonalnie użytkownik może podać indeks tabeli, zwracanej w wyniku wykonania zapytania.
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_dataTable"></param>
        /// <param name="_tabId"></param>
        public bool ExecuteDataTable(string _sqlQuery, ref DataTable _dataTable, int _tabId = 0)
        {
            _dataTable = null;

            DataSet _ds = null;
            ExecuteDataSet(_sqlQuery, ref _ds);

            if (_ds != null)
                if (_ds.Tables.Count > _tabId)
                {
                    _dataTable = _ds.Tables[_tabId];                    
                    return true;
                }

            return false;
        }
        
        /// <summary>
        /// Metoda zwracająca wynik zapytania.
        /// W przypadku wykrycia nieotwartego połączenia, zwracana jest wartośc -1.
        /// W przypadku wystąpienia błędu, zwracana jest wartośc -2.
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <returns></returns>
        public int ExecuteQuery(string _sqlQuery)
        {
            SqlCommand _sqlCommand = null;

            try
            {
                if (_sqlConn.State != ConnectionState.Open)
                    return -1;

                _sqlCommand = new SqlCommand(_sqlQuery, _sqlConn);
                return _sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ExecuteQuery");
                return -2;
            }
        }

        /// <summary>
        /// Metoda zwracająca wartość typu "object", pochodzącą z wybranej kolumny.
        /// W przypadku nieudanego wykonania zapytania zwracana jest wartość "null".
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_valId"></param>
        /// <returns></returns>
        public object ReturnObj(string _sqlQuery, int _colId = 0)
        {
            DataTable _dt = null;

            try
            {
                ExecuteDataTable(_sqlQuery, ref _dt, 0);
                if (_dt.Rows.Count > 0)
                    if (_dt.Rows[0].ItemArray.Length > _colId)
                        if (_dt.Rows[0].ItemArray[_colId] != DBNull.Value)
                            return _dt.Rows[0].ItemArray[_colId];

                return null;
            }
            catch (Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ReturnObj");
                return null;
            }
        }

        /// <summary>
        /// Metoda zwracająca wartość typu "int", pochodzącą z wybranej kolumny.
        /// W przypadku nieudanego wykonania zapytania zwracana jest wartość "0".
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_colId"></param>
        /// <returns></returns>
        public int ReturnInt(string _sqlQuery, int _colId = 0)
        {
            try
            {
                object _val = ReturnObj(_sqlQuery, _colId);

                if (_val == null)
                    return 0;
                else
                    return Convert.ToInt32(_val);
            }
            catch(Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ReturnInt");
                return 0;
            }
        }

        /// <summary>
        /// Metoda zwracająca wartość typu "decimal", pochodzącą z wybranej kolumny.
        /// W przypadku nieudanego wykonania zapytania zwracana jest wartość "0".
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_colId"></param>
        /// <returns></returns>
        public decimal ReturnDec(string _sqlQuery, int _colId = 0)
        {
            try
            {
                object _val = ReturnObj(_sqlQuery, _colId);

                if (_val == null)
                    return 0;
                else
                    return Convert.ToDecimal(_val);
            }
            catch (Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ReturnDec");
                return 0;
            }
        }
        
        /// <summary>
        /// Metoda zwracająca wartość typu "string", pochodzącą z wybranej kolumny.
        /// W przypadku nieudanego wykonania zapytania zwracany jest pusty ciąg znaków.
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_colId"></param>
        /// <returns></returns>
        public string ReturnStr(string _sqlQuery, int _colId = 0)
        {
            try
            {
                object _val = ReturnObj(_sqlQuery, _colId);

                if (_val == null)
                    return string.Empty;
                else
                    return _val.ToString();
            }
            catch (Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ReturnStr");
                return string.Empty;
            }
        }

        /// <summary>
        /// Metoda zwracająca wartość typu "bool", pochodzącą z wybranej kolumny.
        /// W przypadku nieudanego wykonania zapytania zwracana jest wartość "false".
        /// </summary>
        /// <param name="_sqlQuery"></param>
        /// <param name="_colId"></param>
        /// <returns></returns>
        public bool ReturnBool(string _sqlQuery, int _colId = 0)
        {
            try
            {
                object _val = ReturnObj(_sqlQuery, _colId);

                if (_val == null)
                    return false;
                else
                    return Convert.ToBoolean(_val);
            }
            catch (Exception ex)
            {
                _lastQuery = _sqlQuery;
                _errorMsg = ex.Message;
                ShowErrorMessage("ReturnBool");
                return false;
            }
        }
    }
}
