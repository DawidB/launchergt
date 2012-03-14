using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SqlHelperGTLib;
using InsERT;
using LauncherGTLib;

namespace TestApp
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SettingsDefinition[] _settingsDef = new SettingsDefinition[2];
            _settingsDef[0] = new SettingsDefinition();
            _settingsDef[0].Mode = UserSettingsMode.SQL;
            _settingsDef[0].Id = 0;
            _settingsDef[0].TabKey = "tabSql";
            _settingsDef[0].TabText = "Połączenie SQL";
            _settingsDef[1] = new SettingsDefinition();
            _settingsDef[1].Mode = UserSettingsMode.InsNavireoIQA;
            _settingsDef[1].Id = 1;
            _settingsDef[1].TabKey = "tabNavireoIqa";
            _settingsDef[1].TabText = "Ustawienia Navireo";

            //LauncherGT.Instance.AesEnc = new DataEncryptionLib.AesEncrypter("PaleL6v8TaUeN4JjFGJLHhyDVYzfBnQJ", "qRQTI/8t3R7JFTc1CYs52A==");

            //FormLauncher _formLauncher = new FormLauncher(_settingsDef, true);
            FormLauncher _formLauncher = new FormLauncher(true, UserSettingsMode.InsSubiektGT);
            //_formLauncher.Size = new Size(350, 400);
            //DialogResult dr = _formLauncher.SilentStart();
            DialogResult dr = _formLauncher.ShowDialog();
            this.Close();
        }
    }
}
