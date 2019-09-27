using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Avm.Storage;

namespace Avm.Controls
{
    public partial class ElementList : UserControl
    {
        private Action<IBasicInfo> _addItem;
        private Action<IBasicInfo> _downItem;
        private Func<IBasicInfo, string> _groupKeyGetter;
        private IEnumerable<IBasicInfo> _itemListEnumerator;
        private Func<IWin32Window, IBasicInfo, IBasicInfo> _launchEditor;
        private Action<IBasicInfo> _removeItem;
        private Action<IBasicInfo> _upItem;
        private Action<int, IBasicInfo> _insertItem;
        private Action _clearItems;

        public ElementList()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return label1.Text; }
            set { label1.Text = base.Text = value; }
        }

        private bool ManualOrdering
        {
            get { return buttonActionUp.Enabled; }
            set
            {
                buttonActionUp.Visible = value;
                buttonActionUp.Enabled = value;
                buttonActionDown.Visible = value;
                buttonActionDown.Enabled = value;
            }
        }

        public void SetupList(IEnumerable<IBasicInfo> itemListEnumerator,
            Func<IWin32Window, IBasicInfo, IBasicInfo> launchEditor,
            Action<IBasicInfo> addToList,
            Action<IBasicInfo> removeFromList,
            Action<int, IBasicInfo> insertToList = null,
            Action clearItems = null,
            Action<IBasicInfo> upItem = null,
            Action<IBasicInfo> downItem = null,
            Func<IBasicInfo, string> groupKeyGetter = null)
        {
            if (addToList == null)
                throw new ArgumentNullException(nameof(addToList));
            if (removeFromList == null)
                throw new ArgumentNullException(nameof(removeFromList));
            if (launchEditor == null)
                throw new ArgumentNullException(nameof(launchEditor));

            _itemListEnumerator = itemListEnumerator;
            _addItem = addToList;
            _removeItem = removeFromList;
			_insertItem = insertToList;						   
            _clearItems = clearItems;
            _upItem = upItem;
            _downItem = downItem;
            _launchEditor = launchEditor;
            _groupKeyGetter = groupKeyGetter;

            ManualOrdering = upItem != null && downItem != null;

            ReloadList();
        }

        public void ReloadList(IEnumerable<Behaviour> newItemEnumerator)
        {
            _itemListEnumerator = newItemEnumerator;
            ReloadList();
        }

        public void ReloadList()
        {
            listView.BeginUpdate();

            listView.Items.Clear();

            var newListItems = ManualOrdering ? _itemListEnumerator : _itemListEnumerator.OrderBy(x => x.Name);

            Func<IBasicInfo, ListViewItem> createLvi =
                x => new ListViewItem(new[] {x.Name, x.Enabled.ToString(), x.GetDetails(), TriggerCounter.GetCounter(x).ToString()}) {Tag = x};

            if (_groupKeyGetter != null)
            {
                var groups = new Dictionary<string, ListViewGroup>();
                var defaultGroup = new ListViewGroup("Ungrouped");
                groups.Add("Ungrouped", defaultGroup);
                foreach (var item in newListItems)
                {
                    var lvi = createLvi(item);
                    var groupKey = _groupKeyGetter(item);
                    if (string.IsNullOrEmpty(groupKey))
                        defaultGroup.Items.Add(lvi);
                    else
                    {
                        ListViewGroup result;
                        if (!groups.TryGetValue(groupKey, out result))
                        {
                            result = new ListViewGroup(groupKey);
                            groups.Add(groupKey, result);
                        }
                        result.Items.Add(lvi);
                    }

                    listView.Items.Add(lvi);
                }

                listView.Groups.AddRange(groups.OrderBy(x => x.Key).Select(x => x.Value).ToArray());
            }
            else
            {
                listView.Items.AddRange(newListItems.Select(createLvi).ToArray());
            }

            listView.EndUpdate();
        }

        private void buttonTriggerNew_Click(object sender, EventArgs e)
        {
            var result = _launchEditor(this, null);
            if (result != null)
            {
                _addItem(result);
                ReloadList();
            }
        }

        private IBasicInfo GetSelectedElement() => listView.SelectedItems.Cast<ListViewItem>()
            .Select(x => (IBasicInfo) x.Tag).FirstOrDefault();

        private void buttonTriggerEdit_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedElement();
            if (selected == null) return;

            var result = _launchEditor(this, selected);
            if (result != null)
            {
                _removeItem(selected);
                if (_insertItem == null)
                    _addItem(result);
                else
				{
					int index = listView.SelectedIndices[0];
                    _insertItem(index, result);
				}
                ReloadList();
            }
        }

        private void buttonTriggerDelete_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedElement();
            if (selected != null)
            {
                _removeItem(selected);
                ReloadList();
            }
        }

        private void buttonActionUp_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedElement();
            if (selected != null)
            {
                int index = listView.SelectedIndices[0];
                _downItem(selected);
                ReloadList();
                index = Math.Max(--index, 0);
                listView.Items[index].Selected = true;
                listView.Select();
            }
        }

        private void buttonActionDown_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedElement();
            if (selected != null)
            {
                int index = listView.SelectedIndices[0];
                _upItem(selected);
                ReloadList();
                index = Math.Min(++index, listView.Items.Count - 1);
                listView.Items[index].Selected = true;
                listView.Select();
            }
        }

        private void buttonDuplicate_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedElement();
            if (selected != null)
            {
                _addItem((IBasicInfo) selected.Clone());
                ReloadList();
            }
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                buttonTriggerEdit_Click(sender, e);
        }

        private void buttonDeleteAll_Click(object sender, EventArgs e)
        {
            _clearItems();
            ReloadList();
        }
    }
}