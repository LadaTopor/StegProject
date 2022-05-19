namespace StegProject
{
    partial class Main
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
            this.Enrypt = new System.Windows.Forms.Button();
            this.pbField = new System.Windows.Forms.PictureBox();
            this.Decrypt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbField)).BeginInit();
            this.SuspendLayout();
            // 
            // Enrypt
            // 
            this.Enrypt.Location = new System.Drawing.Point(12, 12);
            this.Enrypt.Name = "Enrypt";
            this.Enrypt.Size = new System.Drawing.Size(92, 23);
            this.Enrypt.TabIndex = 0;
            this.Enrypt.Text = "Зашифровать";
            this.Enrypt.UseVisualStyleBackColor = true;
            this.Enrypt.Click += new System.EventHandler(this.Enrypt_Click);
            // 
            // pbField
            // 
            this.pbField.Location = new System.Drawing.Point(0, 41);
            this.pbField.Name = "pbField";
            this.pbField.Size = new System.Drawing.Size(669, 352);
            this.pbField.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbField.TabIndex = 1;
            this.pbField.TabStop = false;
            // 
            // Decrypt
            // 
            this.Decrypt.Location = new System.Drawing.Point(145, 12);
            this.Decrypt.Name = "Decrypt";
            this.Decrypt.Size = new System.Drawing.Size(109, 23);
            this.Decrypt.TabIndex = 2;
            this.Decrypt.Text = "Расшифровать";
            this.Decrypt.UseVisualStyleBackColor = true;
            this.Decrypt.Click += new System.EventHandler(this.Decrypt_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 405);
            this.Controls.Add(this.Decrypt);
            this.Controls.Add(this.pbField);
            this.Controls.Add(this.Enrypt);
            this.Name = "Main";
            this.Text = "Стеганография";
            ((System.ComponentModel.ISupportInitialize)(this.pbField)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Enrypt;
        private System.Windows.Forms.PictureBox pbField;
        private System.Windows.Forms.Button Decrypt;
    }
}

