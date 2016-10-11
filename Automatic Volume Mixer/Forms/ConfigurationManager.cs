using System;
using System.Windows.Forms;
using Avm.Properties;
using Avm.Storage;
using Klocman.Extensions;
using Klocman.Forms.Tools;

namespace Avm.Forms
{
    public partial class ConfigurationManager : Form
    {
        private readonly AutomaticMixer _mixer;

        public ConfigurationManager(AutomaticMixer mixer)
        {
            _mixer = mixer;
            InitializeComponent();

            Icon = Resources.editoricon;

            toolStrip1.Renderer = new ToolStripProfessionalRenderer(new StandardSystemColorTable());

            elementList1.SetupList(_mixer.Behaviours,
                (window, info) =>
                    BehaviourEditor.ShowDialog(window, (Behaviour) info, _mixer.GroupNamesEnumerable),
                info => _mixer.AddBehaviour((Behaviour) info),
                info => _mixer.RemoveBehaviour((Behaviour) info),
                null, null,
                info => ((Behaviour) info).Group);

            _mixer.BehavioursChanged += OnBehavioursChanged;
        }

        private void OnBehavioursChanged(object sender, EventArgs args)
        {
            this.SafeInvoke(() =>
            {
                if (!IsDisposed)
                    elementList1.ReloadList(_mixer.Behaviours);
            });
        }

        public event EventHandler ExportClick
        {
            add { toolStripButtonExp.Click += value; }
            remove { toolStripButtonExp.Click -= value; }
        }

        public event EventHandler ImportClick
        {
            add { toolStripButtonImport.Click += value; }
            remove { toolStripButtonImport.Click -= value; }
        }

        public event EventHandler ViewSessionsClick
        {
            add { toolStripButtonView.Click += value; }
            remove { toolStripButtonView.Click -= value; }
        }

        public event EventHandler SettingsClick
        {
            add { toolStripButtonSett.Click += value; }
            remove { toolStripButtonSett.Click -= value; }
        }

        private void ConfigurationManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            _mixer.BehavioursChanged -= OnBehavioursChanged;
        }
    }
}