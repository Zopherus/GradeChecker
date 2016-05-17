using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeChecking
{
    public partial class ResultsForm : Form
    {
        public ResultsForm(string result)
        {
            InitializeComponent();
            labelResults.Text = result;
        }
    }
}
