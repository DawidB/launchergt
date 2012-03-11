using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using InsERT;
using SqlHelperLib;

namespace SqlHelperGTLib
{
    public class SqlHelperGTBase : SqlHelperBase
    {
        public SqlHelperGTBase() { }

        public bool Initialize(Baza _baza)
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

                //konwertujemy instancję obiektu połączenia;
                _sqlConn = (SqlConnection)_baza.PolaczenieAdoNet;

                //sprawdzamy, czy nowe połączenie jest otwarte
                if (_sqlConn.State == ConnectionState.Open)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
                return false;
            }
            finally
            {
                if (_baza != null)
                {
                    Marshal.ReleaseComObject(_baza);
                    _baza = null;
                }
            }            
        }
    }
}
