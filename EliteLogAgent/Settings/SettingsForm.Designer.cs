namespace EliteLogAgent
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.applyButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.settingsCategoriesListView = new System.Windows.Forms.ListView();
            this.settingsCategoriesImageList = new System.Windows.Forms.ImageList(this.components);
            this.settingsControlContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyButton.Location = new System.Drawing.Point(12, 436);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(186, 23);
            this.applyButton.TabIndex = 6;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.Location = new System.Drawing.Point(12, 407);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(186, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(12, 465);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(186, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // settingsCategoriesListView
            // 
            this.settingsCategoriesListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.settingsCategoriesListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.settingsCategoriesListView.LabelWrap = false;
            this.settingsCategoriesListView.Location = new System.Drawing.Point(12, 13);
            this.settingsCategoriesListView.MultiSelect = false;
            this.settingsCategoriesListView.Name = "settingsCategoriesListView";
            this.settingsCategoriesListView.ShowGroups = false;
            this.settingsCategoriesListView.Size = new System.Drawing.Size(186, 388);
            this.settingsCategoriesListView.SmallImageList = this.settingsCategoriesImageList;
            this.settingsCategoriesListView.TabIndex = 8;
            this.settingsCategoriesListView.UseCompatibleStateImageBehavior = false;
            this.settingsCategoriesListView.View = System.Windows.Forms.View.List;
            this.settingsCategoriesListView.SelectedIndexChanged += new System.EventHandler(this.SettingsCategoriesListView_SelectedIndexChanged);
            // 
            // settingsCategoriesImageList
            // 
            this.settingsCategoriesImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.settingsCategoriesImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.settingsCategoriesImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // settingsControlContainer
            // 
            this.settingsControlContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsControlContainer.Location = new System.Drawing.Point(204, 13);
            this.settingsControlContainer.Name = "settingsControlContainer";
            this.settingsControlContainer.Size = new System.Drawing.Size(540, 475);
            this.settingsControlContainer.TabIndex = 9;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(756, 500);
            this.Controls.Add(this.settingsControlContainer);
            this.Controls.Add(this.settingsCategoriesListView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.applyButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Elite Log Agent - Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ListView settingsCategoriesListView;
        private System.Windows.Forms.Panel settingsControlContainer;
        private System.Windows.Forms.ImageList settingsCategoriesImageList;
    }
}

