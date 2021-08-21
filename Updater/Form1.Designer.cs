
namespace Updater
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelActualVersion = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labelAction = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(208, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Проверить на наличие новой версии";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(8, 8);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(91, 13);
            this.labelVersion.TabIndex = 1;
            this.labelVersion.Text = "Текущая версия";
            // 
            // labelActualVersion
            // 
            this.labelActualVersion.AutoSize = true;
            this.labelActualVersion.Location = new System.Drawing.Point(8, 64);
            this.labelActualVersion.Name = "labelActualVersion";
            this.labelActualVersion.Size = new System.Drawing.Size(112, 13);
            this.labelActualVersion.TabIndex = 2;
            this.labelActualVersion.Text = "История изменений:";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(8, 80);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(607, 246);
            this.textBox1.TabIndex = 3;
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Location = new System.Drawing.Point(224, 28);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(57, 13);
            this.labelAction.TabIndex = 4;
            this.labelAction.Text = "Действие";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(5, 334);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(595, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Легенда: [!] важно; [+] доработка; [*] изменение; [-] убрано; [F] исправлена ошиб" +
    "ка.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 357);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelAction);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.labelActualVersion);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updater The Fantasy Kingdoms Battle";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelActualVersion;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.Label label1;
    }
}

