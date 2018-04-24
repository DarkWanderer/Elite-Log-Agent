namespace EliteLogAgent.Settings
{
    partial class GeneralSettingsControl
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
            this.uploadLatestDataButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // uploadLatestDataButton
            // 
            this.uploadLatestDataButton.Location = new System.Drawing.Point(3, 3);
            this.uploadLatestDataButton.Name = "uploadLatestDataButton";
            this.uploadLatestDataButton.Size = new System.Drawing.Size(184, 23);
            this.uploadLatestDataButton.TabIndex = 0;
            this.uploadLatestDataButton.Text = "Repopulate data for 10 days";
            this.uploadLatestDataButton.UseVisualStyleBackColor = true;
            this.uploadLatestDataButton.Click += new System.EventHandler(this.uploadLatestDataButton_Click);
            // 
            // GeneralSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uploadLatestDataButton);
            this.Name = "GeneralSettingsControl";
            this.Size = new System.Drawing.Size(300, 250);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uploadLatestDataButton;
    }
}
