namespace Avm.Controls
{
    partial class ElementList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeaderActionType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderActionEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderActionDetails = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderStats = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonDeleteAll = new System.Windows.Forms.Button();
            this.buttonActionUp = new System.Windows.Forms.Button();
            this.buttonActionDown = new System.Windows.Forms.Button();
            this.buttonActionNew = new System.Windows.Forms.Button();
            this.buttonDuplicate = new System.Windows.Forms.Button();
            this.buttonActionDelete = new System.Windows.Forms.Button();
            this.buttonActionEdit = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderActionType,
            this.columnHeaderActionEnabled,
            this.columnHeaderActionDetails,
            this.columnHeaderStats});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 19);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size(852, 386);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.buttonTriggerEdit_Click);
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            // 
            // columnHeaderActionType
            // 
            this.columnHeaderActionType.Text = "Name";
            this.columnHeaderActionType.Width = 136;
            // 
            // columnHeaderActionEnabled
            // 
            this.columnHeaderActionEnabled.Text = "Enabled";
            // 
            // columnHeaderActionDetails
            // 
            this.columnHeaderActionDetails.Text = "Details";
            this.columnHeaderActionDetails.Width = 162;
            // 
            // columnHeaderStats
            // 
            this.columnHeaderStats.Text = "Statistics";
            this.columnHeaderStats.Width = 163;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonDeleteAll);
            this.panel2.Controls.Add(this.buttonActionUp);
            this.panel2.Controls.Add(this.buttonActionDown);
            this.panel2.Controls.Add(this.buttonActionNew);
            this.panel2.Controls.Add(this.buttonDuplicate);
            this.panel2.Controls.Add(this.buttonActionDelete);
            this.panel2.Controls.Add(this.buttonActionEdit);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 405);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(852, 28);
            this.panel2.TabIndex = 1;
            // 
            // buttonDeleteAll
            // 
            this.buttonDeleteAll.Location = new System.Drawing.Point(264, 5);
            this.buttonDeleteAll.Name = "buttonDeleteAll";
            this.buttonDeleteAll.Size = new System.Drawing.Size(60, 23);
            this.buttonDeleteAll.TabIndex = 6;
            this.buttonDeleteAll.Text = "Delete all";
            this.buttonDeleteAll.UseVisualStyleBackColor = true;
            this.buttonDeleteAll.Click += new System.EventHandler(this.buttonDeleteAll_Click);
            // 
            // buttonActionUp
            // 
            this.buttonActionUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonActionUp.Location = new System.Drawing.Point(736, 5);
            this.buttonActionUp.Name = "buttonActionUp";
            this.buttonActionUp.Size = new System.Drawing.Size(55, 23);
            this.buttonActionUp.TabIndex = 4;
            this.buttonActionUp.Text = "Up";
            this.buttonActionUp.UseVisualStyleBackColor = true;
            this.buttonActionUp.Click += new System.EventHandler(this.buttonActionUp_Click);
            // 
            // buttonActionDown
            // 
            this.buttonActionDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonActionDown.Location = new System.Drawing.Point(797, 5);
            this.buttonActionDown.Name = "buttonActionDown";
            this.buttonActionDown.Size = new System.Drawing.Size(55, 23);
            this.buttonActionDown.TabIndex = 5;
            this.buttonActionDown.Text = "Down";
            this.buttonActionDown.UseVisualStyleBackColor = true;
            this.buttonActionDown.Click += new System.EventHandler(this.buttonActionDown_Click);
            // 
            // buttonActionNew
            // 
            this.buttonActionNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonActionNew.Location = new System.Drawing.Point(0, 5);
            this.buttonActionNew.Name = "buttonActionNew";
            this.buttonActionNew.Size = new System.Drawing.Size(60, 23);
            this.buttonActionNew.TabIndex = 0;
            this.buttonActionNew.Text = "New...";
            this.buttonActionNew.UseVisualStyleBackColor = true;
            this.buttonActionNew.Click += new System.EventHandler(this.buttonTriggerNew_Click);
            // 
            // buttonDuplicate
            // 
            this.buttonDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDuplicate.Location = new System.Drawing.Point(132, 5);
            this.buttonDuplicate.Name = "buttonDuplicate";
            this.buttonDuplicate.Size = new System.Drawing.Size(60, 23);
            this.buttonDuplicate.TabIndex = 2;
            this.buttonDuplicate.Text = "Copy";
            this.buttonDuplicate.UseVisualStyleBackColor = true;
            this.buttonDuplicate.Click += new System.EventHandler(this.buttonDuplicate_Click);
            // 
            // buttonActionDelete
            // 
            this.buttonActionDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonActionDelete.Location = new System.Drawing.Point(198, 5);
            this.buttonActionDelete.Name = "buttonActionDelete";
            this.buttonActionDelete.Size = new System.Drawing.Size(60, 23);
            this.buttonActionDelete.TabIndex = 3;
            this.buttonActionDelete.Text = "Delete";
            this.buttonActionDelete.UseVisualStyleBackColor = true;
            this.buttonActionDelete.Click += new System.EventHandler(this.buttonTriggerDelete_Click);
            // 
            // buttonActionEdit
            // 
            this.buttonActionEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonActionEdit.Location = new System.Drawing.Point(66, 5);
            this.buttonActionEdit.Name = "buttonActionEdit";
            this.buttonActionEdit.Size = new System.Drawing.Size(60, 23);
            this.buttonActionEdit.TabIndex = 1;
            this.buttonActionEdit.Text = "Edit...";
            this.buttonActionEdit.UseVisualStyleBackColor = true;
            this.buttonActionEdit.Click += new System.EventHandler(this.buttonTriggerEdit_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(852, 19);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.label1.Size = new System.Drawing.Size(64, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Element List";
            // 
            // ElementList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "ElementList";
            this.Size = new System.Drawing.Size(852, 433);
            this.panel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeaderActionType;
        private System.Windows.Forms.ColumnHeader columnHeaderActionEnabled;
        private System.Windows.Forms.ColumnHeader columnHeaderActionDetails;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonActionUp;
        private System.Windows.Forms.Button buttonActionDown;
        private System.Windows.Forms.Button buttonActionNew;
        private System.Windows.Forms.Button buttonActionDelete;
        private System.Windows.Forms.Button buttonActionEdit;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDuplicate;
        private System.Windows.Forms.ColumnHeader columnHeaderStats;
        private System.Windows.Forms.Button buttonDeleteAll;
    }
}
