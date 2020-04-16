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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPageExternal = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPageChieftain = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.ButtonEndTurn = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tstbTurn = new System.Windows.Forms.ToolStripTextBox();
            this.LabelChieftainLevel = new System.Windows.Forms.Label();
            this.labelChieftainExp = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageChieftain.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPageExternal);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPageChieftain);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Location = new System.Drawing.Point(8, 40);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(784, 656);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(776, 627);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Игроки";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPageExternal
            // 
            this.tabPageExternal.Location = new System.Drawing.Point(4, 25);
            this.tabPageExternal.Name = "tabPageExternal";
            this.tabPageExternal.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExternal.Size = new System.Drawing.Size(776, 627);
            this.tabPageExternal.TabIndex = 1;
            this.tabPageExternal.Text = "Окрестности";
            this.tabPageExternal.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(776, 627);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Замок";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPageChieftain
            // 
            this.tabPageChieftain.Controls.Add(this.labelChieftainExp);
            this.tabPageChieftain.Controls.Add(this.LabelChieftainLevel);
            this.tabPageChieftain.Location = new System.Drawing.Point(4, 25);
            this.tabPageChieftain.Name = "tabPageChieftain";
            this.tabPageChieftain.Size = new System.Drawing.Size(776, 627);
            this.tabPageChieftain.TabIndex = 4;
            this.tabPageChieftain.Text = "Военачальник";
            this.tabPageChieftain.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(776, 627);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Войско";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 25);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(776, 627);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Сражение";
            this.tabPage6.UseVisualStyleBackColor = true;
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
            // LabelChieftainLevel
            // 
            this.LabelChieftainLevel.AutoSize = true;
            this.LabelChieftainLevel.Location = new System.Drawing.Point(8, 24);
            this.LabelChieftainLevel.Name = "LabelChieftainLevel";
            this.LabelChieftainLevel.Size = new System.Drawing.Size(54, 13);
            this.LabelChieftainLevel.TabIndex = 0;
            this.LabelChieftainLevel.Text = "Уровень:";
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
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPageExternal;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPageChieftain;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button ButtonEndTurn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripTextBox tstbTurn;
        private System.Windows.Forms.Label labelChieftainExp;
        private System.Windows.Forms.Label LabelChieftainLevel;
    }
}

