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
                DanePodmiotu _subjectData = null;

                try
                {
                    if (_navireo != null)
                        _subjectData = _navireo.DanePodmiotu;

                    return _navireo;
                }
                catch
                {
                    return null;
                }
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

        public Subiekt _subiekt = null;
        public Subiekt Subiekt
        {
            get
            {
                DanePodmiotu _subjectData = null;

                try
                {
                    if (_subiekt != null)
                        _subjectData = _subiekt.DanePodmiotu;

                    return _subiekt;
                }
                catch
                {
                    return null;
                }
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

        private Gestor _gestor = null;
        public Gestor Gestor
        {
            get
            {
                DanePodmiotu _subjectData = null;

                try
                {
                    if (_gestor != null)
                        _subjectData = _gestor.DanePodmiotu;

                    return _gestor;
                }
                catch
                {
                    return null;
                }
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

        private Rewizor _rewizor = null;
        public Rewizor Rewizor
        {
            get
            {
                DanePodmiotu _subjectData = null;

                try
                {
                    if (_rewizor != null)
                        _subjectData = _rewizor.DanePodmiotu;

                    return _rewizor;
                }
                catch
                {
                    return null;
                }
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

        private Gratyfikant _gratyfikant = null;
        public Gratyfikant Gratyfikant
        {
            get
            {
                DanePodmiotu _subjectData = null;

                try
                {
                    if (_gratyfikant != null)
                        _subjectData = _gratyfikant.DanePodmiotu;

                    return _gratyfikant;
                }
                catch
                {
                    return null;
                }
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

        public string ErrorMsg { get; set; }
        public string SettingsFile = "LauncherSettings.xml";

        public UCSettings[] UCSub { get; set; }

        public SettingsPackage SettingsPackage { get; set; }



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
                    _navireo = (Navireo)Marshal.BindToMoniker(_settings.NavFilePath);
                    return true;
                }
                else
                {
                    string _sqlServer = _settings.SqlServer;
                    
                    if (!string.IsNullOrEmpty(_settings.SqlPort))
                        _sqlServer += "," + _settings.SqlPort;

                    _navireo = new Navireo();
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
                        DisposeGestorGT();
                        _gestor = (Gestor)_insGt.Uruchom(_settings.InsAppLoginMode, _settings.InsAppStartMode);
                        _testInsApp = _subiekt.MagazynId;
                        break;
                    case ProduktEnum.gtaProduktGratyfikant:
                        DisposeGratyfikantGT();
                        _gratyfikant = (Gratyfikant)_insGt.Uruchom(_settings.InsAppLoginMode, _settings.InsAppStartMode);
                        _testInsApp = _subiekt.MagazynId;
                        break;
                    case ProduktEnum.gtaProduktRewizor:
                        DisposeRewizorGT();
                        _rewizor = (Rewizor)_insGt.Uruchom(_settings.InsAppLoginMode, _settings.InsAppStartMode);
                        _testInsApp = _subiekt.MagazynId;
                        break;
                    default:
                        DisposeSubiektGT();
                        _subiekt = (Subiekt)_insGt.Uruchom(_settings.InsAppLoginMode, _settings.InsAppStartMode);
                        _testInsApp = _subiekt.MagazynId;
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
