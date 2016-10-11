using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Avm.Properties;
using Avm.Storage;
using Klocman.Forms.Tools;

namespace Avm.Forms
{
    public partial class ElementEditor : Form
    {
        private IBasicInfo _currentItem;

        static ElementEditor()
        {
            var trigType = typeof (ITrigger);
            var actType = typeof (IAction);
            var trigTypes = new List<Type>();
            var actTypes = new List<Type>();

            foreach (var t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract))
            {
                if (trigType.IsAssignableFrom(t))
                    trigTypes.Add(t);
                else if (actType.IsAssignableFrom(t))
                    actTypes.Add(t);
            }

            TriggerTypes = trigTypes.OrderBy(x => x.Name).ToArray();
            ActionTypes = actTypes.OrderBy(x => x.Name).ToArray();
        }

        private ElementEditor()
        {
            InitializeComponent();

            Icon = Resources.editoricon;
        }

        private static Type[] TriggerTypes { get; }
        private static Type[] ActionTypes { get; }
        private ComboBoxWrapper<Type>[] ValidTypes { get; set; }

        private IBasicInfo CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (ReferenceEquals(_currentItem, value))
                    return;

                _currentItem = value;
                propertyGrid1.SelectedObject = _currentItem;
            }
        }

        public static IAction ShowDialog(IWin32Window owner, IAction targetAction)
        {
            return (IAction) ShowDialog(owner, targetAction, ActionTypes);
        }

        public static ITrigger ShowDialog(IWin32Window owner, ITrigger targetTrigger)
        {
            return (ITrigger) ShowDialog(owner, targetTrigger, TriggerTypes);
        }

        public static IBasicInfo ShowDialog(IWin32Window owner, IBasicInfo targetItem, Type targetType)
        {
            if (targetType.IsAssignableFrom(typeof (ITrigger)))
                return ShowDialog(owner, targetItem, TriggerTypes);
            if (targetType.IsAssignableFrom(typeof (IAction)))
                return ShowDialog(owner, targetItem, ActionTypes);

            throw new ArgumentException("Type is not supported", nameof(targetItem));
        }

        private static IBasicInfo ShowDialog(IWin32Window owner, IBasicInfo targetItem, Type[] validTypes)
        {
            using (var window = new ElementEditor())
            {
                if (targetItem != null)
                    window.CurrentItem = (IBasicInfo) targetItem.Clone();
                else
                {
                    window.CurrentItem = (IBasicInfo) Activator.CreateInstance(validTypes.First());
                    window.CurrentItem.Enabled = true;
                }

                window.ValidTypes = validTypes.Select(t => new ComboBoxWrapper<Type>(t,
                    type => BasicInfoBase.TrimName(type.Name))).OrderBy(w => w.ToString()).ToArray();

                window.comboBox1.Items.AddRange(window.ValidTypes.Cast<object>().ToArray());
                window.comboBox1.SelectedItem = window.comboBox1.Items.Cast<ComboBoxWrapper<Type>>()
                    .First(x => x.WrappedObject == window.CurrentItem?.GetType());

                if (window.ShowDialog(owner) == DialogResult.OK)
                    return window.CurrentItem;
            }

            return null;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newType = (comboBox1.SelectedItem as ComboBoxWrapper<Type>)?.WrappedObject;
            if (newType == null || CurrentItem.GetType() == newType)
                return;

            //TODO perserve IBasicInfo properties
            CurrentItem = (IBasicInfo) Activator.CreateInstance(newType);
            CurrentItem.Enabled = true;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}