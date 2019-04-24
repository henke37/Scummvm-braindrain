namespace ScummResearchForm {
	partial class AddIgnoreForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.AddButton = new System.Windows.Forms.Button();
			this.VarNameTxt = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// AddButton
			// 
			this.AddButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.AddButton.Location = new System.Drawing.Point(12, 39);
			this.AddButton.Name = "AddButton";
			this.AddButton.Size = new System.Drawing.Size(75, 23);
			this.AddButton.TabIndex = 0;
			this.AddButton.Text = "Add";
			this.AddButton.UseVisualStyleBackColor = true;
			this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
			// 
			// VarNameTxt
			// 
			this.VarNameTxt.Location = new System.Drawing.Point(13, 13);
			this.VarNameTxt.Name = "VarNameTxt";
			this.VarNameTxt.Size = new System.Drawing.Size(128, 20);
			this.VarNameTxt.TabIndex = 1;
			this.VarNameTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VarNameTxt_KeyDown);
			// 
			// AddIgnoreForm
			// 
			this.AcceptButton = this.AddButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(156, 73);
			this.Controls.Add(this.VarNameTxt);
			this.Controls.Add(this.AddButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddIgnoreForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "AddIgnoreForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button AddButton;
		private System.Windows.Forms.TextBox VarNameTxt;
	}
}