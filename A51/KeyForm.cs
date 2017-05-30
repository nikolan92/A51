using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A51
{
    public partial class KeyForm : Form
    {
        
        public KeyForm()
        {
            InitializeComponent();
            textBoxKey.Text = "1111000010001000001000110110100100010100101000010000010010001001";//some default key
        }
        public KeyForm(string fileName)
        {
            InitializeComponent();
            fileNameLbl.Text = fileName;
        }
        public string Key
        {
            get { return textBoxKey.Text; }
        }
        private void textBoxKey_TextChanged(object sender, EventArgs e)
        {
            if (textBoxKey.Text.All(chr => chr.Equals('0') || chr.Equals('1')) && !string.IsNullOrEmpty(textBoxKey.Text) && textBoxKey.Text.Length == 64)
            {
                keyStatusLbl.ForeColor = Color.Green;
                keyStatusLbl.Text = "Valid.";
                encryptBtn.Enabled = true;
            }
            else
            {
                keyStatusLbl.ForeColor = Color.Red;
                keyStatusLbl.Text = "Invalid!";
                encryptBtn.Enabled = false;
            }
        }

        private void KeyForm_Load(object sender, EventArgs e)
        {
            textBoxKey.Text = "1111000010001000001000110110100100010100101000010000010010001001";//some default key
        }
    }
}
