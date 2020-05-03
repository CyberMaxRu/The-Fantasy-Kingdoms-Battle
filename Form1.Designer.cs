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
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLobby = new System.Windows.Forms.TabPage();
            this.tabPageGuilds = new System.Windows.Forms.TabPage();
            this.tabPageBuildings = new System.Windows.Forms.TabPage();
            this.tabPageTemples = new System.Windows.Forms.TabPage();
            this.tabPageHeroes = new System.Windows.Forms.TabPage();
            this.tabPageBattle = new System.Windows.Forms.TabPage();
            this.textBoxResultBattle = new System.Windows.Forms.TextBox();
            this.ButtonEndTurn = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tabControl1.SuspendLayout();
            this.tabPageBattle.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStrip
            // 
            this.StatusStrip.Location = new System.Drawing.Point(0, 747);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.ShowItemToolTips = true;
            this.StatusStrip.Size = new System.Drawing.Size(979, 22);
            this.StatusStrip.SizingGrip = false;
            this.StatusStrip.TabIndex = 0;
            this.StatusStrip.Text = "statusStrip";
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPageLobby);
            this.tabControl1.Controls.Add(this.tabPageGuilds);
            this.tabControl1.Controls.Add(this.tabPageBuildings);
            this.tabControl1.Controls.Add(this.tabPageTemples);
            this.tabControl1.Controls.Add(this.tabPageHeroes);
            this.tabControl1.Controls.Add(this.tabPageBattle);
            this.tabControl1.Location = new System.Drawing.Point(8, 40);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(960, 696);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageLobby
            // 
            this.tabPageLobby.AllowDrop = true;
            this.tabPageLobby.Location = new System.Drawing.Point(4, 25);
            this.tabPageLobby.Name = "tabPageLobby";
            this.tabPageLobby.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLobby.Size = new System.Drawing.Size(952, 667);
            this.tabPageLobby.TabIndex = 0;
            this.tabPageLobby.Text = "Лобби";
            this.tabPageLobby.ToolTipText = "Лобби";
            this.tabPageLobby.UseVisualStyleBackColor = true;
            // 
            // tabPageGuilds
            // 
            this.tabPageGuilds.Location = new System.Drawing.Point(4, 25);
            this.tabPageGuilds.Name = "tabPageGuilds";
            this.tabPageGuilds.Size = new System.Drawing.Size(952, 667);
            this.tabPageGuilds.TabIndex = 6;
            this.tabPageGuilds.Text = "Гильдии";
            this.tabPageGuilds.ToolTipText = "Гильдии";
            this.tabPageGuilds.UseVisualStyleBackColor = true;
            // 
            // tabPageBuildings
            // 
            this.tabPageBuildings.Location = new System.Drawing.Point(4, 25);
            this.tabPageBuildings.Name = "tabPageBuildings";
            this.tabPageBuildings.Size = new System.Drawing.Size(952, 667);
            this.tabPageBuildings.TabIndex = 2;
            this.tabPageBuildings.Text = "Здания";
            this.tabPageBuildings.ToolTipText = "Экономические и прочие здания";
            this.tabPageBuildings.UseVisualStyleBackColor = true;
            // 
            // tabPageTemples
            // 
            this.tabPageTemples.Location = new System.Drawing.Point(4, 25);
            this.tabPageTemples.Name = "tabPageTemples";
            this.tabPageTemples.Size = new System.Drawing.Size(952, 667);
            this.tabPageTemples.TabIndex = 7;
            this.tabPageTemples.Text = "Храмы";
            this.tabPageTemples.ToolTipText = "Храмы";
            this.tabPageTemples.UseVisualStyleBackColor = true;
            // 
            // tabPageHeroes
            // 
            this.tabPageHeroes.Location = new System.Drawing.Point(4, 25);
            this.tabPageHeroes.Name = "tabPageHeroes";
            this.tabPageHeroes.Size = new System.Drawing.Size(952, 667);
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
            this.tabPageBattle.Size = new System.Drawing.Size(952, 667);
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
            // ButtonEndTurn
            // 
            this.ButtonEndTurn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButtonEndTurn.Location = new System.Drawing.Point(832, 24);
            this.ButtonEndTurn.Name = "ButtonEndTurn";
            this.ButtonEndTurn.Size = new System.Drawing.Size(136, 48);
            this.ButtonEndTurn.TabIndex = 2;
            this.ButtonEndTurn.Text = "Конец\r\nхода";
            this.ButtonEndTurn.UseVisualStyleBackColor = true;
            this.ButtonEndTurn.Click += new System.EventHandler(this.ButtonEndTurn_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(979, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 769);
            this.Controls.Add(this.ButtonEndTurn);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fantasy King\'s Battle prototype";
            this.tabControl1.ResumeLayout(false);
            this.tabPageBattle.ResumeLayout(false);
            this.tabPageBattle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLobby;
        private System.Windows.Forms.TabPage tabPageBuildings;
        private System.Windows.Forms.TabPage tabPageHeroes;
        private System.Windows.Forms.TabPage tabPageBattle;
        private System.Windows.Forms.Button ButtonEndTurn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TextBox textBoxResultBattle;
        private System.Windows.Forms.TabPage tabPageGuilds;
        private System.Windows.Forms.TabPage tabPageTemples;
    }
}

