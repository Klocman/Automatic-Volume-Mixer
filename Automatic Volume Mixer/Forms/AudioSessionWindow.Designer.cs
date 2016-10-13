namespace Avm.Forms
{
    partial class AudioSessionWindow
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
            this.audioSessionViewer1 = new Avm.Controls.AudioSessionViewer();
            this.SuspendLayout();
            // 
            // audioSessionViewer1
            // 
            this.audioSessionViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.audioSessionViewer1.Location = new System.Drawing.Point(0, 0);
            this.audioSessionViewer1.Name = "audioSessionViewer1";
            this.audioSessionViewer1.Size = new System.Drawing.Size(488, 189);
            this.audioSessionViewer1.TabIndex = 0;
            // 
            // AudioSessionWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 189);
            this.Controls.Add(this.audioSessionViewer1);
            this.Name = "AudioSessionWindow";
            this.Text = "Automatic Volume Mixer - Running audio sessions";
            this.Shown += new System.EventHandler(this.AudioSessionWindow_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.AudioSessionViewer audioSessionViewer1;
    }
}