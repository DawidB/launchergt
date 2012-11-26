using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SqlHelperLib;
using InsERT;

namespace LauncherGTLib
{
    public partial class UCSettings : UserControl
    {
        private bool _isSqlVisible = true,
            _isInsertVisible = true,
            _isInsertGtVisible = false,
            _isNavireoVisible = false,
            _logEvents = false;
        
        public int TableHeight { get; set; }

        public bool ShowMessages { get; set;}

        private UserSettings _settings;
        public UserSettings Settings { get { return _settings; } }

        private UserSettingsMode _mode;
        public UserSettingsMode Mode { get { return _mode; } }



        private void DefineDataSources()
        {
            try
            {
                DataTable _DTSqlAuthentication = new DataTable();
                _DTSqlAuthentication.Columns.Add("id", typeof(AutentykacjaEnum));
                _DTSqlAuthentication.Columns.Add("name", typeof(string));
                _DTSqlAuthentication.Rows.Add(AutentykacjaEnum.gtaAutentykacjaWindows, "Windows");
                _DTSqlAuthentication.Rows.Add(AutentykacjaEnum.gtaAutentykacjaMieszana, "Mieszana");
                CBSqlAuthentication.DataSource = _DTSqlAuthentication;
                CBSqlAuthentication.DisplayMember = "name";
                CBSqlAuthentication.ValueMember = "id";

                DataTable _DTInsAppType = new DataTable();
                _DTInsAppType.Columns.Add("id", typeof(int));
                _DTInsAppType.Columns.Add("name", typeof(string));
                _DTInsAppType.Rows.Add(0, "Navireo");
                _DTInsAppType.Rows.Add(1, "Subiekt GT");
                _DTInsAppType.Rows.Add(2, "Gestor GT");
                _DTInsAppType.Rows.Add(3, "Rewizor GT");
                _DTInsAppType.Rows.Add(4, "Gratyfikant GT");
                CBInsAppType.DataSource = _DTInsAppType;
                CBInsAppType.DisplayMember = "name";
                CBInsAppType.ValueMember = "id";

                DataTable _DTNavStartMode = _DTInsAppType.Clone();
                _DTNavStartMode.Rows.Add(0, "Dane logowania");
                _DTNavStartMode.Rows.Add(1, "Plik startowy IQA");
                CBNavStartMode.DataSource = _DTNavStartMode;
                CBNavStartMode.DisplayMember = "name";
                CBNavStartMode.ValueMember = "id";

                DataTable _DTInsAppLoginMode = new DataTable();
                _DTInsAppLoginMode.Columns.Add("id", typeof(UruchomDopasujEnum));
                _DTInsAppLoginMode.Columns.Add("name", typeof(string));
                _DTInsAppLoginMode.Rows.Add(UruchomDopasujEnum.gtaUruchomDopasuj, "Dopasuj");
                _DTInsAppLoginMode.Rows.Add(UruchomDopasujEnum.gtaUruchomDopasujOperatora, "Dopasuj użytkownika");
                _DTInsAppLoginMode.Rows.Add(UruchomDopasujEnum.gtaUruchomDopasujUzytkownika, "Dopasuj operatora");
                CBInsAppLoginMode.DataSource = _DTInsAppLoginMode;
                CBInsAppLoginMode.DisplayMember = "name";
                CBInsAppLoginMode.ValueMember = "id";

                DataTable _DTInsAppStartMode = new DataTable();
                _DTInsAppStartMode.Columns.Add("id", typeof(UruchomEnum));
                _DTInsAppStartMode.Columns.Add("name", typeof(string));
                _DTInsAppStartMode.Rows.Add(UruchomEnum.gtaUruchom, "Uruchom");
                _DTInsAppStartMode.Rows.Add(UruchomEnum.gtaUruchomNieArchiwizujPrzyZamykaniu, "Uruchom (bez archiwizacji)");
                _DTInsAppStartMode.Rows.Add(UruchomEnum.gtaUruchomNieZablokowany, "Uruchom niezablokowany");
                _DTInsAppStartMode.Rows.Add(UruchomEnum.gtaUruchomNowy, "Uruchom nowy");
                _DTInsAppStartMode.Rows.Add(UruchomEnum.gtaUruchomWTle, "Uruchom w tle");
                CBInsAppStartMode.DataSource = _DTInsAppStartMode;
                CBInsAppStartMode.DisplayMember = "name";
                CBInsAppStartMode.ValueMember = "id";                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd w metodzie UCSubject.DefineDataSources(). Szczegóły: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindVariablesToControls()
        {
            try
            {
                TBSqlServer.DataBindings.Add("Text", _settings, "SqlServer");
                TBSqlPort.DataBindings.Add("Text", _settings, "SqlPort");
                CBSqlAuthentication.DataBindings.Add("SelectedValue", _settings, "SqlAuthentication", true, DataSourceUpdateMode.OnPropertyChanged);
                TBSqlLogin.DataBindings.Add("Text", _settings, "SqlLogin");
                ChBSqlPassword.DataBindings.Add("Checked", _settings, "RememberSqlPass");
                TBSqlPass.DataBindings.Add("Text", _settings, "SqlPassword");
                CBSqlDatabase.DataBindings.Add("Text", _settings, "SqlDatabase");
                CBInsAppType.DataBindings.Add("SelectedValue", _settings, "InsAppType", true, DataSourceUpdateMode.OnPropertyChanged);
                CBNavStartMode.DataBindings.Add("SelectedValue", _settings, "NavStartMode", true, DataSourceUpdateMode.OnPropertyChanged);
                TBNavFilePath.DataBindings.Add("Text", _settings, "NavFilePath");
                CBInsAppLoginMode.DataBindings.Add("SelectedValue", _settings, "InsAppLoginMode");
                CBInsAppStartMode.DataBindings.Add("SelectedValue", _settings, "InsAppStartMode");
                CBInsLogin.DataBindings.Add("SelectedValue", _settings, "InsLoginId");
                ChBInsPassword.DataBindings.Add("Checked", _settings, "RememberInsPass");
                TBInsPassword.DataBindings.Add("Text", _settings, "InsPassword");
            }
            catch (Exception ex)
            {
                if (ShowMessages)
                    MessageBox.Show("Wystapił błąd podczas wiązania zmiennych i kontrolek.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SetControlsVisiblity()
        {
            //check selected mode and set variables accordingly
            switch (_mode)
            {
                case UserSettingsMode.Full:                    
                    _isInsertGtVisible = true;
                    _isNavireoVisible = true;
                    break;
                case UserSettingsMode.SQL:
                    _isInsertVisible = false;
                    break;
                case UserSettingsMode.InsNavireo:
                    _isNavireoVisible = true;
                    _settings.InsAppType = 0;
                    break;
                case UserSettingsMode.InsNavireoIQA:
                    _isInsertVisible = false;
                    _isNavireoVisible = true;
                    _isSqlVisible = false;
                    _settings.InsAppType = 0;
                    _settings.NavStartMode = 1;
                    break;
                default:
                    _isInsertGtVisible = true;
                    if (_mode == UserSettingsMode.InsSubiektGT)
                        _settings.InsAppType = 1;
                    else if (_mode == UserSettingsMode.InsGestorGT)
                        _settings.InsAppType = 2;
                    else if (_mode == UserSettingsMode.InsGratyfikantGT)
                        _settings.InsAppType = 3;
                    else if (_mode == UserSettingsMode.InsRewizorGT)
                        _settings.InsAppType = 4;
                    break;
            }

            //change controls visibility according to selected mode:
            //-SQL controls
            LabelSqlServer.Visible = _isSqlVisible;
            TBSqlServer.Visible = _isSqlVisible;
            LabelSqlPort.Visible = _isSqlVisible;
            TBSqlPort.Visible = _isSqlVisible;
            LabelSqlAuthentication.Visible = _isSqlVisible;
            CBSqlAuthentication.Visible = _isSqlVisible;
            LabelSqlLogin.Visible = _isSqlVisible;
            TBSqlLogin.Visible = _isSqlVisible;
            ChBSqlPassword.Visible = _isSqlVisible;
            TBSqlPass.Visible = _isSqlVisible;
            LabelSqlDatabase.Visible = _isSqlVisible;
            CBSqlDatabase.Visible = _isSqlVisible;

            //-InsERT controls
            LabelInsLogin.Visible = _isInsertVisible;
            CBInsLogin.Visible = _isInsertVisible;
            ChBInsPassword.Visible = _isInsertVisible;
            TBInsPassword.Visible = _isInsertVisible;
            LabelInsAppType.Visible = _isInsertVisible;
            CBInsAppType.Visible = _isInsertVisible;
            LabelNavStartMode.Visible = _isInsertVisible;
            CBNavStartMode.Visible = _isInsertVisible;

            //-GT controls
            LLInsAppLoginMode.Visible = _isInsertGtVisible;
            CBInsAppLoginMode.Visible = _isInsertGtVisible;
            LLInsAppStartMode.Visible = _isInsertGtVisible;
            CBInsAppStartMode.Visible = _isInsertGtVisible;
            if (_isInsertGtVisible && !_isNavireoVisible)
            {
                LabelInsAppType.Visible = false;
                CBInsAppType.Visible = false;
                LabelNavStartMode.Visible = false;
                CBNavStartMode.Visible = false;
            }

            //-Navireo controls
            if (!_isInsertGtVisible && _isNavireoVisible)
            {
                LabelInsAppType.Visible = false;
                CBInsAppType.Visible = false;
            }
            LabelNavFilePath.Visible = _isNavireoVisible;
            TLPNavFilePath.Visible = _isNavireoVisible;

            int _heightDelta = 32;
            TableHeight = 0;
            if (LabelSqlServer.Visible)
                TableHeight += _heightDelta;
            if (LabelSqlPort.Visible)
                TableHeight += _heightDelta;
            if (LabelSqlLogin.Visible)
                TableHeight += _heightDelta;
            if (ChBSqlPassword.Visible)
                TableHeight += _heightDelta;
            if (LabelSqlDatabase.Visible)
                TableHeight += _heightDelta;
            if (LabelInsAppType.Visible)
                TableHeight += _heightDelta;
            if (LabelNavStartMode.Visible)
                TableHeight += _heightDelta;
            if (LabelNavFilePath.Visible)
                TableHeight += _heightDelta;
            if (LabelInsLogin.Visible)
                TableHeight += _heightDelta;
            if (ChBInsPassword.Visible)
                TableHeight += _heightDelta;
            if (LLInsAppLoginMode.Visible)
                TableHeight += _heightDelta;
            if (LLInsAppStartMode.Visible)
                TableHeight += _heightDelta;

            ManageControlState();
        }

        public void ManageControlState()
        {
            TBSqlServer.Enabled = true;
            TBSqlLogin.Enabled = (_settings.SqlAuthentication == 0) ? true : false;
            TBSqlPass.Enabled = TBSqlLogin.Enabled;

            CBNavStartMode.Enabled = (_settings.InsAppType == 0) ? true : false;
            TLPNavFilePath.Enabled = (_settings.InsAppType == 0 && _settings.NavStartMode == 1) ? true : false;

            CBInsAppLoginMode.Enabled = !CBNavStartMode.Enabled;
            CBInsAppStartMode.Enabled = !CBNavStartMode.Enabled;
            CBInsLogin.Enabled = !TLPNavFilePath.Enabled;
            TBInsPassword.Enabled = !TLPNavFilePath.Enabled;
            TBSqlServer.Enabled = !TLPNavFilePath.Enabled;

            if (TLPNavFilePath.Enabled)
            {
                TBSqlLogin.Enabled = false;
                TBSqlPass.Enabled = false;
            }

            TBSqlPort.Enabled = TBSqlServer.Enabled;
            CBSqlAuthentication.Enabled = TBSqlServer.Enabled;
            CBSqlDatabase.Enabled = TBSqlServer.Enabled;

            if (!Settings.EnabledSqlServer)
                TBSqlServer.Enabled = false;
            if (!Settings.EnabledSqlPort)
                TBSqlPort.Enabled = false;
            if (!Settings.EnabledSqlLogin)
                TBSqlLogin.Enabled = false;
            if (!Settings.EnabledSqlPassword)
                TBSqlPass.Enabled = false;
            if (!Settings.EnabledSqlDatabase)
                CBSqlDatabase.Enabled = false;
            if (!Settings.EnabledSqlAuth)
                CBSqlAuthentication.Enabled = false;
            if (!Settings.EnabledNavStartMode)
                CBNavStartMode.Enabled = false;
            if (!Settings.EnabledNavFilePathMode)
                TBNavFilePath.Enabled = false;
            if (!Settings.EnabledInsAppLoginMode)
                CBInsAppLoginMode.Enabled = false;
            if (!Settings.EnabledInsAppStartMode)
                CBInsAppStartMode.Enabled = false;
            if (!Settings.EnabledInsAppType)
                CBInsAppType.Enabled = false;
            if (!Settings.EnabledInsLoginName)
                CBInsLogin.Enabled = false;
            if (!Settings.EnabledInsPassword)
                TBInsPassword.Enabled = false;
            if (!Settings.EnabledRememberInsPass)
                ChBInsPassword.Enabled = false;
            if (!Settings.EnabledRememberSqlPass)
                ChBSqlPassword.Enabled = false;
        }

        public bool CheckSettings(bool _overrideShowMsgs)
        {
            bool _checkValid = true;
            string _resultMsg = "",
                _appName = "Navireo";
            MessageBoxIcon _resultIcon = MessageBoxIcon.Information;

            try
            {
                if (_isSqlVisible)
                {
                    _checkValid = LauncherGT.Instance.StartSqlConn(_settings.SqlConnString);

                    if (_checkValid)
                        _resultMsg = "Pomyślnie nawiązano połączenie SQL.";
                    else
                    {
                        _resultMsg = "Nie udało się zainicjować połączenia SQL.\nKomunikat błędu:\n" + SqlHelper.Instance.ErrorMsg;
                        _resultIcon = MessageBoxIcon.Error;
                    }
                }

                if (_checkValid && (_isInsertVisible || _isInsertGtVisible || _isNavireoVisible))
                {
                    _resultMsg += "\n\n";

                    switch (_settings.InsAppType)
                    {
                        case 0:
                            if (_settings.NavStartMode == 0)
                                _checkValid = LauncherGT.Instance.StartNavireo(_settings, false);
                            else
                                _checkValid = LauncherGT.Instance.StartNavireo(_settings, true);
                            break;
                        case 2:
                            _checkValid = LauncherGT.Instance.StartInsertGT(_settings, ProduktEnum.gtaProduktGestor);
                            _appName = "Gestor GT";
                            break;
                        case 3:
                            _checkValid = LauncherGT.Instance.StartInsertGT(_settings, ProduktEnum.gtaProduktRewizor);
                            _appName = "Rewizor GT";
                            break;
                        case 4:
                            _checkValid = LauncherGT.Instance.StartInsertGT(_settings, ProduktEnum.gtaProduktGratyfikant);
                            _appName = "Gratyfikant GT";
                            break;
                        default:
                            _checkValid = LauncherGT.Instance.StartInsertGT(_settings, ProduktEnum.gtaProduktSubiekt);
                            _appName = "Subiekt GT";
                            break;
                    }
                    
                    if (_checkValid)
                        _resultMsg += "Pomyślnie uruchomiono aplikację " + _appName + ".";
                    else
                    {
                        _resultMsg += "Nie udało się uruchomić aplikacji " + _appName + ".\nKomunikat błędu:\n" + LauncherGT.Instance.ErrorMsg;
                        if (_resultIcon == MessageBoxIcon.Information)
                            _resultIcon = MessageBoxIcon.Exclamation;
                    }
                }

                if (ShowMessages && _overrideShowMsgs)
                    MessageBox.Show(_resultMsg, "Informacja", MessageBoxButtons.OK, _resultIcon);

                return _checkValid;
            }
            catch (Exception ex)
            {
                if (ShowMessages && _overrideShowMsgs)
                    MessageBox.Show("Wystąpił problem podczas weryfikacji wprowadzonych ustawień.\nKomunikat błędu:\n" + ex.Message, "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        

        public UCSettings()
        {
            InitializeComponent();

            _settings = new UserSettings();
            _mode = UserSettingsMode.Full;

            DefineDataSources();
            BindVariablesToControls();
            SetControlsVisiblity();
        }

        public UCSettings(UserSettingsMode _mode, ref UserSettings _settings)
        {
            InitializeComponent();

            this._settings = _settings;
            this._mode = _mode;

            DefineDataSources();
            BindVariablesToControls();
            SetControlsVisiblity();
            
            try
            {
                //preloading user list using SQL connection from config file
                CBInsLogin.DataSource = this._settings.LoadInsOperators();
                CBInsLogin.SelectedValue = this._settings.InsLoginId;
            }
            catch { }
        }

        private void ManageControlState(object sender, EventArgs e)
        {
            ManageControlState();
        }

        private void LLCheckData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CheckSettings(true);
        }

        private void LLInsAppLoginMode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string _infoMsg = @"""Dopasuj"" oznacza dopasowanie pierwszej znalezionej aplikacji zadanego typu, która jest podłączona do wskazanego serwera i podanej bazy danych.

""Dopasuj użytkownika"" oznacza dopasowanie pierwszej znalezionej aplikacji zadanego typu, która jest podłączona do wskazanego serwera z wykorzystaniem podanego użytkownika SQL Servera i podanej bazy danych.

""Dopasuj operatora"" oznacza dopasowanie pierwszej znalezionej aplikacji zadanego typu, która jest podłączona do wskazanego serwera, bazy danych oraz zalogowana na podanego użytkownika InsERT GT (operatora).";

            MessageBox.Show(_infoMsg, "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LLInsAppStartMode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string _infoMsg = @"""Uruchom"" oznacza uruchomienie zadanej aplikacji o ile nie zostanie znaleziona działająca, do której można się podłączyć.

""Uruchom (bez archiwizacji)"" oznacza uruchomienie zadanej aplikacji o ile nie zostanie znaleziona działająca, do której można się podłączyć. Po zakończeniu pracy komunikat o konieczności archiwizacji nie zostanie wyświetlony.

""Uruchom niezablokowany"" oznacza uruchomienie zadanej aplikacji o ile nie zostanie znaleziona działająca  aplikacja danego typu w stanie nie zablokowanym (np. przez wyświetlanie interfejsu użytkownika), do której można się podłączyć.

""Uruchom nowy"" oznacza zawsze uruchomienie nowej instancji aplikacji.

""Uruchom w tle"" oznacza uruchomienie/podłączenie do zadanej aplikacji bez wykorzystywania interfejsu użytkownika. Aplikacja podłączona w ten sposób działa w tle - nie otwiera się jej okno.";

            MessageBox.Show(_infoMsg, "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnNavFilePath_Click(object sender, EventArgs e)
        {
            if (OFDNavFilePath.ShowDialog() == DialogResult.OK)
            {
                TBNavFilePath.Focus();
                TBNavFilePath.Text = OFDNavFilePath.FileName;
                BtnNavFilePath.Focus();
            }
        }

        private void CBSqlDatabase_DropDown(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.AppStarting;
                CBSqlDatabase.DataSource = _settings.LoadDatabases();
                this.Cursor = Cursors.Arrow;
            }
            catch(Exception ex) { }
        }

        private void CBInsLogin_DropDown(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.AppStarting;
                CBInsLogin.DataSource = _settings.LoadInsOperators();
                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex) { }
        }
    }
}
