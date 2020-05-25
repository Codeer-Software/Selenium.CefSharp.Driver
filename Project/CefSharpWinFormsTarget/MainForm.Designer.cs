namespace CefSharpWinFormsTarget
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this._panel = new System.Windows.Forms.Panel();
            this._buttonURL = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _panel
            // 
            this._panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panel.Location = new System.Drawing.Point(3, 2);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(796, 388);
            this._panel.TabIndex = 0;
            // 
            // _buttonURL
            // 
            this._buttonURL.Location = new System.Drawing.Point(13, 415);
            this._buttonURL.Name = "_buttonURL";
            this._buttonURL.Size = new System.Drawing.Size(75, 23);
            this._buttonURL.TabIndex = 1;
            this._buttonURL.Text = "URL";
            this._buttonURL.UseVisualStyleBackColor = true;
            this._buttonURL.Click += new System.EventHandler(this._buttonURL_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this._buttonURL);
            this.Controls.Add(this._panel);
            this.Name = "MainForm";
            this.Text = "MainWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _panel;
        private System.Windows.Forms.Button _buttonURL;
    }
}

