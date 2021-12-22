
namespace LocalAppNew
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e)
        {
            exitGame();
        }


        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.updateDelayText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // updateDelayText
            // 
            this.updateDelayText.AutoSize = true;
            this.updateDelayText.BackColor = System.Drawing.Color.Transparent;
            this.updateDelayText.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.updateDelayText.ForeColor = System.Drawing.Color.Red;
            this.updateDelayText.Location = new System.Drawing.Point(14, 12);
            this.updateDelayText.Name = "updateDelayText";
            this.updateDelayText.Size = new System.Drawing.Size(48, 23);
            this.updateDelayText.TabIndex = 3;
            this.updateDelayText.Text = "0 ms";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(777, 779);
            this.Controls.Add(this.updateDelayText);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Practice";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label updateDelayText;
    }
}

