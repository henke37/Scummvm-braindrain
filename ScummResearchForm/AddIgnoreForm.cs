using System;
using System.Windows.Forms;

namespace ScummResearchForm {
	public partial class AddIgnoreForm : Form {
		public AddIgnoreForm() {
			InitializeComponent();
		}

		private void AddButton_Click(object sender, EventArgs e) {
			string varName = VarNameTxt.Text;
			ResearchForm.AddIgnore(varName);
			Close();
		}

		private ScummResearchForm ResearchForm => (ScummResearchForm)Owner;
	}
}
