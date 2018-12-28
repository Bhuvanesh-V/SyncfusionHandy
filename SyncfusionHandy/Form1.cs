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

namespace SyncfusionHandy
{
    public partial class Form1 : Form
    {
        #region Constant variables
        const string startUpApplicationsFileName = "startup.csv";
        const string urlKeywordFileName = "keyword.csv";
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        #endregion

        #region Native Methods
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        #region Private variables
        private Dictionary<int, string> cmdProcessDictionary = new Dictionary<int, string>();
        private Boolean addKey;
        private Boolean addValue;
        private string value;
        #endregion

        public Form1()
        {
            InitializeComponent();
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(textBox1.Text))
            {
                //MessageBox.Show("You enterred " + textBox1.Text);
                //string tempURl = "https://syncfusion.atlassian.net/secure/RapidBoard.jspa?rapidView=407";
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    if (this.addKey)
                    {
                        this.value = textBox1.Text;
                        textBox1.Text = "Enter the URL";
                        this.textBox1.SelectAll();
                        this.addKey = false;
                        this.addValue = true;
                    } else if(this.addValue)
                    {
                        Utils.SaveUrlByKeyword(urlKeywordFileName, this.value, textBox1.Text);
                        this.addValue = false;
                        this.textBox1.Text = "Key word Added successfully";
                        this.textBox1.SelectAll();
                    } else if(this.textBox1.Text.ToLower() == "snap")
                    {
                        this.saveScreen();
                    }
                    else if (this.textBox1.Text.ToLower() == "cmd")
                    {
                        Process.Start("cmd.exe");
                    }
                    else if (this.textBox1.Text.ToLower() == "tempdel")
                    {
                        Utils.deleteTemp();
                    }
                    else
                    {
                        try
                        {
                            OpenBrowser(textBox1.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Not valid URL");
                        }
                    }
                    this.textBox1.SelectAll();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {   
            this.MouseDown += Form1_MouseDown;
            //RunCommandDebug(@"C:\Users\anandarajt\tempRepo", "git clone https://github.com/syncfusion/ej2-quickstart.git quickstart\n cd quickstart\n call npm install\n call npm start");
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void RunCommandDebug(string path, string cmd)
        {
            string batFileName = DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + ".bat";
            File.WriteAllText(batFileName, @"cd /d " + path + "\n" + cmd + " \nset /p DUMMY=Hit ENTER to continue...");
            Process cmdProcess = Process.Start(batFileName);
            cmdProcessDictionary[cmdProcess.Id] = batFileName;
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Exited += cmdProcess_Exited;
        }

        void RunCommand(string path, string cmd)
        {
            string batFileName = DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + ".cmd";
            File.WriteAllText(batFileName, @"cd /d " + path + "\n" + cmd);
            Process cmdProcess = Process.Start(batFileName);
            cmdProcessDictionary[cmdProcess.Id] = batFileName;
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Exited += cmdProcess_Exited;
        }

        void cmdProcess_Exited(object sender, EventArgs e)
        {
            File.Delete(cmdProcessDictionary[(sender as Process).Id]);
        }

        /// <summary>
        /// Method to open the browser
        /// </summary>
        /// <param name="url">url to open</param>
        void OpenBrowser(string keyword)
        {
            try {
                if (keyword.EndsWith(".exe") && keyword.IndexOf("http:\\") == -1 && keyword.IndexOf("https:\\") == -1)
                {
                    Process.Start(keyword);
                }
                else
                {
                    Dictionary<string, string> keywordDictionary = Utils.GetUrlByKeyword(urlKeywordFileName);
                    string caseInsensitiveKeyword = "";
                    foreach (var key in keywordDictionary.Keys)
                    {
                        if(key.ToLower() == keyword.ToLower())
                        {
                            caseInsensitiveKeyword = key;
                            break;
                        }
                    }
                    Process.Start(keywordDictionary[caseInsensitiveKeyword]);
                }
            }
            catch (Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// Method to start applications when running our application
        /// </summary>
        /// <param name="fileName"></param>
        void StartUpApplications()
        {
            List<string> startUp = Utils.GetStartUpApplications(startUpApplicationsFileName);
            foreach (var start in startUp)
            {
                Process.Start(start);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button1, "Alarm");
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button2, "Add items");
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button3, "Edit items");
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button4, "View Items");
        }

        private void button5_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button5, "Browse");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //new CommandCreator().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "Enter the key word";
            this.addKey = true;
            this.textBox1.Focus();
            this.textBox1.SelectAll();
        }

        private void saveScreen()
        {
            this.Hide();
            Utils.takeScreen();
            this.Show();
            this.textBox1.Text = "Screen shot saved!";
            this.textBox1.SelectAll();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Process.Start("notepad++.exe");
        }
    }
}
