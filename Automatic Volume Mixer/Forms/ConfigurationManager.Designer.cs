namespace Avm.Forms
{
    partial class ConfigurationManager
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonViewSessions = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSett = new System.Windows.Forms.ToolStripButton();
            this.elementList1 = new Avm.Controls.ElementList();
            this.toolStripButtonViewVariables = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonImport,
            this.toolStripButtonExp,
            this.toolStripSeparator1,
            this.toolStripButtonViewSessions,
            this.toolStripButtonViewVariables,
            this.toolStripButtonSett});
            this.toolStrip1.Location = new System.Drawing.Point(8, 8);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(499, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripButtonImport
            // 
            this.toolStripButtonImport.Image = global::Avm.Properties.Resources.import;
            this.toolStripButtonImport.Name = "toolStripButtonImport";
            this.toolStripButtonImport.Size = new System.Drawing.Size(72, 22);
            this.toolStripButtonImport.Text = "Import...";
            // 
            // toolStripButtonExp
            // 
            this.toolStripButtonExp.Image = global::Avm.Properties.Resources.export;
            this.toolStripButtonExp.Name = "toolStripButtonExp";
            this.toolStripButtonExp.Size = new System.Drawing.Size(69, 22);
            this.toolStripButtonExp.Text = "Export...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonViewSessions
            // 
            this.toolStripButtonViewSessions.Image = global::Avm.Properties.Resources.app;
            this.toolStripButtonViewSessions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonViewSessions.Name = "toolStripButtonViewSessions";
            this.toolStripButtonViewSessions.Size = new System.Drawing.Size(131, 22);
            this.toolStripButtonViewSessions.Text = "View audio sessions";
            // 
            // toolStripButtonSett
            // 
            this.toolStripButtonSett.Image = global::Avm.Properties.Resources.settings;
            this.toolStripButtonSett.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSett.Name = "toolStripButtonSett";
            this.toolStripButtonSett.Size = new System.Drawing.Size(69, 22);
            this.toolStripButtonSett.Text = "Settings";
            // 
            // elementList1
            // 
            this.elementList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementList1.Location = new System.Drawing.Point(8, 33);
            this.elementList1.Name = "elementList1";
            this.elementList1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.elementList1.Size = new System.Drawing.Size(499, 269);
            this.elementList1.TabIndex = 1;
            this.elementList1.Text = "This list contains events that will be tested and executed depending on the state" +
    " of the audio sessions.";
            // 
            // toolStripButtonViewVariables
            // 
            this.toolStripButtonViewVariables.Image = global::Avm.Properties.Resources.app;
            this.toolStripButtonViewVariables.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonViewVariables.Name = "toolStripButtonViewVariables";
            this.toolStripButtonViewVariables.Size = new System.Drawing.Size(101, 22);
            this.toolStripButtonViewVariables.Text = "View variables";
            // 
            // ConfigurationManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 310);
            this.Controls.Add(this.elementList1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ConfigurationManager";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Text = "Automatic Volume Mixer - Event Manager";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigurationManager_FormClosed);
            this.Shown += new System.EventHandler(this.ConfigurationManager_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Controls.ElementList elementList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonImport;
        private System.Windows.Forms.ToolStripButton toolStripButtonExp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonViewSessions;
        private System.Windows.Forms.ToolStripButton toolStripButtonSett;
        private System.Windows.Forms.ToolStripButton toolStripButtonViewVariables;
    }
}