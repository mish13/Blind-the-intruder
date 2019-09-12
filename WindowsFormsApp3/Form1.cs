using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private bool mouseDown;
        private Point lastLocation = new Point(0,0);
        private bool maximized = false;
        byte[] intByteArr;
        byte[,] keyValuePair;
        private bool encrypt = true;
        private bool confirmed = false;

        public Form1()
        {
            InitializeComponent();
            selectPanel.Height = btHome.Height;
            selectPanel.Top = btHome.Top;
            homePanel1.BringToFront();
            homePanel2.BringToFront();
            decryptPanel.SendToBack();
            encryptPanel.SendToBack();
            controlPanel.SendToBack();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btExit_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btHome_Click_1(object sender, EventArgs e)
        {
            selectPanel.Height = btHome.Height;
            selectPanel.Top = btHome.Top;
            homePanel1.BringToFront();
            homePanel2.BringToFront();
            decryptPanel.SendToBack();
            encryptPanel.SendToBack();
            controlPanel.SendToBack();
        }

        private void btEncrypt_Click_1(object sender, EventArgs e)
        {
            selectPanel.Height = btEncrypt.Height;
            selectPanel.Top = btEncrypt.Top;
            encryptPanel.BringToFront();
            controlPanel.BringToFront();
            decryptPanel.SendToBack();
            homePanel1.SendToBack();
            homePanel2.SendToBack();
            encrypt = true;
        }

        private void btDecrypt_Click_1(object sender, EventArgs e)
        {
            selectPanel.Height = btDecrypt.Height;
            selectPanel.Top = btDecrypt.Top;
            decryptPanel.BringToFront();
            homePanel2.SendToBack();
            controlPanel.BringToFront();
            encryptPanel.SendToBack();
            homePanel1.SendToBack();
            encrypt = false;
        }

        private void btClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = new Point(e.X,e.Y);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this.lastLocation.X, p.Y - this.lastLocation.Y);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void btMaximize_Click(object sender, EventArgs e)
        {
            
        }

        private void btBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Multiselect = false;
            if (od.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = od.FileName;
            }
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbConfirmPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if (!File.Exists(tbPath.Text))
            {
                MessageBox.Show("No file is chosen.");
                return;
            }
            if (String.IsNullOrEmpty(tbPassword.Text))
            {
                MessageBox.Show("Please enter password.");
                return;
            }
            if (String.IsNullOrEmpty(tbConfirmPassword.Text))
            {
                MessageBox.Show("Please confirm your password.");
            }
            if (tbPassword.Text != tbConfirmPassword.Text)
            {
                MessageBox.Show("Confirmed password doesn't match.");
                tbConfirmPassword.Clear();
            }
            if (!String.IsNullOrEmpty(tbPassword.Text) && tbPassword.Text == tbConfirmPassword.Text)
                confirmed = true;
            try
            {
                byte[] fileContent = File.ReadAllBytes(tbPath.Text);
                int fileSize = fileContent.Length;
                byte[] keys = new byte[fileSize];
                byte[] password = Encoding.ASCII.GetBytes(tbPassword.Text);
                int mod = password.Length;

                for (int i = 0; i < fileSize; i++)
                {
                    keys[i] = password[i % mod];
                }

                byte[] result = new byte[fileSize];

                if (encrypt && confirmed)
                {
                    for (int i = 0; i < fileSize; i++)
                    {
                        int valueIndex = -1, keyIndex = -1;

                        for (int j = 0; j < 256; j++)
                        {
                            if (intByteArr[j] == fileContent[i])
                            {
                                valueIndex = j;
                                break;
                            }
                        }

                        for (int j = 0; j < 256; j++)
                        {
                            if (intByteArr[j] == keys[i])
                            {
                                keyIndex = j;
                                break;
                            }
                        }

                        result[i] = keyValuePair[keyIndex, valueIndex];
                    }


                    tbPassword.Clear();
                    tbConfirmPassword.Clear();
                }
                else if(!encrypt && confirmed)
                {

                    for (int i = 0; i < fileSize; i++)
                    {
                        int valueIndex = -1, keyIndex = -1;

                        for (int j = 0; j < 256; j++)
                        {
                            if (intByteArr[j] == keys[i])
                            {
                                keyIndex = j;
                                break;
                            }
                        }

                        for (int j = 0; j < 256; j++)
                        {
                            if (keyValuePair[keyIndex, j] == fileContent[i])
                            {
                                valueIndex = j;
                                break;
                            }
                        }

                        result[i] = intByteArr[valueIndex];
                    }
                    tbPassword.Clear();
                    tbConfirmPassword.Clear();
                }
                if(confirmed)
                {
                    String fileExtension = Path.GetExtension(tbPath.Text);
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Files (*" + fileExtension + ") | *" + fileExtension;
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(saveFile.FileName, result);
                    }
                }
                
            }
            catch
            {
                MessageBox.Show("The file is in use. Close the file first");
            }

        }

        private void tbPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            intByteArr = new byte[256];
            keyValuePair = new byte[256, 256];

            for (int i = 0; i < 256; i++)
            {
                intByteArr[i] = Convert.ToByte(i);
            }

            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    keyValuePair[i, j] = intByteArr[(i + j) % 256];
                }
            }
        }
    }
}
