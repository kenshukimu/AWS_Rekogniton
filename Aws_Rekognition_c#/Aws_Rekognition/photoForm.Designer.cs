namespace Aws_Rekognition
{
    partial class photoForm
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
            this.pictureBox01 = new System.Windows.Forms.PictureBox();
            this.button5 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox01)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox01
            // 
            this.pictureBox01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox01.Location = new System.Drawing.Point(12, 23);
            this.pictureBox01.Name = "pictureBox01";
            this.pictureBox01.Size = new System.Drawing.Size(342, 311);
            this.pictureBox01.TabIndex = 1;
            this.pictureBox01.TabStop = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(360, 23);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(200, 311);
            this.button5.TabIndex = 10;
            this.button5.Text = "Snapshot";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // photoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 342);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.pictureBox01);
            this.Name = "photoForm";
            this.Text = "photoForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.photoForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox01)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox01;
        private System.Windows.Forms.Button button5;
    }
}