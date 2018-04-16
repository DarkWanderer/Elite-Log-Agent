namespace TrayAgent
{
    partial class SettingsForm
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
            this.uploadLatestDataButton = new System.Windows.Forms.Button();
            this.uploadOnStartupCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.settingsTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // uploadLatestDataButton
            // 
            this.uploadLatestDataButton.Location = new System.Drawing.Point(345, 12);
            this.uploadLatestDataButton.Name = "uploadLatestDataButton";
            this.uploadLatestDataButton.Size = new System.Drawing.Size(114, 23);
            this.uploadLatestDataButton.TabIndex = 1;
            this.uploadLatestDataButton.Text = "Upload latest logs";
            this.uploadLatestDataButton.UseVisualStyleBackColor = true;
            this.uploadLatestDataButton.Click += new System.EventHandler(this.uploadLatestDataButton_Click);
            // 
            // uploadOnStartupCheckbox
            // 
            this.uploadOnStartupCheckbox.AutoSize = true;
            this.uploadOnStartupCheckbox.Location = new System.Drawing.Point(345, 41);
            this.uploadOnStartupCheckbox.Name = "uploadOnStartupCheckbox";
            this.uploadOnStartupCheckbox.Size = new System.Drawing.Size(114, 17);
            this.uploadOnStartupCheckbox.TabIndex = 2;
            this.uploadOnStartupCheckbox.Text = "Upload On Startup";
            this.uploadOnStartupCheckbox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(406, 271);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version: 1.2.3.123";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // settingsTree
            // 
            this.settingsTree.Location = new System.Drawing.Point(12, 12);
            this.settingsTree.Name = "settingsTree";
            this.settingsTree.Size = new System.Drawing.Size(236, 284);
            this.settingsTree.TabIndex = 3;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 305);
            this.Controls.Add(this.settingsTree);
            this.Controls.Add(this.uploadOnStartupCheckbox);
            this.Controls.Add(this.uploadLatestDataButton);
            this.Controls.Add(this.label1);
            this.Name = "SettingsForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button uploadLatestDataButton;
        private System.Windows.Forms.CheckBox uploadOnStartupCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView settingsTree;
    }
}

