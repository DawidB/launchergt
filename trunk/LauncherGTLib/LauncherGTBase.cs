using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using InsERT;
using DataEncryptionLib;
using System.Data.SqlClient;
using SqlHelperLib;
using System.IO;

namespace LauncherGTLib
{
    public class LauncherGTBase
    {
        public AesEncrypter AesEnc { get; set; }

        public SqlConnection SqlConn { get; set; }

        public bool CloseInsAppsOnExit { get; set; }

        private Navireo _navireo = null;
        public Navireo Navireo
        {
            get 
            {
                return _navireo;
            }
            set
            {
                DanePodmiotu _subjectData = null;

                DisposeNavireo();
                _navireo = value;

                if (_navireo != null)
                {
                    try
                    {
                        _subjectData = _navireo.DanePodmiotu;
                        WyczyscNrNip = _subjectData.NIP;
                    }
                    catch (Exception ex) { }
                    finally
                    {
                        if (_subjectData != null)
                        {
                            Marshal.ReleaseComObject(_subjectData);
                            _subjectData = null;
                        }
                    }
                }
            }
        }

        public Subiekt _subiekt = null;
        public Subiekt Subiekt
        {
            get
            {
                return _subiekt;
            }
            set
            {
                DanePodmiotu _subjectData = null;

                DisposeSubiektGT();
                _subiekt = value;

                if (_subiekt != null)
                {
                    try
                    {
                        _subjectData = _subiekt.DanePodmiotu;
                        WyczyscNrNip = _subjectData.NIP;
                    }
                    catch(Exception ex) { }
                    finally
                    {
                        if (_subjectData != null)
                        {
                            Marshal.ReleaseComObject(_subjectData);
                            _subjectData = null;
                        }
                    }
                }
            }
        }

        private Gestor _gestor = null;
        public Gestor Gestor
        {
            get
            {
                return _gestor;
            }
            set
            {
                DanePodmiotu _subjectData = null;

                DisposeGestorGT();
                _gestor = value;

                if (_gestor != null)
                {
                    try
                    {
                        _subjectData = _gestor.DanePodmiotu;
                        WyczyscNrNip = _subjectData.NIP;
                    }
                    catch (Exception ex) { }
                    finally
                    {
                        if (_subjectData != null)
                        {
                            Marshal.ReleaseComObject(_subjectData);
                            _subjectData = null;
                        }
                    }
                }
            }
        }

        private Rewizor _rewizor = null;
        public Rewizor Rewizor
        {
            get
            {
                return _rewizor;
            }
            set
            {
                DanePodmiotu _subjectData = null;

                DisposeRewizorGT();
                _rewizor = value;

                if (_rewizor != null)
                {
                    try
                    {
                        _subjectData = _rewizor.DanePodmiotu;
                        WyczyscNrNip = _subjectData.NIP;
                    }
                    catch (Exception ex) { }
                    finally
                    {
                        if (_subjectData != null)
                        {
                            Marshal.ReleaseComObject(_subjectData);
                            _subjectData = null;
                        }
                    }
                }
            }
        }

        private Gratyfikant _gratyfikant = null;
        public Gratyfikant Gratyfikant
        {
            get
            {
                return _gratyfikant;
            }
            set
            {
                DanePodmiotu _subjectData = null;

                DisposeGratyfikantGT();
                _gratyfikant = value;

                if (_gratyfikant != null)
                {
                    try
                    {
                        _subjectData = _gratyfikant.DanePodmiotu;
                        WyczyscNrNip = _subjectData.NIP;
                    }
                    catch (Exception ex) { }
                    finally
                    {
                        if (_subjectData != null)
                        {
                            Marshal.ReleaseComObject(_subjectData);
                            _subjectData = null;
                        }
                    }
                }
            }
        }

        public string ErrorMsg { get; set; }

        public bool IsSettingsFileShared = true;
        private string _settingsFileLocation;
        public string SettingsFileLocation
        {
            get
            {
                return _settingsFileLocation;
            }
        }
        public string ApplicationName
        {
            set
            {
                //set path variables
                string _commonPathEnding = "\\LauncherGT\\" + value.TrimStart('\\').TrimEnd('\\'),
                    _commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    _userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                //if application name wasn't defined, set default directory
                if (value == "")
                    _commonPathEnding = "\\LauncherGT\\";
                
                //checks whether defined directory exists
                if (Directory.Exists(_commonAppData + _commonPathEnding))
                    _settingsFileLocation = _commonAppData + _commonPathEnding + "\\LauncherSettings.xml";
                else if (Directory.Exists(_userAppData + _commonPathEnding))
                    _settingsFileLocation = _userAppData + _commonPathEnding + "\\LauncherSettings.xml";
                else
                {
                    string _appData;

                    //if directory doesn't exists, it is created
                    if (IsSettingsFileShared)
                        _appData = _commonAppData;
                    else                        
                        _appData = _userAppData;

                    if (!Directory.Exists(_appData + _commonPathEnding))
                        Directory.CreateDirectory(_appData + _commonPathEnding);

                    _settingsFileLocation = _appData + _commonPathEnding + "\\LauncherSettings.xml";
                }
            }
        }


        public UCSettings[] UCSub { get; set; }

        public SettingsPackage SettingsPackage { get; set; }

        private string _nipPodmiotu = "";
        public string NipPodmiotu
        {
            get
            {
                return _nipPodmiotu;
            }
        }
        private string WyczyscNrNip
        {
            set
            {
                _nipPodmiotu = "";

                foreach (char _c in value)
                {
                    if (_c >= 48 && _c <= 57)
                        _nipPodmiotu += _c;
                }
            }
        }



        public LauncherGTBase() 
        {
             AesEnc = new AesEncrypter("HrJpIPjRye8ycBsSYum1fJplEfb05/hz", "gWBVw8Ytz2wlhZuOIBuckw==");
             CloseInsAppsOnExit = false;
        }

        public bool StartNavireo(UserSettings _settings, bool _startWithIqa = false)
        {
            DisposeNavireo();

            try
            {
                if (_startWithIqa)
                {
                    Navireo = (Navireo)Marshal.BindToMoniker(_settings.NavFilePath);
                    return true;
                }
                else
                {
                    string _sqlServer = _settings.SqlServer;
                    
                    if (!string.IsNullOrEmpty(_settings.SqlPort))
                        _sqlServer += "," + _settings.SqlPort;

                    Navireo = new Navireo();
                    _navireo.Zaloguj(_sqlServer,
                        (AutentykacjaEnum)_settings.SqlAuthentication,
                        _settings.SqlLogin,
                        _settings.SqlPassword,
                        _settings.SqlDatabase,
                        "", /*AppRoleName*/
                        "", /*AppRolePass*/
                        AutentykacjaEnum.gtaAutentykacjaMieszana,
                        _settings.InsLoginId,
                        _settings.InsPassword);

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
        }

        public bool StartInsertGT(UserSettings _settings, ProduktEnum _insApp)
        {
            object _testInsApp = null;
            GT _insGt = new GT();
            Dodatki _insAddons = new Dodatki();

            try
            {
                string _sqlServer = _settings.SqlServer;

                if (!string.IsNullOrEmpty(_settings.SqlPort))
                    _sqlServer += "," + _settings.SqlPort;

                _insGt.Produkt = _insApp;
                _insGt.Serwer = _sqlServer;
                _insGt.Autentykacja = (AutentykacjaEnum)_settings.SqlAuthentication;
                _insGt.Uzytkownik = _settings.SqlLogin;
                _insGt.UzytkownikHaslo = _settings.SqlPassword;
                _insGt.Baza = _settings.SqlDatabase;
                _insGt.Operator = _settings.InsLoginName;
                _insGt.OperatorHaslo = _insAddons.Szyfruj(_settings.InsPassword);
                
                switch (_insApp)
                {
                    case ProduktEnum.gtaProduktGestor:
                        Gestor = (Gestor)_insGt.Uruchom(_settings.InsAppLoginMode, _settings.InsAppStartMode);
                        _testInsApp = Gestor.OperatorId;
                        break;
                    case ProduktEnum.gtaProduktGratyfikant:
                        Gratyfikant = (Gratyfikant)_insGt.Uruchom(_settings.InsAppLoginMode, _settings.InsAppStartMode);
                        _testInsApp = Gratyfikant.OperatorId;
                        break;
                    case ProduktEnum.gtaProduktRewizor:
                        Rewizor = (Rewizor)_insGt.Uruchom(_settings.InsAppLoginMode, _settings.InsAppStartMode);
                        _testInsApp = Rewizor.OperatorId;
                        break;
                    default:
                        Subiekt = (Subiekt)_insGt.Uruchom(_settings.InsAppLoginMode, _settings.InsAppStartMode);
                        _testInsApp = Subiekt.OperatorId;
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
            finally
            {
                if (_insGt != null)
                {
                    Marshal.ReleaseComObject(_insGt);
                    _insGt = null;
                }
                if (_insAddons != null)
                {
                    Marshal.ReleaseComObject(_insAddons);
                    _insAddons = null;
                }
            }
        }

        /// <summary>
        /// Initialize SQL Connection and declare SqlConn object.
        /// In case of failure, sets SqlConn to null.
        /// </summary>
        /// <param name="_sqlConnStr"></param>
        /// <returns></returns>
        public bool StartSqlConn(string _sqlConnStr)
        {
            if (SqlHelper.Instance.Initialize(_sqlConnStr))
            {
                SqlConn = SqlHelper.Instance.SqlConn;
                return true;
            }
            else
                return false;
        }

        public void DisposeNavireo()
        {
            try
            {
                if (_navireo != null)
                    _navireo.Zakoncz();
            }
            catch { }
            finally
            {
                if (_navireo != null)
                {
                    Marshal.ReleaseComObject(_navireo);
                    _navireo = null;
                }
            }
        }

        public void DisposeSubiektGT()
        {
            try
            {
                if (_subiekt != null)
                    _subiekt.Zakoncz();
            }
            catch(Exception ex) { }
            finally
            {
                if (_subiekt != null)
                {
                    Marshal.ReleaseComObject(_subiekt);
                    _subiekt = null;
                }
            }
        }

        public void DisposeGestorGT()
        {
            try
            {
                if (_gestor != null)
                    _gestor.Zakoncz();
            }
            catch { }
            finally
            {
                if (_gestor != null)
                {
                    Marshal.ReleaseComObject(_gestor);
                    _gestor = null;
                }
            }
        }

        public void DisposeRewizorGT()
        {
            try
            {
                if (_rewizor != null)
                    _rewizor.Zakoncz();
            }
            catch { }
            finally
            {
                if (_rewizor != null)
                {
                    Marshal.ReleaseComObject(_rewizor);
                    _rewizor = null;
                }
            }
        }

        public void DisposeGratyfikantGT()
        {
            try
            {
                if (_gratyfikant != null)
                    _gratyfikant.Zakoncz();
            }
            catch { }
            finally
            {
                if (_gratyfikant != null)
                {
                    Marshal.ReleaseComObject(_gratyfikant);
                    _gratyfikant = null;
                }
            }
        }
    }
}
