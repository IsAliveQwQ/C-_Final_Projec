namespace WinFormsApp1
{
    partial class EditComicForm
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
            this.lblISBN = new System.Windows.Forms.Label();
            this.txtISBN = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.lblPublisher = new System.Windows.Forms.Label();
            this.txtPublisher = new System.Windows.Forms.TextBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.txtCategory = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbComicImage = new System.Windows.Forms.PictureBox();
            this.btnSelectImage = new System.Windows.Forms.Button();
            this.lblOfferDate = new System.Windows.Forms.Label();
            this.txtOfferDate = new System.Windows.Forms.MaskedTextBox();
            this.lblPages = new System.Windows.Forms.Label();
            this.txtPages = new System.Windows.Forms.TextBox();
            this.lblBookSummary = new System.Windows.Forms.Label();
            this.txtBookSummary = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbComicImage)).BeginInit();
            this.SuspendLayout();
            // 
            // lblISBN
            // 
            this.lblISBN.AutoSize = true;
            this.lblISBN.Location = new System.Drawing.Point(300, 20);
            this.lblISBN.Name = "lblISBN";
            this.lblISBN.Size = new System.Drawing.Size(35, 20);
            this.lblISBN.TabIndex = 0;
            this.lblISBN.Text = "ISBN";
            // 
            // txtISBN
            // 
            this.txtISBN.Location = new System.Drawing.Point(360, 17);
            this.txtISBN.Name = "txtISBN";
            this.txtISBN.Size = new System.Drawing.Size(200, 27);
            this.txtISBN.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(300, 60);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(35, 20);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "書名";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(360, 57);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(200, 27);
            this.txtTitle.TabIndex = 3;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(300, 100);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(35, 20);
            this.lblAuthor.TabIndex = 4;
            this.lblAuthor.Text = "作者";
            // 
            // txtAuthor
            // 
            this.txtAuthor.Location = new System.Drawing.Point(360, 97);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(200, 27);
            this.txtAuthor.TabIndex = 5;
            // 
            // lblPublisher
            // 
            this.lblPublisher.AutoSize = true;
            this.lblPublisher.Location = new System.Drawing.Point(300, 140);
            this.lblPublisher.Name = "lblPublisher";
            this.lblPublisher.Size = new System.Drawing.Size(35, 20);
            this.lblPublisher.TabIndex = 6;
            this.lblPublisher.Text = "出版社";
            // 
            // txtPublisher
            // 
            this.txtPublisher.Location = new System.Drawing.Point(360, 137);
            this.txtPublisher.Name = "txtPublisher";
            this.txtPublisher.Size = new System.Drawing.Size(200, 27);
            this.txtPublisher.TabIndex = 7;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(300, 180);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(35, 20);
            this.lblCategory.TabIndex = 8;
            this.lblCategory.Text = "分類";
            // 
            // txtCategory
            // 
            this.txtCategory.Location = new System.Drawing.Point(360, 177);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.Size = new System.Drawing.Size(200, 27);
            this.txtCategory.TabIndex = 9;
            // 
            // lblOfferDate
            // 
            this.lblOfferDate.AutoSize = true;
            this.lblOfferDate.Location = new System.Drawing.Point(300, 220);
            this.lblOfferDate.Name = "lblOfferDate";
            this.lblOfferDate.Size = new System.Drawing.Size(51, 20);
            this.lblOfferDate.TabIndex = 14;
            this.lblOfferDate.Text = "發售日";
            // 
            // txtOfferDate
            // 
            this.txtOfferDate.Location = new System.Drawing.Point(360, 217);
            this.txtOfferDate.Mask = "0000/00/00";
            this.txtOfferDate.Name = "txtOfferDate";
            this.txtOfferDate.Size = new System.Drawing.Size(200, 30);
            this.txtOfferDate.TabIndex = 15;
            this.txtOfferDate.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lblPages
            // 
            this.lblPages.AutoSize = true;
            this.lblPages.Location = new System.Drawing.Point(300, 260);
            this.lblPages.Name = "lblPages";
            this.lblPages.Size = new System.Drawing.Size(35, 20);
            this.lblPages.TabIndex = 16;
            this.lblPages.Text = "頁數";
            // 
            // txtPages
            // 
            this.txtPages.Location = new System.Drawing.Point(360, 257);
            this.txtPages.Name = "txtPages";
            this.txtPages.Size = new System.Drawing.Size(200, 27);
            this.txtPages.TabIndex = 17;
            // 
            // lblBookSummary
            // 
            this.lblBookSummary.AutoSize = true;
            this.lblBookSummary.Location = new System.Drawing.Point(300, 300);
            this.lblBookSummary.Name = "lblBookSummary";
            this.lblBookSummary.Size = new System.Drawing.Size(35, 20);
            this.lblBookSummary.TabIndex = 18;
            this.lblBookSummary.Text = "摘要";
            // 
            // txtBookSummary
            // 
            this.txtBookSummary.Location = new System.Drawing.Point(360, 297);
            this.txtBookSummary.Multiline = true;
            this.txtBookSummary.Name = "txtBookSummary";
            this.txtBookSummary.Size = new System.Drawing.Size(200, 100);
            this.txtBookSummary.TabIndex = 19;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(360, 410);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 35);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "儲存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(470, 410);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pbComicImage
            // 
            this.pbComicImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbComicImage.Location = new System.Drawing.Point(20, 20);
            this.pbComicImage.Name = "pbComicImage";
            this.pbComicImage.Size = new System.Drawing.Size(250, 369);
            this.pbComicImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbComicImage.TabIndex = 12;
            this.pbComicImage.TabStop = false;
            // 
            // btnSelectImage
            // 
            this.btnSelectImage.Location = new System.Drawing.Point(80, 400);
            this.btnSelectImage.Name = "btnSelectImage";
            this.btnSelectImage.Size = new System.Drawing.Size(120, 30);
            this.btnSelectImage.TabIndex = 13;
            this.btnSelectImage.Text = "選擇圖片";
            this.btnSelectImage.UseVisualStyleBackColor = true;
            this.btnSelectImage.Click += new System.EventHandler(this.BtnSelectImage_Click);
            // 
            // EditComicForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 550);
            this.Controls.Add(this.txtBookSummary);
            this.Controls.Add(this.lblBookSummary);
            this.Controls.Add(this.txtPages);
            this.Controls.Add(this.lblPages);
            this.Controls.Add(this.txtOfferDate);
            this.Controls.Add(this.lblOfferDate);
            this.Controls.Add(this.btnSelectImage);
            this.Controls.Add(this.pbComicImage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtCategory);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.txtPublisher);
            this.Controls.Add(this.lblPublisher);
            this.Controls.Add(this.txtAuthor);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtISBN);
            this.Controls.Add(this.lblISBN);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditComicForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "編輯漫畫";
            this.Load += new System.EventHandler(this.EditComicForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbComicImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblISBN;
        private System.Windows.Forms.TextBox txtISBN;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.Label lblPublisher;
        private System.Windows.Forms.TextBox txtPublisher;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pbComicImage;
        private System.Windows.Forms.Button btnSelectImage;
        private System.Windows.Forms.Label lblOfferDate;
        private System.Windows.Forms.MaskedTextBox txtOfferDate;
        private System.Windows.Forms.Label lblPages;
        private System.Windows.Forms.TextBox txtPages;
        private System.Windows.Forms.Label lblBookSummary;
        private System.Windows.Forms.TextBox txtBookSummary;
    }
} 