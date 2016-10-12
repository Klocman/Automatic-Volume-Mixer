using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Avm.Daemon;
using Klocman.Extensions;

namespace Avm.Controls
{
    public partial class AudioSessionViewer : UserControl
    {
        public AudioSessionViewer()
        {
            InitializeComponent();
            listView1.SetDoubleBuffered(true);
        }

        public void RefreshSessions(StateUpdateEventArgs stateUpdateEventArgs)
        {
            var query = stateUpdateEventArgs.Sessions
                .OrderByDescending(session => session.IsSystemSoundSession)
                .ThenBy(session => session.DisplayName)
                .Select(session => new ListViewItem(new[]
                    {
                        session.DisplayName,
                        session.PeakValue.ToString(CultureInfo.CurrentCulture),
                        session.MasterVolume.ToString(CultureInfo.CurrentCulture),
                        session.IsMuted.ToString()
                    })
                { Tag = session });

            SuspendLayout();
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Items.AddRange(query.ToArray());
            listView1.EndUpdate();
            ResumeLayout();
        }
    }
}