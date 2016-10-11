using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    //TODO more options
    [DefaultProperty(nameof(Message))]
    public class MessageBoxAction : BasicInfoBase, IAction
    {
        [Category("Message box")]
        [Description("Message to display.")]
        public string Message { get; set; } = "Hello world!";

        [Category("Message box")]
        [Description("Should further execution be blocked?")]
        [DefaultValue(false)]
        public bool BlockExecution { get; set; }

        public override string GetDetails()
        {
            return "Show following message: " + Message;
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            Action msg = () => MessageBox.Show(Message, Assembly.GetExecutingAssembly().GetName().Name,
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (BlockExecution)
                msg();
            else
                Task.Factory.StartNew(msg);
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}