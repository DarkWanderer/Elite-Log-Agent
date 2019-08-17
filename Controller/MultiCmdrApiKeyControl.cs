namespace DW.ELA.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Settings;
    using NLog;

    public class MultiCmdrApiKeyControl : AbstractSettingsControl
    {
        private LinkLabel apiKeyLabel;
        private DataGridView apiKeysGridView;
        private DataGridViewTextBoxColumn apiKeysGridCommanderColumn;
        private DataGridViewTextBoxColumn apiKeysGridKeyColumn;
        private DataGridViewCheckBoxColumn apiKeysGridIsValidColumn;
        private Button buttonAddEntry;
        private Button buttonDelEntry;
        private Button buttonValidateKeys;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public MultiCmdrApiKeyControl()
        {
            InitializeComponent();
        }

        public Func<string, string, Task<bool>> ValidateApiKeyFunc { private get; set; }

        public Action<GlobalSettings, IReadOnlyDictionary<string, string>> SaveSettingsFunc { private get; set; }

        public IReadOnlyDictionary<string, string> ApiKeys
        {
            get
            {
                return apiKeysGridView.Rows
                    .Cast<DataGridViewRow>()
                    .ToDictionary(row => row.Cells[apiKeysGridCommanderColumn.Index].Value.ToString(), row => row.Cells[apiKeysGridKeyColumn.Index].Value.ToString());
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
            apiKeyLabel = new LinkLabel();
            apiKeysGridView = new DataGridView();
            apiKeysGridCommanderColumn = new DataGridViewTextBoxColumn();
            apiKeysGridKeyColumn = new DataGridViewTextBoxColumn();
            apiKeysGridIsValidColumn = new DataGridViewCheckBoxColumn();
            buttonAddEntry = new Button();
            buttonDelEntry = new Button();
            buttonValidateKeys = new Button();
            ((System.ComponentModel.ISupportInitialize)apiKeysGridView).BeginInit();
            SuspendLayout();
            // 
            // apiKeyLabel
            // 
            apiKeyLabel.AutoSize = true;
            apiKeyLabel.Location = new System.Drawing.Point(3, 3);
            apiKeyLabel.Name = "apiKeyLabel";
            apiKeyLabel.Size = new System.Drawing.Size(105, 17);
            apiKeyLabel.TabIndex = 6;
            apiKeyLabel.TabStop = true;
            apiKeyLabel.Text = "EDSM API keys";
            apiKeyLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(ApiKeyLabel_LinkClicked);
            // 
            // apiKeysGridView
            // 
            apiKeysGridView.AllowUserToAddRows = false;
            apiKeysGridView.AllowUserToDeleteRows = false;
            apiKeysGridView.AllowUserToResizeRows = false;
            apiKeysGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            | AnchorStyles.Left
            | AnchorStyles.Right;
            apiKeysGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            apiKeysGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            apiKeysGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            apiKeysGridView.Columns.AddRange(new DataGridViewColumn[] {
            apiKeysGridCommanderColumn,
            apiKeysGridKeyColumn,
            apiKeysGridIsValidColumn});
            apiKeysGridView.Location = new System.Drawing.Point(3, 26);
            apiKeysGridView.Name = "apiKeysGridView";
            apiKeysGridView.RowHeadersVisible = false;
            apiKeysGridView.RowHeadersWidth = 51;
            apiKeysGridView.RowTemplate.Height = 24;
            apiKeysGridView.Size = new System.Drawing.Size(479, 195);
            apiKeysGridView.TabIndex = 7;
            // 
            // apiKeysGridCommanderColumn
            // 
            apiKeysGridCommanderColumn.HeaderText = "CMDR";
            apiKeysGridCommanderColumn.MinimumWidth = 6;
            apiKeysGridCommanderColumn.Name = "apiKeysGridCommanderColumn";
            apiKeysGridCommanderColumn.Width = 77;
            // 
            // apiKeysGridKeyColumn
            // 
            apiKeysGridKeyColumn.HeaderText = "API Key";
            apiKeysGridKeyColumn.MinimumWidth = 6;
            apiKeysGridKeyColumn.Name = "apiKeysGridKeyColumn";
            apiKeysGridKeyColumn.Width = 86;
            // 
            // apiKeysGridIsValidColumn
            // 
            apiKeysGridIsValidColumn.HeaderText = "Valid";
            apiKeysGridIsValidColumn.MinimumWidth = 6;
            apiKeysGridIsValidColumn.Name = "apiKeysGridIsValidColumn";
            apiKeysGridIsValidColumn.ReadOnly = true;
            apiKeysGridIsValidColumn.Width = 45;
            // 
            // buttonAddEntry
            // 
            buttonAddEntry.Location = new System.Drawing.Point(114, 0);
            buttonAddEntry.Name = "buttonAddEntry";
            buttonAddEntry.Size = new System.Drawing.Size(23, 23);
            buttonAddEntry.TabIndex = 8;
            buttonAddEntry.Text = "+";
            buttonAddEntry.UseVisualStyleBackColor = true;
            buttonAddEntry.Click += new EventHandler(ButtonAddEntry_Click);
            // 
            // buttonDelEntry
            // 
            buttonDelEntry.Location = new System.Drawing.Point(143, 0);
            buttonDelEntry.Name = "buttonDelEntry";
            buttonDelEntry.Size = new System.Drawing.Size(23, 23);
            buttonDelEntry.TabIndex = 9;
            buttonDelEntry.Text = "-";
            buttonDelEntry.UseVisualStyleBackColor = true;
            buttonDelEntry.Click += new EventHandler(ButtonDelEntry_Click);
            // 
            // buttonValidateKeys
            // 
            buttonValidateKeys.Location = new System.Drawing.Point(172, 0);
            buttonValidateKeys.Name = "buttonValidateKeys";
            buttonValidateKeys.Size = new System.Drawing.Size(86, 23);
            buttonValidateKeys.TabIndex = 10;
            buttonValidateKeys.Text = "Validate";
            buttonValidateKeys.UseVisualStyleBackColor = true;
            buttonValidateKeys.Click += new EventHandler(ButtonValidateKeys_ClickAsync);
            // 
            // MultiCmdrApiKeyControl
            // 
            Controls.Add(buttonValidateKeys);
            Controls.Add(buttonDelEntry);
            Controls.Add(buttonAddEntry);
            Controls.Add(apiKeysGridView);
            Controls.Add(apiKeyLabel);
            Name = "MultiCmdrApiKeyControl";
            Size = new System.Drawing.Size(485, 224);
            ((System.ComponentModel.ISupportInitialize)apiKeysGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        public override void SaveSettings() => SaveSettingsFunc?.Invoke(GlobalSettings, ApiKeys);

        private void ApiKeyLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://www.edsm.net/en/settings/api");

        private async void ButtonValidateKeys_ClickAsync(object sender, EventArgs e)
        {
            if (ValidateApiKeyFunc == null)
                return;

            buttonValidateKeys.Enabled = false;
            try
            {
                var tasks = ApiKeys.Select(kvp => ValidateApiKeyFunc(kvp.Key, kvp.Value));
                _ = await Task.WhenAll(tasks);
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
            var selectedRows = apiKeysGridView.SelectedRows;
            foreach (var row in selectedRows.Cast<DataGridViewRow>())
                apiKeysGridView.Rows.Remove(row);
        }
    }
}
