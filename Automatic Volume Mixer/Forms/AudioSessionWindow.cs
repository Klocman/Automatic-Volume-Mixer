using System;
using System.Windows.Forms;
using Avm.Daemon;
using Avm.Properties;
using Klocman.Extensions;

namespace Avm.Forms
{
    public partial class AudioSessionWindow : Form
    {
        private readonly AutomaticMixer _sourceMixer;
        private readonly EventHandler<StateUpdateEventArgs> _updateDelegate;

        public AudioSessionWindow(AutomaticMixer sourceMixer)
        {
            Opacity = 0;

            InitializeComponent();

            Icon = Resources.editoricon;

            _sourceMixer = sourceMixer;
            _updateDelegate = (_, args) => this.SafeInvoke(() => audioSessionViewer1.RefreshSessions(args));

            VisibleChanged += OnVisibleChanged;
            FormClosed += OnFormClosedEventHandler;
        }

        private void OnFormClosedEventHandler(object sender, FormClosedEventArgs args)
        {
            try
            {
                _sourceMixer.MixerStateUpdate -= _updateDelegate;
            }
            catch
            {
                /*Does this ever throw?*/
            }
        }

        private void OnVisibleChanged(object sender, EventArgs eventArgs)
        {
            if (Visible)
                _sourceMixer.MixerStateUpdate += _updateDelegate;
            else
                _sourceMixer.MixerStateUpdate -= _updateDelegate;
        }

        private void AudioSessionWindow_Shown(object sender, EventArgs e)
        {
            this.MoveCloseToCursor();
            Update();
            Opacity = 1;
        }
    }
}