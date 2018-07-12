using System;
using System.Windows.Forms;

namespace Orange.IDE
{
	public partial class Ask : Form
	{
		public Ask()
		{
			InitializeComponent();
		}

		public string Prompt
		{
			get;
			set;
		}

		public string RequestedText
		{
			get;
			set;
		}

		void Ask_Load(object sender, EventArgs e)
		{
			labelPrompt.Text = Prompt;
		}

		void buttonOK_Click(object sender, EventArgs e)
		{
			RequestedText = textText.Text;
			Close();
		}

		void buttonCancel_Click(object sender, EventArgs e)
		{
			RequestedText = "";
			Close();
		}
	}
}
