namespace TestProj {
	partial class TestForm {
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
			this.components = new System.ComponentModel.Container();
			this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.statusTxt = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// UpdateTimer
			// 
			this.UpdateTimer.Enabled = true;
			this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
			// 
			// statusTxt
			// 
			this.statusTxt.AutoSize = true;
			this.statusTxt.Location = new System.Drawing.Point(13, 13);
			this.statusTxt.Name = "statusTxt";
			this.statusTxt.Size = new System.Drawing.Size(35, 13);
			this.statusTxt.TabIndex = 0;
			this.statusTxt.Text = "label1";
			// 
			// TestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(206, 152);
			this.Controls.Add(this.statusTxt);
			this.Name = "TestForm";
			this.Text = "TestForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer UpdateTimer;
		private System.Windows.Forms.Label statusTxt;
	}
}

