using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    public class MuteAction : NameFilterBase, IAction
    {
        [Category("Mute")]
        [Description("New state of the sessions.")]
        [DefaultValue(MuteStates.Muted)]
        public MuteStates NewState { get; set; }

        public override string GetDetails()
        {
            return $@"{NewState} sessions; {base.GetDetails()}";
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            Debug.Assert(Enabled, "Enabled");

            foreach (var session in args.Sessions.Where(MatchSessionName))
            {
                switch (NewState)
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

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}