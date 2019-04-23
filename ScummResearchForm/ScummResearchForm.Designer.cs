namespace ScummResearchForm {
	partial class ScummResearchForm {
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
			this.outputBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// UpdateTimer
			// 
			this.UpdateTimer.Enabled = true;
			this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
			// 
			// outputBox
			// 
			this.outputBox.CausesValidation = false;
			this.outputBox.Enabled = false;
			this.outputBox.Location = new System.Drawing.Point(13, 13);
			this.outputBox.Multiline = true;
			this.outputBox.Name = "outputBox";
			this.outputBox.ReadOnly = true;
			this.outputBox.ShortcutsEnabled = false;
			this.outputBox.Size = new System.Drawing.Size(549, 403);
			this.outputBox.TabIndex = 0;
			this.outputBox.TabStop = false;
			// 
			// ScummResearchForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.outputBox);
			this.Name = "ScummResearchForm";
			this.Text = "Scumm Research form";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer UpdateTimer;
		private System.Windows.Forms.TextBox outputBox;
	}
}

