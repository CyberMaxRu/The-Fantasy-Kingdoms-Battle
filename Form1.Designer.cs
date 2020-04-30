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
            this.tabPageEconomic = new System.Windows.Forms.TabPage();
            this.tabPageChieftain = new System.Windows.Forms.TabPage();
            this.labelChieftainExp = new System.Windows.Forms.Label();
            this.LabelChieftainLevel = new System.Windows.Forms.Label();
            this.tabPageArmy = new System.Windows.Forms.TabPage();
            this.tabPageBattle = new System.Windows.Forms.TabPage();
            this.textBoxResultBattle = new System.Windows.Forms.TextBox();
            this.ButtonEndTurn = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tstbTurn = new System.Windows.Forms.ToolStripTextBox();
            this.tabPageTemples = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPageChieftain.SuspendLayout();
            this.tabPageBattle.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStrip
            // 
            this.StatusStrip.Location = new System.Drawing.Point(0, 721);
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
            this.tabControl1.Controls.Add(this.tabPageEconomic);
            this.tabControl1.Controls.Add(this.tabPageTemples);
            this.tabControl1.Controls.Add(this.tabPageChieftain);
            this.tabControl1.Controls.Add(this.tabPageArmy);
            this.tabControl1.Controls.Add(this.tabPageBattle);
            this.tabControl1.Location = new System.Drawing.Point(8, 40);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(960, 656);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageLobby
            // 
            this.tabPageLobby.Location = new System.Drawing.Point(4, 25);
            this.tabPageLobby.Name = "tabPageLobby";
            this.tabPageLobby.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLobby.Size = new System.Drawing.Size(952, 627);
            this.tabPageLobby.TabIndex = 0;
            this.tabPageLobby.Text = "Лобби";
            this.tabPageLobby.UseVisualStyleBackColor = true;
            // 
            // tabPageGuilds
            // 
            this.tabPageGuilds.Location = new System.Drawing.Point(4, 25);
            this.tabPageGuilds.Name = "tabPageGuilds";
            this.tabPageGuilds.Size = new System.Drawing.Size(952, 627);
            this.tabPageGuilds.TabIndex = 6;
            this.tabPageGuilds.Text = "Гильдии";
            this.tabPageGuilds.UseVisualStyleBackColor = true;
            // 
            // tabPageEconomic
            // 
            this.tabPageEconomic.Location = new System.Drawing.Point(4, 25);
            this.tabPageEconomic.Name = "tabPageEconomic";
            this.tabPageEconomic.Size = new System.Drawing.Size(952, 627);
            this.tabPageEconomic.TabIndex = 2;
            this.tabPageEconomic.Text = "Экономика";
            this.tabPageEconomic.UseVisualStyleBackColor = true;
            // 
            // tabPageChieftain
            // 
            this.tabPageChieftain.Controls.Add(this.labelChieftainExp);
            this.tabPageChieftain.Controls.Add(this.LabelChieftainLevel);
            this.tabPageChieftain.Location = new System.Drawing.Point(4, 25);
            this.tabPageChieftain.Name = "tabPageChieftain";
            this.tabPageChieftain.Size = new System.Drawing.Size(952, 627);
            this.tabPageChieftain.TabIndex = 4;
            this.tabPageChieftain.Text = "Военачальник";
            this.tabPageChieftain.UseVisualStyleBackColor = true;
            // 
            // labelChieftainExp
            // 
            this.labelChieftainExp.AutoSize = true;
            this.labelChieftainExp.Location = new System.Drawing.Point(8, 48);
            this.labelChieftainExp.Name = "labelChieftainExp";
            this.labelChieftainExp.Size = new System.Drawing.Size(37, 13);
            this.labelChieftainExp.TabIndex = 1;
            this.labelChieftainExp.Text = "Опыт:";
            // 
            // LabelChieftainLevel
            // 
            this.LabelChieftainLevel.AutoSize = true;
            this.LabelChieftainLevel.Location = new System.Drawing.Point(8, 24);
            this.LabelChieftainLevel.Name = "LabelChieftainLevel";
            this.LabelChieftainLevel.Size = new System.Drawing.Size(54, 13);
            this.LabelChieftainLevel.TabIndex = 0;
            this.LabelChieftainLevel.Text = "Уровень:";
            // 
            // tabPageArmy
            // 
            this.tabPageArmy.Location = new System.Drawing.Point(4, 25);
            this.tabPageArmy.Name = "tabPageArmy";
            this.tabPageArmy.Size = new System.Drawing.Size(952, 627);
            this.tabPageArmy.TabIndex = 3;
            this.tabPageArmy.Text = "Войско";
            this.tabPageArmy.UseVisualStyleBackColor = true;
            // 
            // tabPageBattle
            // 
            this.tabPageBattle.Controls.Add(this.textBoxResultBattle);
            this.tabPageBattle.Location = new System.Drawing.Point(4, 25);
            this.tabPageBattle.Name = "tabPageBattle";
            this.tabPageBattle.Size = new System.Drawing.Size(952, 627);
            this.tabPageBattle.TabIndex = 5;
            this.tabPageBattle.Text = "Сражение";
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
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tstbTurn});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(979, 27);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tstbTurn
            // 
            this.tstbTurn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tstbTurn.Name = "tstbTurn";
            this.tstbTurn.Size = new System.Drawing.Size(100, 23);
            // 
            // tabPageTemples
            // 
            this.tabPageTemples.Location = new System.Drawing.Point(4, 25);
            this.tabPageTemples.Name = "tabPageTemples";
            this.tabPageTemples.Size = new System.Drawing.Size(952, 627);
            this.tabPageTemples.TabIndex = 7;
            this.tabPageTemples.Text = "Храмы";
            this.tabPageTemples.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 743);
            this.Controls.Add(this.ButtonEndTurn);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fantasy King\'s Battle prototype";
            this.tabControl1.ResumeLayout(false);
            this.tabPageChieftain.ResumeLayout(false);
            this.tabPageChieftain.PerformLayout();
            this.tabPageBattle.ResumeLayout(false);
            this.tabPageBattle.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLobby;
        private System.Windows.Forms.TabPage tabPageEconomic;
        private System.Windows.Forms.TabPage tabPageChieftain;
        private System.Windows.Forms.TabPage tabPageArmy;
        private System.Windows.Forms.TabPage tabPageBattle;
        private System.Windows.Forms.Button ButtonEndTurn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripTextBox tstbTurn;
        private System.Windows.Forms.Label labelChieftainExp;
        private System.Windows.Forms.Label LabelChieftainLevel;
        private System.Windows.Forms.TextBox textBoxResultBattle;
        private System.Windows.Forms.TabPage tabPageGuilds;
        private System.Windows.Forms.TabPage tabPageTemples;
    }
}

