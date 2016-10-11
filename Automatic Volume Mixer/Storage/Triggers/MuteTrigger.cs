using System.ComponentModel;
using System.Linq;
using Avm.Daemon;

namespace Avm.Storage.Triggers
{
    public class MuteTrigger : NameFilterBase, ITrigger
    {
        [Category("Mute")]
        [Description("State of the session.")]
        [DefaultValue(MuteStates.Muted)]
        public MuteStates MuteState { get; set; }

        public override string GetDetails()
        {
            return $@"Session is {MuteState}; {base.GetDetails()}";
        }

        public bool ProcessTrigger(object sender, StateUpdateEventArgs args)
        {
            if (!Enabled) return false;

            return args.Sessions.Where(x => MatchSessionName(x.Value))
                .Any(session => session.Value.IsMuted == (MuteState == MuteStates.Muted));
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            if (!Enabled) return;

            foreach (var session in args.Sessions.Where(x => MatchSessionName(x.Value))
                .Select(x => x.Value))
            {
                switch (MuteState)
                {
                    case MuteStates.Muted:
                        session.IsMuted = true;
                        break;
                    case MuteStates.Unmuted:
                        session.IsMuted = false;
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }
        }
    }
}