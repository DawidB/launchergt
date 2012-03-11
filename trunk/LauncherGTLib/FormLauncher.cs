using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace LauncherGTLib
{
    public partial class FormLauncher : Form, IDisposable
    {
        private void SaveSettings()
        {
            try
            {
                FileStream _fileStream = new FileStream(LauncherGT.Instance.SettingsFile, FileMode.Create);
                XmlSerializer _serializer = new XmlSerializer(LauncherGT.Instance.SettingsPackage.GetType());
                XmlSerializerNamespaces _namespace = new XmlSerializerNamespaces();
                _namespace.Add("", "");

                _serializer.Serialize(_fileStream, LauncherGT.Instance.SettingsPackage, _namespace);
                _fileStream.Flush();
                _fileStream.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadSettings()
        {
            try
            {
                FileStream _fileStream = new FileStream(LauncherGT.Instance.SettingsFile, FileMode.Open);
                XmlSerializer _serializer = new XmlSerializer(LauncherGT.Instance.SettingsPackage.GetType(), "");

                SettingsPackage _loaded = (SettingsPackage)_serializer.Deserialize(_fileStream);
                if (_loaded.Settings.Length == LauncherGT.Instance.SettingsPackage.Settings.Length)
                    LauncherGT.Instance.SettingsPackage = _loaded;

                _fileStream.Flush();
                _fileStream.Close();
            }
            catch(Exception ex)
            {

            }
        }

        public DialogResult SilentStart()
        {
            for (int i = 0; i < LauncherGT.Instance.UCSub.Length; i++)
                if (!LauncherGT.Instance.UCSub[i].CheckSettings(false))
                    return DialogResult.Cancel;

            return DialogResult.OK;
        }
                


        public FormLauncher(bool _showMessages = false, UserSettingsMode _settingsMode = UserSettingsMode.Full)
        {
            InitializeComponent();

            this.Size = new Size(350, 490);
            TCMain.Visible = false;

            LauncherGT.Instance.SettingsPackage = new SettingsPackage();
            LauncherGT.Instance.SettingsPackage.Settings[0] = new UserSettings();
            LoadSettings();

            LauncherGT.Instance.UCSub = new UCSettings[1];
            LauncherGT.Instance.UCSub[0] = new UCSettings(_settingsMode, ref LauncherGT.Instance.SettingsPackage.Settings[0]);
            LauncherGT.Instance.UCSub[0].Parent = GBMain;
            LauncherGT.Instance.UCSub[0].Dock = DockStyle.Fill;
            LauncherGT.Instance.UCSub[0].ShowMessages = _showMessages;

            this.Height = LauncherGT.Instance.UCSub[0].TableHeight + 142;
            if (this.Height > 490)
                this.Height = 490;
        }

        public FormLauncher(SettingsDefinition[] _settingsDef, bool _showMessages = false)
        {
            InitializeComponent();

            TCMain.Visible = true;

            LauncherGT.Instance.SettingsPackage = new SettingsPackage(_settingsDef.Length);
            LauncherGT.Instance.UCSub = new UCSettings[_settingsDef.Length];

            LoadSettings();

            for (int i = 0; i < LauncherGT.Instance.UCSub.Length; i++)
            {
                TCMain.TabPages.Add(_settingsDef[i].TabKey, _settingsDef[i].TabText);

                if (LauncherGT.Instance.SettingsPackage.Settings[_settingsDef[i].Id] == null)
                    LauncherGT.Instance.SettingsPackage.Settings[_settingsDef[i].Id] = new UserSettings();

                LauncherGT.Instance.UCSub[i] = new UCSettings(_settingsDef[i].Mode, ref LauncherGT.Instance.SettingsPackage.Settings[_settingsDef[i].Id]);
                LauncherGT.Instance.UCSub[i].Parent = TCMain.TabPages[i];
                LauncherGT.Instance.UCSub[i].Dock = DockStyle.Fill;
                LauncherGT.Instance.UCSub[i].ShowMessages = _showMessages;

                if (this.Height < LauncherGT.Instance.UCSub[0].TableHeight + 172)
                {
                    this.Height = LauncherGT.Instance.UCSub[0].TableHeight + 172;
                    if (this.Height > 490)
                        this.Height = 490;
                }
            }
        }

        ~FormLauncher()
        {
            LauncherGT.Instance.DisposeNavireo();
            LauncherGT.Instance.DisposeSubiektGT();
            LauncherGT.Instance.DisposeGestorGT();
            LauncherGT.Instance.DisposeRewizorGT();
            LauncherGT.Instance.DisposeGratyfikantGT();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SaveSettings();

            for (int i = 0; i < LauncherGT.Instance.UCSub.Length; i++)
                if (!LauncherGT.Instance.UCSub[i].CheckSettings(false))
                {
                    this.DialogResult = MessageBox.Show("Uruchomienie aplikacji z podanymi danymi nie powiodło się. Czy chcesz poprawić dane?", "Pytanie", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
                    if (this.DialogResult == DialogResult.Retry)
                        this.DialogResult = DialogResult.None;

                    break;
                }
        }
    }
}
