using System;
using System.ComponentModel;
using System.Diagnostics;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    public class StartProcessAction : BasicInfoBase, IAction
    {
        private int _millisecondsToWait;

        [Category("Start process")]
        [Description("Arguments passed to the executable.")]
        public string Arguments { get; set; } = string.Empty;

        [Category("Start process")]
        [Description("Full or relative path to the executable.")]
        public string FileName { get; set; } = string.Empty;

        [Category("Start process")]
        [Description("If true further execution is blocked until the process exits or the timeout passes.")]
        [DefaultValue(false)]
        public bool WaitForExit { get; set; } = false;

        [Category("Start process")]
        [Description("How many milliseconds to wait for the process to exit. 0 represents infinite wait time.")]
        [DefaultValue(0)]
        public int MillisecondsToWait
        {
            get { return _millisecondsToWait; }
            set { _millisecondsToWait = Math.Max(value, 0); }
        }

        public override string GetDetails()
        {
            return $"Start \"{FileName}\" with arguments \"{Arguments}\".";
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            if (!Enabled) return;

            var p = Process.Start(new ProcessStartInfo(FileName, Arguments) {UseShellExecute = true});

            if (WaitForExit)
                p?.WaitForExit(MillisecondsToWait > 0 ? MillisecondsToWait : int.MaxValue);
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}