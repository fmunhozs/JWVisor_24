using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JWSongs
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }


        private void Form2_Load(object sender, EventArgs e)
        {
            Cursor.Hide();

            axWindowsMediaPlayer1.Left = 0;
            axWindowsMediaPlayer1.Top = 0;

            axWindowsMediaPlayer1.Width = this.Width;
            axWindowsMediaPlayer1.Height = this.Height;

            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.stretchToFit = true;
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 3)
            {
                JWSong.Form1._PRVW = "1"; 
            }

            else if (e.newState == 8)
            {
                JWSong.Form1._PRVW = "z";

                this.Hide();
            }
        }

        private void Form2_Activated(object sender, EventArgs e)
        {
            //Cursor.Hide();
            //this.Refresh();
        }
    }
}
