using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Avm.Daemon;
using Avm.Properties;
using Klocman.Extensions;

namespace Avm.Forms
{
    public partial class VariableViewWindow : Form
    {
        private readonly AutomaticMixer _sourceMixer;

        public VariableViewWindow(AutomaticMixer sourceMixer)
        {
            Opacity = 0;

            InitializeComponent();

            listView1.SetDoubleBuffered(true);

            Icon = Resources.editoricon;

            _sourceMixer = sourceMixer;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            try
            {
                _sourceMixer.MixerStateUpdate -= OnMixerStateUpdate;
            }
            catch
            {
                /*Does this ever throw?*/
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                _sourceMixer.MixerStateUpdate += OnMixerStateUpdate;
            else
                _sourceMixer.MixerStateUpdate -= OnMixerStateUpdate;
        }

        private void OnMixerStateUpdate(object sender, StateUpdateEventArgs stateUpdateEventArgs)
        {
            this.SafeInvoke(() => RefreshList(stateUpdateEventArgs));
        }

        private void RefreshList(StateUpdateEventArgs stateUpdateEventArgs)
        {
            var query = stateUpdateEventArgs.VariableStore
                .OrderBy(x => x.Key)
                .Select(variable => new ListViewItem(new[]
                {
                    variable.Key,
                    variable.Value.ToString(CultureInfo.CurrentCulture)
                })
                { Tag = variable });

            SuspendLayout();
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Items.AddRange(query.ToArray());
            listView1.EndUpdate();
            ResumeLayout();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.MoveCloseToCursor();
            Update();
            Opacity = 1;
        }
    }
}
