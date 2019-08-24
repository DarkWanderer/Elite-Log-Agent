namespace DW.ELA.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Settings;
    using MoreLinq;
    using NLog;

    public class MultiCmdrApiKeyControl : AbstractSettingsControl
    {
        private LinkLabel apiKeyLabel;
        private DataGridView apiKeysGridView;
        private Button buttonAddEntry;
        private Button buttonDelEntry;
        private Button buttonValidateKeys;
        private DataGridViewTextBoxColumn apiKeysGridCommanderColumn;
        private DataGridViewTextBoxColumn apiKeysGridKeyColumn;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public MultiCmdrApiKeyControl()
        {
            InitializeComponent();
        }

        public IApiKeyValidator ApiKeyValidator { private get; set; }

        public string ApiSettingsLink { private get; set; }

        public Action<GlobalSettings, IReadOnlyDictionary<string, string>> SaveSettingsFunc { private get; set; }

        public IReadOnlyDictionary<string, string> ApiKeys
        {
            get
            {
                return apiKeysGridView.Rows
                    .Cast<DataGridViewRow>()
                    .Select(row => new KeyValuePair<string, string>(row.Cells[apiKeysGridCommanderColumn.Index].Value?.ToString(), row.Cells[apiKeysGridKeyColumn.Index].Value?.ToString()))
                    .Where(kvp => !string.IsNullOrEmpty(kvp.Key))
                    .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                    .ToDictionary();
            }
            set
            {
                apiKeysGridView.Rows.Clear();
                foreach (var kvp in value)
                    apiKeysGridView.Rows.Add(kvp.Key, kvp.Value, false);
            }
        }

        private void InitializeComponent()
        {
            this.apiKeyLabel = new System.Windows.Forms.LinkLabel();
            this.apiKeysGridView = new System.Windows.Forms.DataGridView();
            this.apiKeysGridCommanderColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.apiKeysGridKeyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonAddEntry = new System.Windows.Forms.Button();
            this.buttonDelEntry = new System.Windows.Forms.Button();
            this.buttonValidateKeys = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.apiKeysGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // apiKeyLabel
            // 
            this.apiKeyLabel.AutoSize = true;
            this.apiKeyLabel.Location = new System.Drawing.Point(3, 3);
            this.apiKeyLabel.Name = "apiKeyLabel";
            this.apiKeyLabel.Size = new System.Drawing.Size(82, 17);
            this.apiKeyLabel.TabIndex = 6;
            this.apiKeyLabel.TabStop = true;
            this.apiKeyLabel.Text = "Get API key";
            this.apiKeyLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ApiKeyLabel_LinkClicked);
            // 
            // apiKeysGridView
            // 
            this.apiKeysGridView.AllowUserToAddRows = false;
            this.apiKeysGridView.AllowUserToDeleteRows = false;
            this.apiKeysGridView.AllowUserToResizeRows = false;
            this.apiKeysGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.apiKeysGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.apiKeysGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.apiKeysGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.apiKeysGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.apiKeysGridCommanderColumn,
            this.apiKeysGridKeyColumn});
            this.apiKeysGridView.Location = new System.Drawing.Point(3, 26);
            this.apiKeysGridView.Name = "apiKeysGridView";
            this.apiKeysGridView.RowHeadersVisible = false;
            this.apiKeysGridView.RowHeadersWidth = 51;
            this.apiKeysGridView.RowTemplate.Height = 24;
            this.apiKeysGridView.Size = new System.Drawing.Size(479, 195);
            this.apiKeysGridView.TabIndex = 7;
            this.apiKeysGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ApiKeysGridView_CellContentClick);
            // 
            // apiKeysGridCommanderColumn
            // 
            this.apiKeysGridCommanderColumn.HeaderText = "CMDR";
            this.apiKeysGridCommanderColumn.MinimumWidth = 6;
            this.apiKeysGridCommanderColumn.Name = "apiKeysGridCommanderColumn";
            this.apiKeysGridCommanderColumn.Width = 77;
            // 
            // apiKeysGridKeyColumn
            // 
            this.apiKeysGridKeyColumn.HeaderText = "API Key";
            this.apiKeysGridKeyColumn.MinimumWidth = 6;
            this.apiKeysGridKeyColumn.Name = "apiKeysGridKeyColumn";
            this.apiKeysGridKeyColumn.Width = 86;
            // 
            // buttonAddEntry
            // 
            this.buttonAddEntry.Location = new System.Drawing.Point(91, 0);
            this.buttonAddEntry.Name = "buttonAddEntry";
            this.buttonAddEntry.Size = new System.Drawing.Size(46, 23);
            this.buttonAddEntry.TabIndex = 8;
            this.buttonAddEntry.Text = "Add";
            this.buttonAddEntry.UseVisualStyleBackColor = true;
            this.buttonAddEntry.Click += new System.EventHandler(this.ButtonAddEntry_Click);
            // 
            // buttonDelEntry
            // 
            this.buttonDelEntry.Location = new System.Drawing.Point(143, 0);
            this.buttonDelEntry.Name = "buttonDelEntry";
            this.buttonDelEntry.Size = new System.Drawing.Size(74, 23);
            this.buttonDelEntry.TabIndex = 9;
            this.buttonDelEntry.Text = "Remove";
            this.buttonDelEntry.UseVisualStyleBackColor = true;
            this.buttonDelEntry.Click += new System.EventHandler(this.ButtonDelEntry_Click);
            // 
            // buttonValidateKeys
            // 
            this.buttonValidateKeys.Location = new System.Drawing.Point(223, 0);
            this.buttonValidateKeys.Name = "buttonValidateKeys";
            this.buttonValidateKeys.Size = new System.Drawing.Size(86, 23);
            this.buttonValidateKeys.TabIndex = 10;
            this.buttonValidateKeys.Text = "Validate";
            this.buttonValidateKeys.UseVisualStyleBackColor = true;
            this.buttonValidateKeys.Click += new System.EventHandler(this.ButtonValidateKeys_ClickAsync);
            // 
            // MultiCmdrApiKeyControl
            // 
            this.Controls.Add(this.buttonValidateKeys);
            this.Controls.Add(this.buttonDelEntry);
            this.Controls.Add(this.buttonAddEntry);
            this.Controls.Add(this.apiKeysGridView);
            this.Controls.Add(this.apiKeyLabel);
            this.Name = "MultiCmdrApiKeyControl";
            this.Size = new System.Drawing.Size(485, 224);
            ((System.ComponentModel.ISupportInitialize)(this.apiKeysGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public override void SaveSettings() => SaveSettingsFunc?.Invoke(GlobalSettings, ApiKeys);

        private void ApiKeyLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start(ApiSettingsLink);

        private async void ButtonValidateKeys_ClickAsync(object sender, EventArgs e)
        {
            if (ApiKeyValidator == null)
                return;
            var validator = ApiKeyValidator;

            buttonValidateKeys.Enabled = false;
            try
            {
                foreach (var row in apiKeysGridView.Rows.Cast<DataGridViewRow>() )
                {
                    var cmdrNameCell = row.Cells[apiKeysGridCommanderColumn.Index];
                    var apiKeyCell = row.Cells[apiKeysGridKeyColumn.Index];
                    apiKeyCell.Style.BackColor = Color.LightGray;

                    var cmdrName = cmdrNameCell.Value?.ToString();
                    var apiKey = apiKeyCell.Value?.ToString();
                    var isValid = await validator.ValidateKeyAsync(cmdrName, apiKey);

                    apiKeyCell.Style.BackColor = isValid ? Color.LightGreen : Color.LightSalmon;
                    cmdrNameCell.Style.BackColor = apiKeyCell.Style.BackColor;
                }
            }
            catch
            {
            }
            finally
            {
                buttonValidateKeys.Enabled = true;
            }
        }

        private void ButtonAddEntry_Click(object sender, EventArgs e) => apiKeysGridView.Rows.Add();

        private void ButtonDelEntry_Click(object sender, EventArgs e)
        {
            var selectedRows = apiKeysGridView.SelectedCells.Cast<DataGridViewCell>().Select(c => c.OwningRow);
            foreach (var row in selectedRows.Cast<DataGridViewRow>())
                apiKeysGridView.Rows.Remove(row);
        }

        private void ApiKeysGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
