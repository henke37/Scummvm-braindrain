using System;
using System.Windows.Forms;

namespace ScummResearchForm {
	public partial class AddIgnoreForm : Form {
		public AddIgnoreForm() {
			InitializeComponent();
			VarNameTxt.Focus();
		}

		private void AddButton_Click(object sender, EventArgs e) {
			string varName = VarNameTxt.Text;
			ResearchForm.AddIgnore(varName);
			Close();
		}

		private ScummResearchForm ResearchForm => (ScummResearchForm)Owner;

		private void VarNameTxt_KeyDown(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Enter:
					e.Handled = true;
					AddButton.PerformClick();
					break;
				case Keys.Escape:
					e.Handled = true;
					Close();
					break;
			}
		}
	}
}
