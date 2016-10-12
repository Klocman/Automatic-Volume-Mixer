using System;
using System.Drawing;
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
            Opacity = 0;

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

        private void ConfigurationManager_Shown(object sender, EventArgs e)
        {
            var screen = Screen.FromPoint(MousePosition).WorkingArea;
            Location = new Point(Math.Max(Math.Min(MousePosition.X - Width / 2, screen.X + screen.Width - Width), screen.X),
                Math.Max(Math.Min(MousePosition.Y - Height / 2, screen.Y + screen.Height - Height), screen.Y));
            
            Update();
            Opacity = 1;
        }
    }
}