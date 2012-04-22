using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using SqlHelperGTLib;
using SqlHelperLib;
using System.Windows.Forms;
using InsERT;

namespace LauncherGTLib
{
    [Serializable]
    public class UserSettings
    {
        #region "Variables and properties"

        public bool RememberSqlPass { get; set; }
        [XmlIgnore]
        public bool EnabledRememberSqlPass { get; set; }

        public bool RememberInsPass { get; set; }
        [XmlIgnore]
        public bool EnabledRememberInsPass { get; set; }

        public string SqlServer { get; set; }
        [XmlIgnore]
        public bool EnabledSqlServer { get; set; }

        public string SqlPort { get; set; }
        [XmlIgnore]
        public bool EnabledSqlPort { get; set; }

        public int SqlAuthentication { get; set; }
        [XmlIgnore]
        public bool EnabledSqlAuth { get; set; }

        public string SqlLogin { get; set; }
        [XmlIgnore]
        public bool EnabledSqlLogin { get; set; }

        private string _sqlPassword = "";
        [XmlIgnore]
        public string SqlPassword 
        {
            get 
            {
                if (_sqlPassword == "")
                    return "";
                else
                    return LauncherGT.Instance.AesEnc.GetDecryptedString(_sqlPassword); 
            }
            set
            {
                if (value == "")
                    _sqlPassword = "";
                else
                    _sqlPassword = LauncherGT.Instance.AesEnc.GetEncryptedString(value); 
            }
        }
        public string EncSqlPassword
        {
            get
            {
                if (RememberSqlPass)
                    return _sqlPassword;
                else
                    return "";
            }
            set { _sqlPassword = value; }
        }
        [XmlIgnore]
        public bool EnabledSqlPassword { get; set; }

        public string SqlDatabase { get; set; }
        [XmlIgnore]
        public bool EnabledSqlDatabase { get; set; }

        public int InsAppType { get; set; }
        [XmlIgnore]
        public bool EnabledInsAppType { get; set; }

        public int NavStartMode { get; set; }
        [XmlIgnore]
        public bool EnabledNavStartMode { get; set; }

        public string NavFilePath { get; set; }
        [XmlIgnore]
        public bool EnabledNavFilePathMode { get; set; }

        public int InsAppLoginMode { get; set; }
        [XmlIgnore]
        public bool EnabledInsAppLoginMode { get; set; }

        public int InsAppStartMode { get; set; }
        [XmlIgnore]
        public bool EnabledInsAppStartMode { get; set; }

        private int _insLoginId = 0;
        public int InsLoginId 
        {
            get { return _insLoginId; } 
            set 
            { 
                _insLoginId = value; 

                //wyszukujemy nazwę operatora dla wybranego id
                if (_dtInsOperators == null)
                    LoadInsOperators();
                if (_dtInsOperators != null)
                    foreach (DataRow _dr in _dtInsOperators.Rows)
                        if ((int)_dr.ItemArray[0] == _insLoginId)
                        {
                            _insLoginName = _dr.ItemArray[1].ToString();
                            break;
                        }
            } 
        }

        private string _insLoginName = "";
        public string InsLoginName { get { return _insLoginName; } }
        [XmlIgnore]
        public bool EnabledInsLoginName { get; set; }

        private string _insPassword = "";
        [XmlIgnore]
        public string InsPassword
        {
            get 
            {
                if (_insPassword == "")
                    return _insPassword;
                else
                {
                    try
                    {
                        return LauncherGT.Instance.AesEnc.GetDecryptedString(_insPassword);
                    }
                    catch 
                    {
                        return "";
                    }
                }
            }
            set
            {
                if (value == "")
                    _insPassword = "";
                else
                    _insPassword = LauncherGT.Instance.AesEnc.GetEncryptedString(value);
            }
        }
        public string EncInsPassword 
        {
            get
            {
                if (RememberInsPass)
                    return _insPassword;
                else
                    return "";
            }
            set { _insPassword = value; }
        }
        [XmlIgnore]
        public bool EnabledInsPassword { get; set; }

        private DataTable _dtDatabases = null,
            _dtInsOperators = null;
        public DataTable DTDatabases { get { return _dtDatabases; } }
        public DataTable DTInsOperators { get { return _dtDatabases; } }
        
        public string SqlConnString
        {
            get
            {
                if (string.IsNullOrEmpty(SqlServer))
                    return "";

                string _sqlConnString = "Data Source=" + SqlServer;

                if (!string.IsNullOrEmpty(SqlPort))
                    _sqlConnString += "," + SqlPort;

                _sqlConnString += ";";

                if (string.IsNullOrEmpty(SqlDatabase))
                    _sqlConnString += "Initial catalog=master;";
                else
                    _sqlConnString += "Initial catalog=" + SqlDatabase + ";";

                if (SqlAuthentication == 1)
                    _sqlConnString += "Integrated Security=SSPI;";
                else
                    _sqlConnString += "User Id=" + SqlLogin + ";Password=" + SqlPassword + ";";

                return _sqlConnString;
            }
        }

        #endregion


        public UserSettings()
        {
            EnabledInsAppLoginMode = true;
            EnabledInsAppStartMode = true;
            EnabledInsAppType = true;
            EnabledInsLoginName = true;
            EnabledInsPassword = true;
            EnabledNavFilePathMode = true;
            EnabledNavStartMode = true;
            EnabledRememberInsPass = true;
            EnabledRememberSqlPass = true;
            EnabledSqlAuth = true;
            EnabledSqlDatabase = true;
            EnabledSqlLogin = true;
            EnabledSqlPassword = true;
            EnabledSqlPort = true;
            EnabledSqlServer = true;
        }

        public DataTable LoadDatabases()
        {
            try
            {
                if (string.IsNullOrEmpty(SqlConnString))
                    return null;

                string _sqlQuery = @"
SELECT name
FROM master.sys.databases
WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb', 'tempdb')
ORDER BY name";

                if (SqlHelper.Instance.Initialize(SqlConnString))
                    SqlHelper.Instance.ExecuteDataTable(_sqlQuery, ref _dtDatabases, 0);
            }
            catch { }

            return _dtDatabases;
        }

        public DataTable LoadInsOperators()
        {
            try
            {
                if (string.IsNullOrEmpty(SqlConnString))
                    return null;

                string _sqlQuery = @"
SELECT uz_Id AS [id], uz_Nazwisko + ' ' + uz_Imie AS [uzytkownik]
FROM pd_Uzytkownik
ORDER BY uzytkownik";

                SqlHelper.Instance.Initialize(SqlConnString);
                SqlHelper.Instance.ExecuteDataTable(_sqlQuery, ref _dtInsOperators, 0);
            }
            catch { }

            return _dtInsOperators;
        }
    }
}
