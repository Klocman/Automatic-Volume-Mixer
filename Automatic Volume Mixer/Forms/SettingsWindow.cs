using System;
using System.Reflection;
using System.Windows.Forms;
using Avm.Properties;
using Klocman.Extensions;
using Klocman.Tools;

namespace Avm.Forms
{
    public partial class SettingsWindow : Form
    {
        private const string RunKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string RunKeyValueName = "AutomaticVolumeMixer";

        public SettingsWindow()
        {
            Opacity = 0;

            InitializeComponent();

            Icon = Resources.editoricon;

            var settings = Settings.Default.Binder;
            settings.BindControl(checkBoxRestoreSessionSettings, s => s.RestoreSessionSettingsOnExit, this);
            settings.SendUpdates(this);

            using (var key = RegistryTools.OpenRegistryKey(RunKeyPath))
            {
                checkBoxBoot.Checked = key?.GetValue(RunKeyValueName) != null;
            }
            checkBoxBoot.CheckedChanged += checkBoxBoot_CheckedChanged;
        }

        private void checkBoxBoot_CheckedChanged(object sender, EventArgs e)
        {
            using (var key = RegistryTools.CreateSubKeyRecursively(RunKeyPath))
            {
                if (checkBoxBoot.Checked)
                    key.SetValue(RunKeyValueName, $"\"{Assembly.GetExecutingAssembly().Location}\"");
                else
                    key.DeleteValue(RunKeyValueName);
            }
        }

        private void SettingsWindow_Shown(object sender, EventArgs e)
        {
            this.MoveCloseToCursor();
            Update();
            Opacity = 1;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}