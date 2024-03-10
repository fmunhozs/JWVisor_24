using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Id3;
using Id3.Info;

namespace JWSong
{
    public partial class Form1 : Form
    {
        public Boolean bPreludio;
        public static string Estado;
        string NomeDoArquivo;
        public static string _PRVW;
        public static string _ABA;

        readonly string ptMD = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName + @"\Roaming\Visor";

        ListViewItem itm;
        public JWSongs.Form2 dlg;
        Mp3File m;

        int[] num;

        
        public Form1()
        {
            
            num = new int[2];

            num[1] = DriveInfo.GetDrives().Count();

            InitializeComponent();
            PopulateTreeView();

        }

        private void PopulateTreeView()
        {
            TreeNode rootNode;

            // DIRETÓRIO PRINCIPAL
            DirectoryInfo info = new DirectoryInfo(@"C:\Users\Wilson\Videos\JWLibrary");
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                // rootNode.ImageIndex = 0;
                GetDirectories(info.GetDirectories(), rootNode);
                trvPastas.Nodes.Add(rootNode);
            }

            // DIRETÓRIO JW LIBRARY (DEFAULT)
            //DirectoryInfo info2 = new DirectoryInfo(ptMD.Replace("\\\\","\\"));

            //if ((info2.Exists) && (info2.FullName !="Visor"))

            //{
            //    rootNode = new TreeNode(info2.Name);
            //    rootNode.Tag = info2;
            //    // rootNode.ImageIndex = 0;
            //    GetDirectories(info2.GetDirectories(), rootNode);
            //    trvPastas.Nodes.Add(rootNode);
            //}


            // UNIDADES REMOVÍVEIS
            DirectoryInfo info3;

            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.DriveType == DriveType.Removable)
                    {
                        info3 = new DirectoryInfo(drive.Name);

                        rootNode = new TreeNode(drive.Name);
                        rootNode.Tag = drive;
                        GetDirectories(info3.GetDirectories(), rootNode);
                        trvPastas.Nodes.Add(rootNode);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao tentar abrir unidade removível.", "JW Visor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;

            foreach (DirectoryInfo subDir in subDirs)
            {
                try
                {
                    aNode = new TreeNode(subDir.Name, 0, 0);
                    aNode.Tag = subDir;
                    aNode.ImageKey = "folder";
                    subSubDirs = subDir.GetDirectories();
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (Exception)
                {
                    // MessageBox.Show("Erro ao tentar abrir diretório.", "JW Visor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        int iTela = 0;
        
        private void ChecaResolucao()
        {
            Rectangle bounds;
            
            if (dlg == null)           
            {
                dlg = new JWSongs.Form2();
            }

            Screen[] screens = Screen.AllScreens;

            iTela = 0;

            if (screens.Length > 1)
            {
                iTela = 1;

                bounds = screens[iTela].Bounds;
                dlg.SetBounds(screens[0].Bounds.Width, 0, bounds.Width, bounds.Height);
            }
            else
            {
                iTela = 0;
                bounds = screens[iTela].Bounds;

                dlg.SetBounds(0, 0, bounds.Width, bounds.Height);
            }

            dlg.pictureBox1.Width = bounds.Width;
            dlg.pictureBox1.Height = bounds.Height;




            toolStripStatusLabel2.Text = "Resolução da tela " + (iTela + 1).ToString() + ":"; 
            toolStripStatusLabel3.Text = bounds.Width.ToString() + "x" + bounds.Height.ToString();

            if ((bounds.Width == 1280) && (bounds.Height == 720))
            {
                toolStripStatusLabel2.Image = imageList1.Images[4];
                toolStripStatusLabel4.Text = "OK";
            }
            else
            {
                toolStripStatusLabel2.Image = imageList1.Images[3];
                toolStripStatusLabel4.Text = "Ajuste a resolução da tela 2 para 1280x720.";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Visor " + Application.ProductVersion;

            //ChecaResolucao();

            trackBar1.Value = Convert.ToInt32(lblVol.Text);            

            // Cânticos ÁUDIO
            bPreludio = false;

            axwmpVideo.settings.volume = 0;
            axwmpVideo.Ctlenabled = false;

            axwmpPreview.settings.volume = 0;

            String[] sMusicas; 

            Int16 x = 0;
            bPreludio = false;

            //  DIRETÓRIO DOS CÂNTICOS
            sMusicas = System.IO.Directory.GetFiles("C:\\JWCanticos", "*.mp4");

            x = 0;

            foreach (string s in sMusicas)
            {
                x++;
                itm = lsvVideo.Items.Add(s);
                itm.SubItems.Add(MP4Title(s));
            }

            m = null;
            itm = null;
             
            lsvCant.Refresh();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(ptMD + @"\config.xml");

            XmlNodeList parentNode = xmlDoc.GetElementsByTagName("config");

            foreach (XmlNode childrenNode in parentNode)
            {
                chkZoom.Checked = Convert.ToBoolean(Convert.ToInt32(childrenNode.SelectSingleNode("zoom").InnerText));
                chkFade.Checked = Convert.ToBoolean(Convert.ToInt32(childrenNode.SelectSingleNode("fade").InnerText));
            }

            GC.Collect();
        }

        private void CarregaPreludio()
        {
            String[] sPre = System.IO.Directory.GetFiles("C:\\JWPreludio", "*.mp3");
            foreach (string p in sPre)
            {
                m = new Mp3File(p, 0);

                itm = lsvPreludio.Items.Add(p);

                if(m.HasTags)
                {
                    itm.SubItems.Add(m.GetAllTags()[0].Title);
                }
                else
                {
                    itm.SubItems.Add(p);
                }
                
            }
        }

        private void txtCant1_Click(object sender, EventArgs e)
        {
            txtCant1.BackColor = Color.LimeGreen;
            txtCant2.BackColor = Color.White;
            txtCant3.BackColor = Color.White;

            lblPos.Text = "1";

            Display(txtCant1);

        }

        private void txtCant2_Click(object sender, EventArgs e)
        {
            txtCant1.BackColor = Color.White;
            txtCant2.BackColor = Color.LimeGreen;
            txtCant3.BackColor = Color.White;

            lblPos.Text = "2";

            Display(txtCant2);
        }

        private void txtCant3_Click(object sender, EventArgs e)
        {
            txtCant1.BackColor = Color.White;
            txtCant2.BackColor = Color.White;
            txtCant3.BackColor = Color.LimeGreen;

            lblPos.Text = "3";

            Display(txtCant3);
        }

        private void btnBS_Click(object sender, EventArgs e)
        {
            Teclado("-");
        }

        void Teclado(String tecla)
        {

            string nome = "txtCant";
            string par = "btParar";
            string toc = "btTocar";

            for (int i = 1; i <= 3; i++)
            {
                Control[] txt = Controls.Find(nome + i.ToString(), true);
                TextBox txt2 = (TextBox)txt[0];

                Control[] btP = Controls.Find(par + i.ToString(), true);
                Button btP2 = (Button)btP[0];

                Control[] btT = Controls.Find(toc + i.ToString(), true);
                Button btT2 = (Button)btT[0];

                if (txt2.BackColor == Color.LimeGreen)
                {
                    if (tecla == "-")
                    {
                        if (txt2.Text.Length > 0)
                        {
                            txt2.Text = txt2.Text.Remove(txt2.Text.Length - 1);
                            btP2.Enabled = true;
                            btT2.Enabled = true;
                        }
                    }
                    else
                    {
                        if ((txt2.Text.Length >= 0) && (txt2.Text.Length <= 2))
                        {
                            txt2.Text = txt2.Text + tecla;
                            btP2.Enabled = true;
                            btT2.Enabled = true;
                        }
                    }

                    Display(txt2);

                }
            }

        }

        private void Display(TextBox _tx)
        {
            try
            {
                lblPos.Text = _tx.Name.Substring(7, 1);

                string NomeV = MP4Title(lsvVideo.Items[Convert.ToInt16(_tx.Text) - 1].Text).Split('.')[1].Trim();
                
                lblTitulo.Text = NomeV;
                lblCant.Text = _tx.Text;
            }
            catch (Exception)
            {
                lblTitulo.Text = "-";
                lblCant.Text = "-";
            }
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            Teclado("0");
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            Teclado("1");
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            Teclado("2");
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            Teclado("3");
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            Teclado("4");
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            Teclado("5");
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            Teclado("6");
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            Teclado("7");
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            Teclado("8");
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            Teclado("9");
        }

        private void btTocar1_Click(object sender, EventArgs e)
        {
            bPreludio = false;

            lblPlay.Text = "-1";

            Int16 outval;  
            if (Int16.TryParse(txtCant1.Text, out outval))
            {
                Tocar(Convert.ToInt16(txtCant1.Text), 1);
                lblPlay.Text = "1";
            }
            else
            {
                MessageBox.Show("Insira um valor válido para o cântico 1.", "JW Visor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtCant1.Text = String.Empty;
            }

        }

        private void btTocar2_Click(object sender, EventArgs e)
        {
            bPreludio = false;

            lblPlay.Text = "-1";

            Int16 outval;
            if (Int16.TryParse(txtCant2.Text, out outval))
            {
                Tocar(Convert.ToInt16(txtCant2.Text), 2);
                lblPlay.Text = "2";
            }
            else
            {
                MessageBox.Show("Insira um valor válido para o cântico 2.", "JW Visor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtCant2.Text = String.Empty;
            }
        }

        private void btTocar3_Click(object sender, EventArgs e)
        {
            bPreludio = false;

            lblPlay.Text = "-1";

            Int16 outval;
            if (Int16.TryParse(txtCant3.Text, out outval))
            {
                Tocar(Convert.ToInt16(txtCant3.Text), 3);
                lblPlay.Text = "3";
            }
            else
            {
                MessageBox.Show("Insira um valor válido para o cântico 3.", "JW Visor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtCant3.Text = String.Empty;
            }
        }
             

        void Tocar(int Cantico, int Caixa)
        {
            _ABA = tabControl1.SelectedTab.Text;
            timer1.Stop();

            lblMidia.Text = "C";

            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer2.Ctlcontrols.stop();
            axwmpVideo.Ctlcontrols.stop();

            ListView lsv;

            bPreludio = false;
            Boolean erro = false;

 

            if (chkVideo.Checked) { lsv = lsvVideo; } else { lsv = lsvCant; }

            for (int i = 0; i < lsv.Items.Count; i++)
            {
                if(lsv.Items[i].Text.IndexOf(String.Format("{0:000}", Cantico)) > 0)
                {
                    if (!chkVideo.Checked)
                    {
                        #region
                        string NomeC = lsv.Items[i].SubItems[1].Text.Split('_')[1].ToString();

                        lblSt.Text = "1";

                        lblPos.Text = Caixa.ToString();
                        lblCant.Text = Cantico.ToString();
                        lblTitulo.Text = NomeC;

                        axWindowsMediaPlayer1.URL = lsv.Items[i].Text;

                        erro = false;
                        break;
                        #endregion
                    }
                    else
                    {
                        string NomeV = lsv.Items[i].SubItems[1].Text.Split('.')[1].ToString().Trim();

                        dlg.axWindowsMediaPlayer1.settings.volume = trackBar1.Value;

                        timer1.Start();

                        _PRVW = "1";

                        lblPos.Text = Caixa.ToString();
                        lblCant.Text = Cantico.ToString();
                        lblTitulo.Text = NomeV; // "Vídeo";

                        dlg.Opacity = 1;

                        if (chkZoom.Checked) { AbrirZoom(1); }

                        dlg.pictureBox1.Visible = false;
                        dlg.axWindowsMediaPlayer1.Visible = true;

                        dlg.axWindowsMediaPlayer1.URL = lsv.Items[i].Text;
                        dlg.axWindowsMediaPlayer1.settings.rate = 1;

                        dlg.Show();

                        axwmpVideo.URL = lsv.Items[i].Text;

                        btTocar.Enabled = false;
                        btTocar.ImageIndex = 0;

                        btParar.Enabled = false;

                        erro = false;
                        return;
                    }
                }
                else
                {
                    Inicio();
                    erro = true;
                }
            }

            if (erro)
            {
                MessageBox.Show("Número para o cântico " + Caixa.ToString() + " inválido.", "JW Visor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btParar1_Click(object sender, EventArgs e)
        {
            if(lblMidia.Text == "C")
            {
                timer1.Stop();

                axwmpVideo.Ctlcontrols.stop();
                dlg.axWindowsMediaPlayer1.Ctlcontrols.stop();

                dlg.Hide();

                if (chkZoom.Checked) { AbrirZoom(0); }

                lblSt.Text = "3";

                if (lblPlay.Text == "1")
                {
                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                    lblSt.Text = "3";
                }

                btTocar.Enabled = true;
                btParar.Enabled = true;
            }

            GC.Collect();
        }

        private void btParar2_Click(object sender, EventArgs e)
        {
            if (lblMidia.Text == "C")
            {
                timer1.Stop();

                axwmpVideo.Ctlcontrols.stop();
                dlg.axWindowsMediaPlayer1.Ctlcontrols.stop();

                dlg.Hide();

                if (chkZoom.Checked) { AbrirZoom(0); }

                lblSt.Text = "3";

                if (lblPlay.Text == "2")
                {
                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                    lblSt.Text = "3";
                }

                btTocar.Enabled = true;
                btParar.Enabled = true;
            }

            GC.Collect();
        }

        private void btParar3_Click(object sender, EventArgs e)
        {
            if (lblMidia.Text == "C")
            {
                timer1.Stop();

                axwmpVideo.Ctlcontrols.stop();
                dlg.axWindowsMediaPlayer1.Ctlcontrols.stop();

                dlg.Hide();

                if (chkZoom.Checked) { AbrirZoom(0); }

                lblSt.Text = "3";

                if (lblPlay.Text == "3")
                {
                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                    lblSt.Text = "3";
                }

                btTocar.Enabled = true;
                btParar.Enabled = true;
            }

            GC.Collect();
        }

        private void Preludio(AxWMPLib.AxWindowsMediaPlayer player)
        {
            trackBar1.Value = 5;
            lblVol.Text = trackBar1.Value.ToString();

            axWindowsMediaPlayer1.settings.volume = trackBar1.Value;
            axWindowsMediaPlayer2.settings.volume = trackBar1.Value;

            timer1.Stop();
            dlg.axWindowsMediaPlayer1.Ctlcontrols.stop();

            if (bPreludio)
            {
                dlg.Hide();

                GC.Collect();
                
                if (lsvPreludio.Items.Count == 0) { CarregaPreludio(); }

                Random rndPre = new Random();
                int iPre = rndPre.Next(0, lsvPreludio.Items.Count);

                player.URL = lsvPreludio.Items[iPre].Text;

                lblSt.Text = "1";

                lblPos.Text = "P";

                lblCant.Text = "-";
            
                lsvPreludio.Items.Remove(lsvPreludio.Items[iPre]);

                lblTitulo.Text = "Prelúdio";
            }
        }

        private void btnPreludio_Click(object sender, EventArgs e)
        {
            trackBar1.Enabled = false;

            axwmpVideo.Ctlcontrols.stop();

            btParar.Enabled = true;
            btTocar.Enabled = true;

            bPreludio = true;
            
            Preludio(axWindowsMediaPlayer1);

            axWindowsMediaPlayer2.Ctlcontrols.stop();
        }


        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8) // FIM DA REPRODUÇÃO
            {
                if (bPreludio)
                {
                    Preludio(axWindowsMediaPlayer2);
                }
                else
                {
                    lblSt.Text = "z";
                }                
            }
        }

        private void axWindowsMediaPlayer2_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8) // FIM DA REPRODUÇÃO
            {
                Preludio(axWindowsMediaPlayer1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 30;
            lblVol.Text = trackBar1.Value.ToString();

            timer1.Stop();

            lblSt.Text = "-";
            lblPos.Text = "-";
            lblCant.Text = "-";
            lblTitulo.Text = "-";

            bPreludio = false;
            trackBar1.Enabled = true; 

            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer2.Ctlcontrols.stop();

            if (dlg ==  null)
            {
                axwmpVideo.Ctlcontrols.stop();

                lblSt.Text = "-";
                lblPos.Text = "-";
                lblCant.Text = "-";
                lblTitulo.Text = "-";
            }

            btParar.Enabled = true;
            btTocar.Enabled = true;
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Application.ProductVersion, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Inicio()
        {
            lblCant.Text = "-";
            lblPlay.Text = "-1";
            lblPos.Text = "-";
            lblSt.Text = "-";
            lblTitulo.Text = "-";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblSt.Text = Estado;

            if (Estado == "z")
            {
                btTocar.ImageIndex = 0;

                btTocar.Enabled = true;
                btParar.Enabled = true;
            }
        }


        private void trvPastas_NodeMouseClick_1(object sender, TreeNodeMouseClickEventArgs e)
        {
            axwmpPreview.Ctlcontrols.stop();

            TreeNode newSelected = e.Node;
            lsvArquivos.Items.Clear();

            tssMidia.Text = "< " + e.Node.FullPath.Replace("\\\\","\\")+ " >";

            if (e.Node.IsSelected)
            {
                try
                {
                    DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
                    ListViewItem.ListViewSubItem[] subItems;
                    ListViewItem item = null;

                    foreach (FileInfo file in nodeDirInfo.GetFiles())
                    {
                        if (file.Extension != ".txt")
                        {
                            item = new ListViewItem(file.Name, 1);
                            subItems = new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(item, "File"), new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString()) };

                            item.SubItems.AddRange(subItems);
                            lsvArquivos.Items.Add(item);
                        }
                    }
                }
                catch (Exception)
                {
                    DriveInfo nodeDirInfo = (DriveInfo)newSelected.Tag;
                    ListViewItem.ListViewSubItem[] subItems;
                    ListViewItem item = null;

                    foreach (FileInfo file in nodeDirInfo.RootDirectory.GetFiles())
                    {
                        if (file.Extension != ".txt")
                        {
                            item = new ListViewItem(file.Name, 1);
                            subItems = new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(item, "File"), new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString()) };

                            item.SubItems.AddRange(subItems);
                            lsvArquivos.Items.Add(item);
                        }
                    }
                }
            }

            lsvArquivos.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }


        private void lsvArquivos_Click(object sender, EventArgs e)
        {
            SelecionaArquivo();
        }

        private void SelecionaArquivo()
        {
            try
            {
                if (lsvArquivos.SelectedItems.Count == 1)
                {
                    if (
                            (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".png") > 0) ||
                            (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".jpeg") > 0) ||
                            (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".jpg") > 0) ||
                            (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".mp4") > 0) ||
                            (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".mp3") > 0) ||
                            (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".avi") > 0)
                       )

                        if (trvPastas.SelectedNode.FullPath.IndexOf("JWMidias") >= 0)
                        {
                            NomeDoArquivo = @"C:\" + trvPastas.SelectedNode.FullPath + @"\" + lsvArquivos.SelectedItems[0].Text;
                        }
                        else
                        {
                            if (trvPastas.SelectedNode.FullPath.IndexOf(":") >= 0)
                            {
                                NomeDoArquivo = trvPastas.SelectedNode.FullPath.Replace(@"\\", @"\") + @"\" + lsvArquivos.SelectedItems[0].Text ;
                            }
                            else
                            {
                                NomeDoArquivo = ptMD + @"\" + trvPastas.SelectedNode.FullPath + @"\" + lsvArquivos.SelectedItems[0].Text;
                            }
                                    
                        }

                    lblNomeArq.Text = NomeDoArquivo.Replace(@"Videos\Videos", @"Videos");
                    tssMidia.Text = lblNomeArq.Text;

                    axwmpPreview.settings.volume = 0;

                    axwmpPreview.URL = lblNomeArq.Text;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao tentar abrir arquivo.", "JW Visor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SegundaTela()
        {
            Screen[] screens = Screen.AllScreens;

            int iTela;
            iTela = 0;

            if (screens.Length > 1)
            {
                iTela = 1;
            }

            Rectangle bounds = screens[iTela].Bounds;
            dlg = new JWSongs.Form2();

            dlg.SetBounds(bounds.Width, 0, bounds.Width, bounds.Height);

            dlg.pictureBox1.Width = bounds.Width;
            dlg.pictureBox1.Height = bounds.Height;

            dlg.StartPosition = FormStartPosition.Manual;
            dlg.Show();

            if (
                    (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".jpg") > 0) ||
                    (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".jpeg") > 0) ||
                    (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".png") > 0)
               )
            {
                dlg.Opacity = 0;

                dlg.pictureBox1.Image = Image.FromFile(lblNomeArq.Text);
                dlg.axWindowsMediaPlayer1.Visible = false;

                dlg.Refresh();

                double i = 0.0;

                if (chkFade.Checked)
                {
                    while (i < 1)
                    {
                        i += 0.1;

                        dlg.Opacity = i;
                        System.Threading.Thread.Sleep(100);

                        dlg.Refresh();
                    }
                }
                else
                {
                    dlg.Opacity = 1;
                    dlg.Refresh(); 
                }
 
            }
            else
            {
                btTocar.ImageIndex = 1;

                dlg.pictureBox1.Visible = false;

                dlg.Width = bounds.Width;
                dlg.Height = bounds.Height;

                dlg.axWindowsMediaPlayer1.Width = dlg.Width;
                dlg.axWindowsMediaPlayer1.Height = dlg.Height;

                dlg.axWindowsMediaPlayer1.Visible = true;

                dlg.axWindowsMediaPlayer1.URL = lblNomeArq.Text;
            }
        }
        
        TimeSpan tsMidia;

        private void btTocar_Click_1(object sender, EventArgs e)
        {
            TocarVideo();
        }
        private void TocarVideo()
        {
            //ChecaResolucao();

            _ABA = tabControl1.SelectedTab.Text;

            if (dlg == null) { return; }

            lblMidia.Text = "V";

            GC.Collect();

            if (lsvArquivos.SelectedItems.Count != 1) { return; }

            if  (
                    (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".jpg") > 0) ||
                    (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".jpeg") > 0) ||
                    (lsvArquivos.SelectedItems[0].Text.ToLower().IndexOf(".png") > 0)
                )
            {
                lblStatusVideo.Text = "1";

                lblTCorrente.Text = "00:00:00";
                lblTTotal.Text = "00:00:00";

                double fade_out = 1;

                if (chkFade.Checked)
                {
                    do
                    {
                        dlg.Opacity = fade_out;
                        dlg.Refresh();

                        System.Threading.Thread.Sleep(20);

                        fade_out -= 0.1;
                    }
                    while (fade_out > 0);
                }

                btTocar.ImageIndex = 0;
                lblStatusVideo.Text = "1";

                try
                {
                    dlg.axWindowsMediaPlayer1.Ctlcontrols.stop();
                    dlg.axWindowsMediaPlayer1.Visible = false;

                    dlg.pictureBox1.Visible = true;
                    dlg.pictureBox1.Image = Image.FromFile(lblNomeArq.Text);

                    Bitmap bmpOrig = (Bitmap)Image.FromFile(lblNomeArq.Text);

                    dlg.Opacity = 0;

                    dOriginalH = bmpOrig.Height;
                    dOriginalW = bmpOrig.Width;

                    dZoom = 1;

                    dlg.pictureBox1.Size = bmpOrig.Size;

                    dlg.pictureBox1.Top = (dlg.Height / 2) - (bmpOrig.Height / 2);
                    dlg.pictureBox1.Left = (dlg.Width / 2) - (bmpOrig.Width / 2);

                    dlg.Show();

                    dlg.Refresh();

                    if (chkZoom.Checked) { AbrirZoom(1); } 
                }
                catch (Exception)
                {
                    lblStatusVideo.Text = "-";
                    MessageBox.Show("Falha ao abrir arquivo.", "JW Visor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (chkFade.Checked)
                {
                    double i = 0.0;

                    while (i < 1)
                    {
                        i += 0.1;

                        dlg.Opacity = i;
                        System.Threading.Thread.Sleep(100);
                    }
                }
                else
                {
                    dlg.Opacity = 1;
                }
            }
            else
            {
                if (btTocar.ImageIndex == 0) // TOCAR
                {
                    // TEMPO DE REPRODUÇÃO
                    tsMidia = new TimeSpan(0, 0, Convert.ToInt32(axwmpPreview.currentMedia.duration));

                    lblTCorrente.Text = "00:00:00";
                    lblTTotal.Text = tsMidia.Hours.ToString("00") + ":" + tsMidia.Minutes.ToString("00") + ":" + tsMidia.Seconds.ToString("00");

                    timer1.Start();

                    dlg.Opacity = 1;

                    if (lblNomeArq.Text != "")
                    {
                        dlg.pictureBox1.Visible = false;
                        dlg.axWindowsMediaPlayer1.Visible = true;
                        dlg.Refresh();

                        if (dlg.axWindowsMediaPlayer1.URL != lblNomeArq.Text)
                        {
                            dlg.axWindowsMediaPlayer1.URL = lblNomeArq.Text;
                            if (chkZoom.Checked) { AbrirZoom(1); }

                            _PRVW = "1";
                            lblStatusVideo.Text = _PRVW;
                        }
                        else
                        {
                            if ((lblStatusVideo.Text == "-") || (lblStatusVideo.Text == "3") || (lblStatusVideo.Text == "z"))
                            {
                                if (chkZoom.Checked) { AbrirZoom(1); }

                                TelaSecundaria();

                                dlg.axWindowsMediaPlayer1.Ctlcontrols.play();
                                return;
                            }

                            if (lblStatusVideo.Text == "2")
                            {
                                TelaSecundaria();
                                
                                dlg.axWindowsMediaPlayer1.Ctlcontrols.play();
                                return;
                            }
                        }
                    }

                    TelaSecundaria();

                }
                else if (btTocar.ImageIndex == 1) // PAUSAR
                {
                    _PRVW = "2";
                    lblStatusVideo.Text = _PRVW;

                    btTocar.ImageIndex = 0;

                    try
                    {
                        dlg.axWindowsMediaPlayer1.Ctlcontrols.pause();
                        dlg.Refresh();
                    }
                    catch (Exception)
                    {
                        dlg = null;
                    }
                }

            }
        }

        private void TelaSecundaria()
        {
            if (dlg.axWindowsMediaPlayer1.URL.ToLower().IndexOf(".mp3") == -1)
            {
                dlg.Show();
            }
            else
            {
                dlg.Hide();
            }

            dlg.Refresh();

            btTocar.ImageIndex = 1;
            dlg.axWindowsMediaPlayer1.Ctlcontrols.play();
        }


        private void btParar_Click_1(object sender, EventArgs e)
        {
            PararVideo();
        }

        private void PararVideo()
        {
            bZoomAtivo = false;

            if ((lblStatusVideo.Text == "3") || (lblStatusVideo.Text == "-")) { return;  }

            try
            {
                axwmpPreview.Ctlcontrols.stop();

                if (dlg == null)
                {
                    return;
                }

                double fade_out = 1;

                btTocar.ImageIndex = 0;
                _PRVW = "3";

                if (chkFade.Checked)
                {
                    do
                    {
                        dlg.Opacity = fade_out;
                        dlg.Refresh();

                        System.Threading.Thread.Sleep(20);

                        fade_out -= 0.1;
                    }
                    while (fade_out > 0);

                }

                if (chkZoom.Checked) { AbrirZoom(0); }

                dlg.axWindowsMediaPlayer1.Ctlcontrols.stop();
                dlg.Hide();

                lblStatusVideo.Text = _PRVW; 

            }
            catch (Exception)
            {
                throw;
            }

        }

        string line, x, y;

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (_ABA == "Cânticos") 
            { 
                lblSt.Text = _PRVW;

                if (lblSt.Text == "z")
                {
                    timer1.Stop();

                    btTocar.ImageIndex = 0;

                    btTocar.Enabled = true;
                    btParar.Enabled = true;

                    Estado = null;

                    if (chkZoom.Checked) { AbrirZoom(0); }
                }
            }

            if (_ABA == "Mídias") 
            {
                if (
                        (lblNomeArq.Text.ToLower().IndexOf(".jpg") > 0) ||
                        (lblNomeArq.Text.ToLower().IndexOf(".jpeg") > 0)
                   )
                {
                    return;
                }

                lblStatusVideo.Text = _PRVW;

                if (lblStatusVideo.Text == "z")
                {
                    timer1.Stop();

                    btTocar.ImageIndex = 0;

                    btTocar.Enabled = true;
                    btParar.Enabled = true;

                    if (chkZoom.Checked) { AbrirZoom(0); }
                }

                if (_PRVW == "1")
                {
                    tsMidia = new TimeSpan(0, 0, Convert.ToInt32(dlg.axWindowsMediaPlayer1.Ctlcontrols.currentPosition));

                    if (dlg.axWindowsMediaPlayer1.Ctlcontrols.currentPosition >= dlg.axWindowsMediaPlayer1.currentMedia.duration)
                    {
                        tsMidia = new TimeSpan(0, 0, Convert.ToInt32(dlg.axWindowsMediaPlayer1.currentMedia.duration));
                    }

                    lblTCorrente.Text = tsMidia.Hours.ToString("00") + ":" + tsMidia.Minutes.ToString("00") + ":" + Convert.ToInt32(tsMidia.Seconds).ToString("00");
                    lblTCorrente.Refresh();
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text == tabControl1.TabPages[1].Text)
            {
                num[0] = num[1];
                num[1] = DriveInfo.GetDrives().Count();
                
                if (num[0] != num[1])
                {
                    lsvArquivos.Items.Clear();

                    trvPastas.Nodes.Clear();
                    PopulateTreeView();
                }
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            Cursor.Show();

            ChecaResolucao();
        }

        private void lsvArquivos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsvArquivos.SelectedItems.Count > 0)
            {
                SelecionaArquivo();
            }
        }

        private void axwmpPreview_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if ((e.newState == 3) && (lblNomeArq.Text.IndexOf(".mp4") < 0))
            {
                axwmpPreview.Ctlcontrols.pause();
            }
        }

        string[] z;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }


        private void UpdateXML(string No, string Valor)
        {
            string sFile = ptMD + @"\config.xml";
            XmlDocument xml = new XmlDocument();

            xml.Load(sFile);

            XmlElement xelZoom = xml.SelectSingleNode("config") as XmlElement;
            if (xelZoom != null)
            {
                XmlElement another = xelZoom.SelectSingleNode(No) as XmlElement;
                another.InnerText = Valor;
                xml.Save(sFile);
            }
        }

        private void chkZoom_CheckedChanged(object sender, EventArgs e)
        {
            UpdateXML("zoom", Convert.ToInt32(chkZoom.Checked).ToString());
        }

 
        private void txtCant1_KeyDown(object sender, KeyEventArgs e)
        {
            Display(txtCant1);
        }

        private void txtCant2_KeyDown(object sender, KeyEventArgs e)
        {
            Display(txtCant2);
        }

        private void txtCant3_KeyDown(object sender, KeyEventArgs e)
        {
            Display(txtCant3);
        }

        private void trvPastas_AfterSelect(object sender, TreeViewEventArgs e)
        {
            axwmpPreview.Ctlcontrols.stop();

            TreeNode newSelected = e.Node;
            lsvArquivos.Items.Clear();

            tssMidia.Text = "< " + e.Node.FullPath.Replace("\\\\","\\")+ " >";

            if (e.Node.IsSelected)
            {
                try
                {
                    DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
                    ListViewItem.ListViewSubItem[] subItems;
                    ListViewItem item = null;

                    foreach (FileInfo file in nodeDirInfo.GetFiles())
                    {
                        if (file.Extension != ".txt")
                        {
                            item = new ListViewItem(file.Name, 1);
                            subItems = new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(item, "File"), new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString()) };

                            item.SubItems.AddRange(subItems);
                            lsvArquivos.Items.Add(item);
                        }
                    }
                }
                catch (Exception)
                {
                    DriveInfo nodeDirInfo = (DriveInfo)newSelected.Tag;
                    ListViewItem.ListViewSubItem[] subItems;
                    ListViewItem item = null;

                    foreach (FileInfo file in nodeDirInfo.RootDirectory.GetFiles())
                    {
                        if (file.Extension != ".txt")
                        {
                            item = new ListViewItem(file.Name, 1);
                            subItems = new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(item, "File"), new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString()) };

                            item.SubItems.AddRange(subItems);
                            lsvArquivos.Items.Add(item);
                        }
                    }
                }
            }

            lsvArquivos.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void trvPastas_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            lsvArquivos.Items.Clear();
        }

        public string MP4Title(string NomeDoArquivo)
        {
            System.IO.StreamReader f = new System.IO.StreamReader(NomeDoArquivo);

            while ((line = f.ReadLine()) != null)
            {
                if (line.IndexOf("data") >= 0)
                {
                    x = line;

                    z = x.Split('�');

                    for (int j = 0; j < z.Length; j++)
                    {
                        if (z[j].IndexOf("nam\0") >= 0)
                        {
                            y = z[j].Replace("nam\0", string.Empty);

                            for (int a = 21; a <= 122; a++)
                            {
                                y = y.Replace(Convert.ToChar(a).ToString() + "data", string.Empty);
                            }

                            y = y.Replace("\0A", string.Empty);
                            y = y.Replace("\0", string.Empty);
                            y = y.Replace("\u0001", string.Empty);

                            break;
                        }
                    }

                    break;
                }
            }

            f.Close();

            return y;
        }

        bool bZoomAtivo;

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lblVol.Text = trackBar1.Value.ToString();
            dlg.axWindowsMediaPlayer1.settings.volume = trackBar1.Value; 
        }

        private void chkFade_CheckedChanged_1(object sender, EventArgs e)
        {
            UpdateXML("fade", Convert.ToInt32(chkFade.Checked).ToString());

        }

        private void trvPastas_DoubleClick(object sender, EventArgs e)
        {
            lsvArquivos.Items.Clear();

            trvPastas.Nodes.Clear();
            PopulateTreeView();
        }

        double dZoom = 1;
        double dOriginalW = 0;
        double dOriginalH = 0;

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if (dlg == null) { return; }

            if (dOriginalW == 0) 
            {
                dOriginalW = dlg.pictureBox1.Width;
                dOriginalH = dlg.pictureBox1.Height;
            }

            dZoom += 0.2;
            dlg.pictureBox1.Width = Convert.ToInt16(dOriginalW * dZoom);
            dlg.pictureBox1.Height = Convert.ToInt16(dOriginalH * dZoom);

            dlg.Refresh();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (dlg == null) { return; }

            if (dOriginalW == 0)
            {
                dOriginalW = dlg.pictureBox1.Width;
                dOriginalH = dlg.pictureBox1.Height;
            }

            dZoom -= 0.2;
            dlg.pictureBox1.Width = Convert.ToInt16(dOriginalW * dZoom);
            dlg.pictureBox1.Height = Convert.ToInt16(dOriginalH * dZoom);

            dlg.Refresh();
        }

        private void btnCima_Click(object sender, EventArgs e)
        {
            if (dlg == null) { return; }

            dlg.pictureBox1.Top -= 50;
            dlg.Refresh();
        }

        private void btnBaixo_Click(object sender, EventArgs e)
        {
            if (dlg == null) { return; }

            dlg.pictureBox1.Top += 50;
            dlg.Refresh();

            // upload para git 01
        }

        private void btnEsq_Click(object sender, EventArgs e)
        {
            if (dlg == null) { return; }

            dlg.pictureBox1.Left -= 50;
            dlg.Refresh();
        }

        private void btnDir_Click(object sender, EventArgs e)
        {
            if (dlg == null) { return; }

            dlg.pictureBox1.Left += 50;
            dlg.Refresh();
        }

        private void 
            AbrirZoom(int Estado)
        {
            if (Estado == 0) 
            {
                ProcessoZoom("S");
                bZoomAtivo = false;
            } 
            else 
            {
                if (!bZoomAtivo)
                {
                    ProcessoZoom("E");
                    bZoomAtivo = true;
                }
            }
        }

        private void ProcessoZoom(string Estado)
        {
            ProcessStartInfo pInfo = new ProcessStartInfo();

            pInfo.FileName = Application.StartupPath + @"\ZOOM_" + Estado + ".exe";

            Process p = Process.Start(pInfo);

            p.WaitForExit();
        }
    }
}
