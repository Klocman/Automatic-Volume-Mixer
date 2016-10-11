using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avm.Properties;

namespace Avm.Forms
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();

            Icon = Resources.editoricon;

            var settings = Settings.Default;

            checkBoxRestoreSessionSettings.DataBindings.Add(nameof(checkBoxRestoreSessionSettings.Checked),
                settings, nameof(settings.RestoreSessionSettingsOnExit), false, DataSourceUpdateMode.OnPropertyChanged);

        }
    }
}
