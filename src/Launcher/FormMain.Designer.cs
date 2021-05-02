
namespace Launcher
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonPlay = new System.Windows.Forms.Button();
            this.flowLayoutPanelNews = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBoxBanner = new System.Windows.Forms.PictureBox();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.progressBarActions = new System.Windows.Forms.ProgressBar();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.labelValidate = new System.Windows.Forms.Label();
            this.labelServer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBanner)).BeginInit();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.panel3.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonPlay.BackColor = System.Drawing.Color.SeaGreen;
            this.buttonPlay.Enabled = false;
            this.buttonPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlay.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonPlay.Location = new System.Drawing.Point(328, 122);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(128, 48);
            this.buttonPlay.TabIndex = 5;
            this.buttonPlay.Text = "&ENTRAR";
            this.buttonPlay.UseVisualStyleBackColor = false;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // flowLayoutPanelNews
            // 
            this.flowLayoutPanelNews.AutoScroll = true;
            this.flowLayoutPanelNews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelNews.Location = new System.Drawing.Point(0, 192);
            this.flowLayoutPanelNews.Name = "flowLayoutPanelNews";
            this.flowLayoutPanelNews.Size = new System.Drawing.Size(784, 369);
            this.flowLayoutPanelNews.TabIndex = 10;
            // 
            // pictureBoxBanner
            // 
            this.pictureBoxBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBoxBanner.Image = global::Launcher.Properties.Resources.Background;
            this.pictureBoxBanner.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxBanner.Name = "pictureBoxBanner";
            this.pictureBoxBanner.Size = new System.Drawing.Size(784, 139);
            this.pictureBoxBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxBanner.TabIndex = 2;
            this.pictureBoxBanner.TabStop = false;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.panelHeader.Controls.Add(this.labelVersion);
            this.panelHeader.Controls.Add(this.labelTitle);
            this.panelHeader.Controls.Add(this.pictureBoxIcon);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(8);
            this.panelHeader.Size = new System.Drawing.Size(784, 46);
            this.panelHeader.TabIndex = 9;
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelVersion.Font = new System.Drawing.Font("Consolas", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelVersion.Location = new System.Drawing.Point(738, 8);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Padding = new System.Windows.Forms.Padding(4);
            this.labelVersion.Size = new System.Drawing.Size(38, 20);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "1.0.0";
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelTitle.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelTitle.Location = new System.Drawing.Point(38, 8);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Padding = new System.Windows.Forms.Padding(4);
            this.labelTitle.Size = new System.Drawing.Size(410, 30);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "MINECRAFT: LISIADOS";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBoxIcon.Image = global::Launcher.Properties.Resources.pack;
            this.pictureBoxIcon.Location = new System.Drawing.Point(8, 8);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxIcon.TabIndex = 3;
            this.pictureBoxIcon.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.panel3.Controls.Add(this.buttonPlay);
            this.panel3.Controls.Add(this.flowLayoutPanelNews);
            this.panel3.Controls.Add(this.progressBarActions);
            this.panel3.Controls.Add(this.panelStatus);
            this.panel3.Controls.Add(this.pictureBoxBanner);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(784, 561);
            this.panel3.TabIndex = 10;
            // 
            // progressBarActions
            // 
            this.progressBarActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBarActions.Location = new System.Drawing.Point(0, 188);
            this.progressBarActions.Name = "progressBarActions";
            this.progressBarActions.Size = new System.Drawing.Size(784, 4);
            this.progressBarActions.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarActions.TabIndex = 9;
            this.progressBarActions.Value = 100;
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.panelStatus.Controls.Add(this.labelValidate);
            this.panelStatus.Controls.Add(this.labelServer);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatus.Location = new System.Drawing.Point(0, 139);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(784, 49);
            this.panelStatus.TabIndex = 8;
            // 
            // labelValidate
            // 
            this.labelValidate.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelValidate.Font = new System.Drawing.Font("Consolas", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelValidate.Location = new System.Drawing.Point(0, 0);
            this.labelValidate.Name = "labelValidate";
            this.labelValidate.Padding = new System.Windows.Forms.Padding(8);
            this.labelValidate.Size = new System.Drawing.Size(256, 49);
            this.labelValidate.TabIndex = 2;
            this.labelValidate.Text = "Validando...";
            this.labelValidate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelServer
            // 
            this.labelServer.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelServer.Font = new System.Drawing.Font("Consolas", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelServer.Location = new System.Drawing.Point(528, 0);
            this.labelServer.Name = "labelServer";
            this.labelServer.Padding = new System.Windows.Forms.Padding(8);
            this.labelServer.Size = new System.Drawing.Size(256, 49);
            this.labelServer.TabIndex = 1;
            this.labelServer.Text = "Conectando...";
            this.labelServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panel3);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Minecraft: Lisiados";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBanner)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelNews;
        private System.Windows.Forms.PictureBox pictureBoxBanner;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ProgressBar progressBarActions;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.Label labelValidate;
    }
}