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
			System.Windows.Forms.SplitContainer splitContainer1;
			System.Windows.Forms.ToolStrip toolStrip1;
			System.Windows.Forms.ToolStripButton AddIgnoreBtn;
			this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.outputBox = new System.Windows.Forms.TextBox();
			this.IgnoreListBox = new System.Windows.Forms.CheckedListBox();
			this.SaveButton = new System.Windows.Forms.ToolStripButton();
			this.LoadButton = new System.Windows.Forms.ToolStripButton();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			AddIgnoreBtn = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			toolStrip1.SuspendLayout();
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
			this.outputBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputBox.Enabled = false;
			this.outputBox.Location = new System.Drawing.Point(0, 0);
			this.outputBox.Multiline = true;
			this.outputBox.Name = "outputBox";
			this.outputBox.ReadOnly = true;
			this.outputBox.ShortcutsEnabled = false;
			this.outputBox.Size = new System.Drawing.Size(431, 450);
			this.outputBox.TabIndex = 0;
			this.outputBox.TabStop = false;
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.Location = new System.Drawing.Point(0, 0);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(this.outputBox);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(this.IgnoreListBox);
			splitContainer1.Panel2.Controls.Add(toolStrip1);
			splitContainer1.Size = new System.Drawing.Size(800, 450);
			splitContainer1.SplitterDistance = 431;
			splitContainer1.TabIndex = 1;
			// 
			// toolStrip1
			// 
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            AddIgnoreBtn,
            this.SaveButton,
            this.LoadButton});
			toolStrip1.Location = new System.Drawing.Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new System.Drawing.Size(365, 25);
			toolStrip1.TabIndex = 0;
			toolStrip1.Text = "toolStrip1";
			// 
			// IgnoreListBox
			// 
			this.IgnoreListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.IgnoreListBox.FormattingEnabled = true;
			this.IgnoreListBox.Location = new System.Drawing.Point(0, 25);
			this.IgnoreListBox.Name = "IgnoreListBox";
			this.IgnoreListBox.Size = new System.Drawing.Size(365, 425);
			this.IgnoreListBox.TabIndex = 1;
			// 
			// AddIgnoreBtn
			// 
			AddIgnoreBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			AddIgnoreBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			AddIgnoreBtn.Name = "AddIgnoreBtn";
			AddIgnoreBtn.Size = new System.Drawing.Size(33, 22);
			AddIgnoreBtn.Text = "Add";
			AddIgnoreBtn.Click += new System.EventHandler(this.AddIgnore_Click);
			// 
			// SaveButton
			// 
			this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(35, 22);
			this.SaveButton.Text = "Save";
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// LoadButton
			// 
			this.LoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.LoadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(37, 22);
			this.LoadButton.Text = "Load";
			this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "txt";
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "txt";
			// 
			// ScummResearchForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(splitContainer1);
			this.Name = "ScummResearchForm";
			this.Text = "Scumm Research form";
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
			splitContainer1.ResumeLayout(false);
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer UpdateTimer;
		private System.Windows.Forms.TextBox outputBox;
		private System.Windows.Forms.CheckedListBox IgnoreListBox;
		private System.Windows.Forms.ToolStripButton SaveButton;
		private System.Windows.Forms.ToolStripButton LoadButton;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
	}
}

