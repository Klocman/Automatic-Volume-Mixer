using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Avm.Properties;
using Avm.Storage;
using Klocman.Extensions;

namespace Avm.Forms
{
    public partial class BehaviourEditor : Form
    {
        private static readonly TriggeringMode[] TriggeringKindResults =
        {
            TriggeringMode.Always,
            TriggeringMode.RisingEdge,
            TriggeringMode.FallingEdge,
            TriggeringMode.BothEdges,
            TriggeringMode.Timed
        };

        private readonly RadioButton[] _triggeringKindControls;

        private BehaviourEditor()
        {
            InitializeComponent();

            _triggeringKindControls = new[] {radioButton1, radioButton2, radioButton3, radioButton4, radioButton5};

            Icon = Resources.editoricon;
        }

        public Behaviour CurrentBehaviour { get; private set; }

        private void SetupEditor(Behaviour newBehaviour, IEnumerable<string> groups)
        {
            CurrentBehaviour = newBehaviour;

            var triggers = newBehaviour.Triggers;
            elementListTriggers.SetupList(triggers,
                (window, info) => ElementEditor.ShowDialog(window, (ITrigger) info),
                info => triggers.Add((ITrigger) info),
                info => triggers.Remove((ITrigger) info));

            var conditions = newBehaviour.Conditions;
            elementListConditions.SetupList(conditions,
                (window, info) => ElementEditor.ShowDialog(window, (ITrigger) info),
                info => conditions.Add((ITrigger) info),
                info => conditions.Remove((ITrigger) info));

            var actions = newBehaviour.Actions;
            elementListActions.SetupList(actions,
                (window, info) => ElementEditor.ShowDialog(window, (IAction) info),
                info => actions.Add((IAction) info),
                info => actions.Remove((IAction) info),
                info => actions.MoveUp(actions.IndexOf((IAction) info)),
                info => actions.MoveDown(actions.IndexOf((IAction) info)));

            textBoxName.DataBindings.Add(nameof(textBoxName.Text),
                CurrentBehaviour, nameof(CurrentBehaviour.Name),
                false, DataSourceUpdateMode.OnValidation);

            checkBoxEnabled.DataBindings.Add(nameof(checkBoxEnabled.Checked),
                CurrentBehaviour, nameof(CurrentBehaviour.Enabled),
                false, DataSourceUpdateMode.OnValidation);

            _triggeringKindControls[Array.IndexOf(TriggeringKindResults, newBehaviour.TriggeringKind)].Checked = true;

            comboBoxGroup.Text = newBehaviour.Group ?? string.Empty;
            comboBoxGroup.Items.AddRange(groups.Cast<object>().ToArray());
            numericUpDown1.Value = Math.Min(
                Math.Max((decimal) newBehaviour.MinimalTimedTriggerDelay, numericUpDown1.Minimum),
                numericUpDown1.Maximum);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public static Behaviour ShowDialog(IWin32Window owner, Behaviour targetBehaviour, IEnumerable<string> groups)
        {
            using (var window = new BehaviourEditor())
            {
                window.SetupEditor(targetBehaviour != null ? (Behaviour) targetBehaviour.Clone() : new Behaviour(),
                    groups);

                return window.ShowDialog(owner) == DialogResult.OK ? window.CurrentBehaviour : null;
            }
        }

        private void BehaviourEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            CurrentBehaviour.TriggeringKind = TriggeringKindResults[
                Array.IndexOf(_triggeringKindControls, _triggeringKindControls.First(x => x.Checked))];
            CurrentBehaviour.Group = comboBoxGroup.Text;
            CurrentBehaviour.MinimalTimedTriggerDelay = (double) numericUpDown1.Value;
        }
    }
}