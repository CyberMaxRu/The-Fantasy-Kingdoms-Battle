namespace Fantasy_King_s_Battle
{
    partial class FormBattle
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
            this.button1 = new System.Windows.Forms.Button();
            this.lblStep = new System.Windows.Forms.Label();
            this.lblTotalSteps = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblDrawBack = new System.Windows.Forms.Label();
            this.lblDrawHeroes = new System.Windows.Forms.Label();
            this.lblSkippedFrames = new System.Windows.Forms.Label();
            this.lblCalcStep = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(880, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Шаг";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblStep
            // 
            this.lblStep.AutoSize = true;
            this.lblStep.Location = new System.Drawing.Point(8, 8);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(35, 13);
            this.lblStep.TabIndex = 1;
            this.lblStep.Text = "label1";
            // 
            // lblTotalSteps
            // 
            this.lblTotalSteps.AutoSize = true;
            this.lblTotalSteps.Location = new System.Drawing.Point(176, 8);
            this.lblTotalSteps.Name = "lblTotalSteps";
            this.lblTotalSteps.Size = new System.Drawing.Size(35, 13);
            this.lblTotalSteps.TabIndex = 2;
            this.lblTotalSteps.Text = "label1";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(336, 8);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(35, 13);
            this.lblSpeed.TabIndex = 3;
            this.lblSpeed.Text = "label1";
            // 
            // lblDrawBack
            // 
            this.lblDrawBack.AutoSize = true;
            this.lblDrawBack.Location = new System.Drawing.Point(432, 8);
            this.lblDrawBack.Name = "lblDrawBack";
            this.lblDrawBack.Size = new System.Drawing.Size(35, 13);
            this.lblDrawBack.TabIndex = 4;
            this.lblDrawBack.Text = "label1";
            // 
            // lblDrawHeroes
            // 
            this.lblDrawHeroes.AutoSize = true;
            this.lblDrawHeroes.Location = new System.Drawing.Point(528, 8);
            this.lblDrawHeroes.Name = "lblDrawHeroes";
            this.lblDrawHeroes.Size = new System.Drawing.Size(35, 13);
            this.lblDrawHeroes.TabIndex = 5;
            this.lblDrawHeroes.Text = "label2";
            // 
            // lblSkippedFrames
            // 
            this.lblSkippedFrames.AutoSize = true;
            this.lblSkippedFrames.Location = new System.Drawing.Point(640, 8);
            this.lblSkippedFrames.Name = "lblSkippedFrames";
            this.lblSkippedFrames.Size = new System.Drawing.Size(35, 13);
            this.lblSkippedFrames.TabIndex = 6;
            this.lblSkippedFrames.Text = "label1";
            // 
            // lblCalcStep
            // 
            this.lblCalcStep.AutoSize = true;
            this.lblCalcStep.Location = new System.Drawing.Point(760, 8);
            this.lblCalcStep.Name = "lblCalcStep";
            this.lblCalcStep.Size = new System.Drawing.Size(35, 13);
            this.lblCalcStep.TabIndex = 7;
            this.lblCalcStep.Text = "label1";
            // 
            // FormBattle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 637);
            this.Controls.Add(this.lblCalcStep);
            this.Controls.Add(this.lblSkippedFrames);
            this.Controls.Add(this.lblDrawHeroes);
            this.Controls.Add(this.lblDrawBack);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.lblTotalSteps);
            this.Controls.Add(this.lblStep);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.Name = "FormBattle";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormBattle";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.Label lblTotalSteps;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblDrawBack;
        private System.Windows.Forms.Label lblDrawHeroes;
        private System.Windows.Forms.Label lblSkippedFrames;
        private System.Windows.Forms.Label lblCalcStep;
    }
}