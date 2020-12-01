namespace Fantasy_King_s_Battle
{
    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLobby = new System.Windows.Forms.TabPage();
            this.tabPageGuilds = new System.Windows.Forms.TabPage();
            this.tabPageBuildings = new System.Windows.Forms.TabPage();
            this.tabPageTemples = new System.Windows.Forms.TabPage();
            this.tabPageTowers = new System.Windows.Forms.TabPage();
            this.tabPageHeroes = new System.Windows.Forms.TabPage();
            this.tabPageBattle = new System.Windows.Forms.TabPage();
            this.textBoxResultBattle = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tslDay = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tslGold = new System.Windows.Forms.ToolStripLabel();
            this.tslBuilders = new System.Windows.Forms.ToolStripLabel();
            this.tslHeroes = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbEndTurn = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.tabPageBattle.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPageLobby);
            this.tabControl1.Controls.Add(this.tabPageGuilds);
            this.tabControl1.Controls.Add(this.tabPageBuildings);
            this.tabControl1.Controls.Add(this.tabPageTemples);
            this.tabControl1.Controls.Add(this.tabPageTowers);
            this.tabControl1.Controls.Add(this.tabPageHeroes);
            this.tabControl1.Controls.Add(this.tabPageBattle);
            this.tabControl1.Location = new System.Drawing.Point(80, 100);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(896, 60);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPageLobby
            // 
            this.tabPageLobby.AllowDrop = true;
            this.tabPageLobby.Location = new System.Drawing.Point(4, 25);
            this.tabPageLobby.Name = "tabPageLobby";
            this.tabPageLobby.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLobby.Size = new System.Drawing.Size(888, 31);
            this.tabPageLobby.TabIndex = 0;
            this.tabPageLobby.Text = "Лобби";
            this.tabPageLobby.ToolTipText = "Лобби";
            this.tabPageLobby.UseVisualStyleBackColor = true;
            // 
            // tabPageGuilds
            // 
            this.tabPageGuilds.Location = new System.Drawing.Point(4, 25);
            this.tabPageGuilds.Name = "tabPageGuilds";
            this.tabPageGuilds.Size = new System.Drawing.Size(888, 31);
            this.tabPageGuilds.TabIndex = 6;
            this.tabPageGuilds.Text = "Гильдии";
            this.tabPageGuilds.ToolTipText = "Гильдии";
            this.tabPageGuilds.UseVisualStyleBackColor = true;
            // 
            // tabPageBuildings
            // 
            this.tabPageBuildings.Location = new System.Drawing.Point(4, 25);
            this.tabPageBuildings.Name = "tabPageBuildings";
            this.tabPageBuildings.Size = new System.Drawing.Size(888, 31);
            this.tabPageBuildings.TabIndex = 2;
            this.tabPageBuildings.Text = "Строения";
            this.tabPageBuildings.ToolTipText = "Экономические строения";
            this.tabPageBuildings.UseVisualStyleBackColor = true;
            // 
            // tabPageTemples
            // 
            this.tabPageTemples.Location = new System.Drawing.Point(4, 25);
            this.tabPageTemples.Name = "tabPageTemples";
            this.tabPageTemples.Size = new System.Drawing.Size(888, 31);
            this.tabPageTemples.TabIndex = 7;
            this.tabPageTemples.Text = "Храмы";
            this.tabPageTemples.ToolTipText = "Храмы";
            this.tabPageTemples.UseVisualStyleBackColor = true;
            // 
            // tabPageTowers
            // 
            this.tabPageTowers.Location = new System.Drawing.Point(4, 25);
            this.tabPageTowers.Name = "tabPageTowers";
            this.tabPageTowers.Size = new System.Drawing.Size(888, 31);
            this.tabPageTowers.TabIndex = 8;
            this.tabPageTowers.Text = "Защитные сооружения";
            this.tabPageTowers.ToolTipText = "Защитные сооружения";
            this.tabPageTowers.UseVisualStyleBackColor = true;
            // 
            // tabPageHeroes
            // 
            this.tabPageHeroes.Location = new System.Drawing.Point(4, 25);
            this.tabPageHeroes.Name = "tabPageHeroes";
            this.tabPageHeroes.Size = new System.Drawing.Size(888, 31);
            this.tabPageHeroes.TabIndex = 3;
            this.tabPageHeroes.Text = "Герои";
            this.tabPageHeroes.ToolTipText = "Герои";
            this.tabPageHeroes.UseVisualStyleBackColor = true;
            // 
            // tabPageBattle
            // 
            this.tabPageBattle.Controls.Add(this.textBoxResultBattle);
            this.tabPageBattle.Location = new System.Drawing.Point(4, 25);
            this.tabPageBattle.Name = "tabPageBattle";
            this.tabPageBattle.Size = new System.Drawing.Size(888, 31);
            this.tabPageBattle.TabIndex = 5;
            this.tabPageBattle.Text = "Сражение";
            this.tabPageBattle.ToolTipText = "Сражение";
            this.tabPageBattle.UseVisualStyleBackColor = true;
            // 
            // textBoxResultBattle
            // 
            this.textBoxResultBattle.Location = new System.Drawing.Point(8, 16);
            this.textBoxResultBattle.Multiline = true;
            this.textBoxResultBattle.Name = "textBoxResultBattle";
            this.textBoxResultBattle.Size = new System.Drawing.Size(752, 480);
            this.textBoxResultBattle.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(979, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslDay,
            this.toolStripSeparator1,
            this.tslGold,
            this.tslBuilders,
            this.tslHeroes,
            this.toolStripSeparator2,
            this.tsbEndTurn});
            this.toolStripMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStripMain.Location = new System.Drawing.Point(0, 24);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.ShowItemToolTips = false;
            this.toolStripMain.Size = new System.Drawing.Size(979, 51);
            this.toolStripMain.TabIndex = 5;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // tslDay
            // 
            this.tslDay.AutoSize = false;
            this.tslDay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tslDay.Name = "tslDay";
            this.tslDay.Size = new System.Drawing.Size(104, 48);
            this.tslDay.Text = "toolStripLabel1";
            this.tslDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tslDay.MouseLeave += new System.EventHandler(this.tsl_MouseLeave);
            this.tslDay.MouseHover += new System.EventHandler(this.tslDay_MouseHover);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 50);
            // 
            // tslGold
            // 
            this.tslGold.AutoSize = false;
            this.tslGold.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tslGold.Name = "tslGold";
            this.tslGold.Size = new System.Drawing.Size(200, 48);
            this.tslGold.Text = "toolStripLabel2";
            this.tslGold.MouseLeave += new System.EventHandler(this.tsl_MouseLeave);
            this.tslGold.MouseHover += new System.EventHandler(this.tslGold_MouseHover);
            // 
            // tslBuilders
            // 
            this.tslBuilders.AutoSize = false;
            this.tslBuilders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tslBuilders.Name = "tslBuilders";
            this.tslBuilders.Size = new System.Drawing.Size(120, 48);
            this.tslBuilders.Text = "toolStripLabel3";
            this.tslBuilders.MouseLeave += new System.EventHandler(this.tsl_MouseLeave);
            this.tslBuilders.MouseHover += new System.EventHandler(this.tslBuilders_MouseHover);
            // 
            // tslHeroes
            // 
            this.tslHeroes.AutoSize = false;
            this.tslHeroes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tslHeroes.Name = "tslHeroes";
            this.tslHeroes.Size = new System.Drawing.Size(120, 48);
            this.tslHeroes.Text = "tslHeroes";
            this.tslHeroes.MouseLeave += new System.EventHandler(this.tsl_MouseLeave);
            this.tslHeroes.MouseHover += new System.EventHandler(this.tslHeroes_MouseHover);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 50);
            // 
            // tsbEndTurn
            // 
            this.tsbEndTurn.AutoToolTip = false;
            this.tsbEndTurn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEndTurn.Image = ((System.Drawing.Image)(resources.GetObject("tsbEndTurn.Image")));
            this.tsbEndTurn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEndTurn.Name = "tsbEndTurn";
            this.tsbEndTurn.Size = new System.Drawing.Size(23, 20);
            this.tsbEndTurn.Text = "toolStripButton1";
            this.tsbEndTurn.Click += new System.EventHandler(this.tsbEndTurn_Click);
            this.tsbEndTurn.MouseLeave += new System.EventHandler(this.tsl_MouseLeave);
            this.tsbEndTurn.MouseHover += new System.EventHandler(this.tsbEndTurn_MouseHover);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 877);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fantasy King\'s Battle concept (build at 01.12.2020)";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.tabControl1.ResumeLayout(false);
            this.tabPageBattle.ResumeLayout(false);
            this.tabPageBattle.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLobby;
        private System.Windows.Forms.TabPage tabPageBuildings;
        private System.Windows.Forms.TabPage tabPageHeroes;
        private System.Windows.Forms.TabPage tabPageBattle;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TextBox textBoxResultBattle;
        private System.Windows.Forms.TabPage tabPageGuilds;
        private System.Windows.Forms.TabPage tabPageTemples;
        private System.Windows.Forms.TabPage tabPageTowers;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripLabel tslDay;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel tslGold;
        private System.Windows.Forms.ToolStripLabel tslBuilders;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbEndTurn;
        private System.Windows.Forms.ToolStripLabel tslHeroes;
    }
}

