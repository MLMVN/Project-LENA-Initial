using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/*---------------------- Added Libraries ----------------------*/
using System.Runtime.InteropServices; // DLLImport
using System.Threading; // CancellationToken
using System.Xml; // Loading xml file parameters
using BitMiracle.LibTiff.Classic; // Use Tiff images
using System.Diagnostics; // Stopwatch
using Microsoft.WindowsAPICodePack.Taskbar; // Taskbar Progress
using System.IO; // BinaryReader, open and save files
using System.Numerics;

namespace Project_LENA
{
    public partial class Form1 : Form
    {
        Functions functions;
        MLMVN mlmvn;

        CancellationTokenSource cTokenSource1; // Declare a System.Threading.CancellationTokenSource.
        PauseTokenSource pTokenSource1; // Declaring a usermade pausetoken
        CancellationTokenSource cTokenSource2; // Declare a System.Threading.CancellationTokenSource.
        PauseTokenSource pTokenSource2; // Declaring a usermade pausetoken

        public Form1()
        {
            functions = new Functions(this);
            mlmvn = new MLMVN(this);
            this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            this.Location = new Point(0, 0);
            InitializeComponent();          
            this.AutoSize = false;
            this.Height = 250;
            //this.Size = new Size(640, 250); // Initiallize size of window

        }

        // Avoids flickering
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        } 

        // Help button on windows form
        private void Form1_helpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("This program uses an intelligent approach to image processing using MLMVN, " +
                "otherwise known as Multilayer feedforward neural network based on multi-valued neurons." +
                "\n\nThis program can create and generate an image suitable to be filtered, generate samples" +
                "used to create weights used to process images, generate the weights learned from this process," +
                "and filter a noisy image from the weights created. \n\nAdditional help can be provided by " +
                "hovering over an element of interest.\n\n  - Developed by Gabriel Del Pino and Hiroyuki Plumlee.",
                "About Project LENA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            e.Cancel = true; // does not display question mark on cursor after message
        }

        string Title = "Project LENA  1.0.0.2";

        // Selecting different tabs
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == tabPage1.Name)
            {
                this.timer3.Enabled = false;
                this.timer4.Enabled = false;
                this.timer5.Enabled = false;
                this.timer6.Enabled = false;
                this.timer7.Enabled = false;
                this.timer8.Enabled = false;
                this.timer9.Enabled = false;
                this.timer10.Enabled = false;
                this.timer11.Enabled = false;
                this.timer14.Enabled = false;
                this.timer15.Enabled = false;
                this.timer16.Enabled = false;
                if (this.checkBox5.Checked == false) this.timer1.Enabled = true; // Makes tab smaller; user checked checkbox to generate grayscale image
                else if (this.Height >= 370) this.timer13.Enabled = true;
                else if (this.checkBox5.Checked == true) this.timer12.Enabled = true; // Makes tab larger; user checked checkbox to generate grayscale image
                else this.timer2.Enabled = true;
            }

            if (e.TabPage.Name == tabPage2.Name)
            {
                this.timer1.Enabled = false;
                this.timer2.Enabled = false;
                this.timer5.Enabled = false;
                this.timer6.Enabled = false;
                this.timer7.Enabled = false;
                this.timer8.Enabled = false;
                this.timer9.Enabled = false;
                this.timer10.Enabled = false;
                this.timer11.Enabled = false;
                this.timer12.Enabled = false;
                this.timer13.Enabled = false;
                this.timer14.Enabled = false;
                this.timer15.Enabled = false;
                this.timer16.Enabled = false;
                if (this.Height >= 330) this.timer3.Enabled = true;
                else this.timer4.Enabled = true;
            }

            if (e.TabPage.Name == tabPage3.Name)
            {
                this.timer1.Enabled = false;
                this.timer2.Enabled = false;
                this.timer3.Enabled = false;
                this.timer4.Enabled = false;
                this.timer7.Enabled = false;
                this.timer8.Enabled = false;
                this.timer10.Enabled = false;
                this.timer11.Enabled = false;
                this.timer12.Enabled = false;
                this.timer13.Enabled = false;
                this.timer14.Enabled = false;
                this.timer15.Enabled = false;
                if (this.button7.Enabled == false) this.timer9.Enabled = true; // Makes tab larger; user clicked 'Learn'
                else if (this.button12.Enabled == false) this.timer16.Enabled = true; // Makes tab larger; user clicked 'Test'
                else if (this.Height >= 450) this.timer5.Enabled = true;
                else this.timer6.Enabled = true;
            }

            if (e.TabPage.Name == tabPage4.Name)
            {
                this.timer1.Enabled = false;
                this.timer2.Enabled = false;
                this.timer3.Enabled = false;
                this.timer4.Enabled = false;
                this.timer5.Enabled = false;
                this.timer6.Enabled = false;
                this.timer9.Enabled = false;
                this.timer12.Enabled = false;
                this.timer13.Enabled = false;
                this.timer16.Enabled = false;
                if (this.Height >= 650 && this.button10.Enabled == false) this.timer15.Enabled = true;
                else if (this.button10.Enabled == false) this.timer10.Enabled = true; // Makes tab larger; user clicked 'Process Image'
                else if (radioButton3.Checked == true || radioButton4.Checked == true && this.Height > 390) timer14.Enabled = true;
                else if (radioButton3.Checked == true || radioButton4.Checked == true) this.timer11.Enabled = true; // radiobutton enabled              
                else if (this.Height >= 250) this.timer7.Enabled = true;              
                else this.timer8.Enabled = true;
            }
        }

        #region Timers
        // When user clicks tab 1 from a larger tab
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 250) this.timer1.Enabled = false;
            else this.Height -= 20;
        }

        // When user clicks tab 1 from a smaller tab
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 250) this.timer2.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks tab 2 from a larger tab
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 330) this.timer3.Enabled = false;
            else this.Height -= 20;
        }

        // When user clicks tab 2 from a smaller tab
        private void timer4_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 330) this.timer4.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks tab 3 from a larger tab
        private void timer5_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 450) this.timer5.Enabled = false;
            else this.Height -= 20;
        }

        // When user clicks tab 3 from a smaller tab
        private void timer6_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 450) this.timer6.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks tab 4 from a larger tab
        private void timer7_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 250) this.timer7.Enabled = false;
            else this.Height -= 20;
        }

        // When user clicks tab 4 from a smaller tab
        private void timer8_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 250) this.timer8.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks 'Learn' on tab 3
        private void timer9_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 650) this.timer9.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks 'Process Image' on tab 4
        private void timer10_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 650) this.timer10.Enabled = false;
            else this.Height += 20;
        }

        // When user checks a radio button on tab 4
        private void timer11_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 390) this.timer11.Enabled = false;
            else this.Height += 20;
        }

        // When user has grayscale image checkbox on on tab 1 from a smaller tab
        private void timer12_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 370) this.timer12.Enabled = false;
            else this.Height += 20;
        }

        // When user has grayscale image checkbox on on tab 1 from a larger tab
        private void timer13_Tick(object sender, EventArgs e)
        {

            if (this.Height <= 370) this.timer13.Enabled = false;
            else this.Height -= 20;
        }

        // When user checked a radio button on tab 4 from a larger tab
        private void timer14_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 390) this.timer14.Enabled = false;
            else this.Height -= 20;
        }
        // when user clicked 'Process Image' on tab 4 from a larger tab
        private void timer15_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 650) this.timer15.Enabled = false;
            else this.Height -= 20;
        }
        // When user clicks 'Test' on tab 3
        private void timer16_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 610) this.timer16.Enabled = false;
            else this.Height += 20;
        }
        #endregion

        #region Controls

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                textBox6.Enabled = true;
                button19.Enabled = true;
            }
            else
            {
                textBox6.Enabled = false;
                button19.Enabled = false;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == 2)
            {
                checkBox1.Enabled = true;
            }
            else
            {
                checkBox1.Enabled = false;
            }
        }

        // Process using pixels, tab 2
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox7.Enabled = false;
            comboBox6.Enabled = true;
        }

        // Process using patches, tab 2
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox6.Enabled = false;
            comboBox7.Enabled = true;
        }

        // process using pixels, tab 4
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Height == 250) this.timer11.Enabled = true;

            label27.Visible = false;
            maskedTextBox1.Visible = false;
            maskedTextBox2.Visible = false;

            label23.Visible = true;
            label24.Visible = true;
            label24.Text = "Input layer size:";
            label24.Location = new Point(210, 30);
            label25.Visible = true;
            label25.Text = "Hidden layer size:";
            label25.Location = new Point(385, 30);
            label26.Visible = true;
            label26.Text = "Kernel Size:";
            textBox13.Visible = true;
            textBox16.Visible = true;
            textBox16.Location = new Point(296, 27);
            textBox17.Visible = true;
            textBox17.Location = new Point(481, 27);
            comboBox4.Visible = true;
        }

        // process using patches, tab 4
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Height == 250) this.timer11.Enabled = true;

            comboBox4.Visible = false;

            label23.Visible = true;
            label24.Visible = true;
            label24.Text = "Step:";
            label24.Location = new Point(230, 30);
            label25.Visible = true;
            label25.Text = "Layers:";
            label25.Location = new Point(380, 30);
            label26.Visible = true;
            label26.Text = "Network Size:";
            label27.Visible = true;
            textBox13.Visible = true;
            textBox16.Visible = true;
            textBox16.Location = new Point(268, 27);
            textBox17.Visible = true;
            textBox17.Location = new Point(427, 27);
            maskedTextBox1.Visible = true;
            maskedTextBox2.Visible = true;
        }

        // Load color image
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) // Test result.
                textBox11.Text = openFileDialog1.FileName;
        }

        // Create Grayscale Image
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox11.Text))
            {
                MessageBox.Show("Please input the color image.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            else
            {
                Tiff colorimage = Tiff.Open(textBox11.Text, "r");
                // Obtain basic tag information of the image
                #region GetTagInfo
                int width = colorimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = colorimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                #endregion



                int imageSize = height * width;
                int[] raster = new int[imageSize];

                byte[] scanline = new byte[colorimage.ScanlineSize()];

                // Read the image into the memory buffer
                int[,] red = new int[height, width];
                int[,] green = new int[height, width];
                int[,] blue = new int[height, width];
                double[,] grey = new double[width, height];

                double[,] Y = new double[width, height];
                double[,] U = new double[width, height];
                double[,] V = new double[width, height];

                // Here I fix the reversal of the image (vertically) and
                // get the color values from each pixel
                // I want the closest result to VEGA
                for (int i = height - 1; i != -1; i--)
                {
                    colorimage.ReadScanline(scanline, i);
                    for (int j = 0; j < width; j++)
                    {
                        red[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                        green[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                        blue[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct
                        //Y[i, j] = (1 * red[i, j]) + (0 * green[i, j]) + (1.13983 * blue[i, j]);
                        Y[i, j] = (0.299 * red[i, j]) + (0.587 * green[i, j]) + (0.114 * blue[i, j]);
                        U[i, j] = -(0.14713 * red[i, j]) - (0.28886 * green[i, j]) + (0.436 * blue[i, j]);
                        V[i, j] = (0.615 * red[i, j]) - (0.51499 * green[i, j]) - (0.10001 * blue[i, j]);
                        //grey[i,j] = 

  // PSNR 29.12092 ---> grey[i, j] = (red[i, j] + green[i, j] + blue[i, j])/3;
  // PSNR 27.87570 ---> grey[i, j] = (0.299 * red[i, j] + 0.587 * green[i, j] + 0.114 * blue[i, j]);
  // PSNR 27.72297 ---> grey[i, j] = (0.3 * red[i, j] + 0.6 * green[i, j] + 0.1 * blue[i, j]);
  // PSNR 30.61221 ---> grey[i, j] = (0.2126 * red[i, j] + 0.7152 * green[i, j] + 0.0722 * blue[i, j]); 
  // PSNR 30.71359 ---> grey[i, j] = (0.212 * red[i, j] + 0.715 * green[i, j] + 0.072 * blue[i, j]);
  // PSNR 31.40589 ---> grey[i, j] = (0.21 * red[i, j] + 0.71 * green[i, j] + 0.07 * blue[i, j]);
  // PSNR 31.77098 ---> grey[i, j] = (0.2 * red[i, j] + 0.7 * green[i, j] + 0.1 * blue[i, j]); 
  // PSNR 31.85644 ---> grey[i, j] = (0.2 * red[i, j] + 0.72 * green[i, j] + 0.07 * blue[i, j]); // Closest result to VEGA
                        
                    }
                }

                //string FileName = Path.GetFileNameWithoutExtension(textBox11.Text) + "_Y" + ".tif";
                saveFileDialog2.FileName = Path.GetFileNameWithoutExtension(textBox11.Text) + "_Y" + ".tif";

                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {                   
                    using (Tiff output = Tiff.Open(saveFileDialog2.FileName, "w"))
                    {
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        output.SetField(TiffTag.ROWSPERSTRIP, height);
                        output.SetField(TiffTag.XRESOLUTION, 88.0);
                        output.SetField(TiffTag.YRESOLUTION, 88.0);
                        output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                        byte[] im = new byte[width * sizeof(byte /*can be changed depending on the format of the image)*/)];

                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (Y[i, j] > 255) Y[i, j] = 255;
                                if (Y[i, j] < 0) Y[i, j] = 0;
                                im[j] = Convert.ToByte(Y[i, j]);
                            }
                            output.WriteScanline(im, i);
                        }
                        output.WriteDirectory();
                    }
                    //System.Diagnostics.Process.Start(FileName);                   
                }
                textBox18.Text = saveFileDialog2.FileName;
            }
        }

        // Generate Noisy Image
        private void button3_Click(object sender, EventArgs e)
        {
            #region Grayscale Image
            if (checkBox4.Checked == false)
            {
                Tiff greyimage = Tiff.Open(textBox18.Text, "r");
                double noise = Convert.ToDouble(textBox1.Text);

                // Obtain basic tag information of the image
                #region GetTagInfo
                int width = greyimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = greyimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                byte bits = greyimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                byte pixel = greyimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                double dpiX = greyimage.GetField(TiffTag.XRESOLUTION)[0].ToDouble();
                double dpiY = greyimage.GetField(TiffTag.YRESOLUTION)[0].ToDouble();
                #endregion

                if (string.IsNullOrEmpty(textBox18.Text))
                {
                    MessageBox.Show("Please input the grayscale image.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please enter the noise.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (pixel > 1)
                {
                    MessageBox.Show("Grayscale image not entered. Check 'Process color image' for color images.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[,] grey = new byte[height, width];
                grey = Functions.Tiff2Array(greyimage, height, width);

                double greysum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        greysum += grey[i, j];
                    }
                }

                double mean = greysum / (height * width); // image mean

                double variancesum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        variancesum += Math.Pow((grey[i, j] - mean), 2);
                    }
                }
                double dispersion;
                dispersion = variancesum / (height * width); // image dispersion

                double standarddev;
                standarddev = Math.Sqrt(dispersion);

                // int width = Convert.ToInt32(textBox1.Text);
                // int height = Convert.ToInt32(textBox2.Text);

                // calls the "createRandomTiff" method
                double[,] y = Functions.createRandomTiff(width, height, mean, standarddev, noise, grey, checkBox3.Checked);

                string fileName = textBox18.Text;

                if (checkBox3.Checked == true)
                fileName = Path.GetFileNameWithoutExtension(textBox18.Text) + "_gauss_" + Convert.ToString(noise) + ".tif";
                if (checkBox3.Checked == false)
                fileName = "gauss_" + Convert.ToString(noise) + ".tif";

                saveFileDialog2.FileName = fileName;

                if (saveFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                {
                    using (Tiff output = Tiff.Open(saveFileDialog2.FileName, "w"))
                    {
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        output.SetField(TiffTag.ROWSPERSTRIP, height);
                        output.SetField(TiffTag.XRESOLUTION, 88.0);
                        output.SetField(TiffTag.YRESOLUTION, 88.0);
                        output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                        byte[] im = new byte[width * sizeof(byte /*can be changed depending on the format of the image*/)];

                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                im[j] = Convert.ToByte(y[i, j]);
                            }
                            output.WriteScanline(im, i);
                        }
                        output.WriteDirectory();
                    }
                }
               // System.Diagnostics.Process.Start(fileName);
            }
            #endregion

            #region Color Image
            if (checkBox4.Checked == true)
            {
                Tiff colorimage = Tiff.Open(textBox18.Text, "r");
                double noise = Convert.ToDouble(textBox1.Text);

                // Obtain basic tag information of the image
                #region GetTagInfo
                int width = colorimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = colorimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                byte bits = colorimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                byte pixel = colorimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                double dpiX = colorimage.GetField(TiffTag.XRESOLUTION)[0].ToDouble();
                double dpiY = colorimage.GetField(TiffTag.YRESOLUTION)[0].ToDouble();
                #endregion



                if (string.IsNullOrEmpty(textBox18.Text))
                {
                    MessageBox.Show("Please input the grayscale image.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please enter the noise.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (pixel == 1)
                {
                    MessageBox.Show("Color image not entered. Uncheck 'Process color image' for color images.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int imageSize = height * width * 3;
                byte[] raster = new byte[imageSize];

                byte[] scanline = new byte[colorimage.ScanlineSize()];

                // Read the image into the memory buffer
                byte[,] red = new byte[height, width];
                byte[,] green = new byte[height, width];
                byte[,] blue = new byte[height, width];

                for (int i = height - 1; i != -1; i--)
                {
                    colorimage.ReadScanline(scanline, i);
                    for (int j = 0; j < width; j++)
                    {
                        red[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                        green[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                        blue[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct
                    }
                }

                #region Red

                double redsum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        redsum += red[i, j];
                    }
                }

                double mean_R = redsum / (height * width); // image mean

                double variancesum_R = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        variancesum_R += Math.Pow((red[i, j] - mean_R), 2);
                    }
                }
                double dispersion_R;
                dispersion_R = variancesum_R / (height * width); // image dispersion

                double standarddev_R;
                standarddev_R = Math.Sqrt(dispersion_R);

                // int width = Convert.ToInt32(textBox1.Text);
                // int height = Convert.ToInt32(textBox2.Text);

                // calls the "createRandomTiff" method

                double[,] r = Functions.createRandomTiff(width, height, mean_R, standarddev_R, noise, red, checkBox3.Checked);
                #endregion

                #region Green
                double greensum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        greensum += green[i, j];
                    }
                }

                double mean_G = greensum / (height * width); // image mean

                double variancesum_G = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        variancesum_G += Math.Pow((green[i, j] - mean_G), 2);
                    }
                }
                double dispersion_G;
                dispersion_G = variancesum_G / (height * width); // image dispersion

                double standarddev_G;
                standarddev_G = Math.Sqrt(dispersion_G);

                // int width = Convert.ToInt32(textBox1.Text);
                // int height = Convert.ToInt32(textBox2.Text);

                // calls the "createRandomTiff" method

                double[,] g = Functions.createRandomTiff(width, height, mean_G, standarddev_G, noise, green, checkBox3.Checked);
                #endregion

                #region Blue
                double bluesum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        bluesum += blue[i, j];
                    }
                }

                double mean_B = bluesum / (height * width); // image mean

                double variancesum_B = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        variancesum_B += Math.Pow((blue[i, j] - mean_B), 2);
                    }
                }
                double dispersion_B;
                dispersion_B = variancesum_B / (height * width); // image dispersion

                double standarddev_B;
                standarddev_B = Math.Sqrt(dispersion_B);

                // int width = Convert.ToInt32(textBox1.Text);
                // int height = Convert.ToInt32(textBox2.Text);

                // calls the "createRandomTiff" method

                double[,] b = Functions.createRandomTiff(width, height, mean_B, standarddev_B, noise, blue, checkBox3.Checked);
                #endregion

                #region Merge RGB
                //colorimage.ReadScanline(scanline, i);
                byte[,] RGB = new byte[height, colorimage.ScanlineSize()];

                 //merges all color values back to one 2D array
                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        RGB[i, j] = Convert.ToByte(r[i, j]);
                //    }
                //}

                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        RGB[i, j + width] = Convert.ToByte(g[i, j]);
                //    }
                //}
                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        RGB[i, j + (2 * width)] = Convert.ToByte(b[i, j]);
                //    }
                //}

                //for (int i = height - 1; i != -1; i--)
                //{
                //    colorimage.ReadScanline(scanline, i);
                //    for (int j = 0; j < width; j++)
                //    {
                //        red[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                //        green[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                //        blue[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        RGB[i, 3 * j] = Convert.ToByte(r[i, j]);
                    }
                }

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        RGB[i, 3 * j + 1] = Convert.ToByte(g[i, j]);
                    }
                }
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        RGB[i, 3 * j + 2] = Convert.ToByte(b[i, j]);
                    }
                }
                #endregion

                string fileName = textBox18.Text;

                if (checkBox3.Checked == true)
                    fileName = Path.GetFileNameWithoutExtension(textBox18.Text) + "_gauss_" + Convert.ToString(noise) + ".tif";
                if (checkBox3.Checked == false)
                    fileName = "gauss_" + Convert.ToString(noise) + ".tif";

                saveFileDialog2.FileName = fileName;

                if (saveFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                {
                    using (Tiff output = Tiff.Open(saveFileDialog2.FileName, "w"))
                    {
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        output.SetField(TiffTag.ROWSPERSTRIP, height);
                        output.SetField(TiffTag.XRESOLUTION, 88.0);
                        output.SetField(TiffTag.YRESOLUTION, 88.0);
                        output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                        //// Write the tiff tags to the file
                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        //output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        
                       

                        //int width = colorimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                        //int height = colorimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                        //byte bits = colorimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                        //byte pixel = colorimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                        //double dpiX = colorimage.GetField(TiffTag.XRESOLUTION)[0].ToDouble();
                        //double dpiY = colorimage.GetField(TiffTag.YRESOLUTION)[0].ToDouble();

                        byte[] im = new byte[colorimage.ScanlineSize() * sizeof(byte /*can be changed depending on the format of the image*/)];

                        // for (int i = 0; i < height; ++i)
                        for (int i = height - 1; i != -1; i--)
                        {
                            for (int j = 0; j < colorimage.ScanlineSize(); ++j)
                            {
                                im[j] = RGB[i, j];
                            }
                            //output.WriteEncodedStrip(0, im, colorimage.ScanlineSize());
                            //output.WriteEncodedStrip(0, im, width * height);
                            output.WriteScanline(im, i);
                        }
                        output.WriteDirectory();
                        // Actually write the image


                    }
                }
            }
            #endregion
        }

        // Load Clean Image
        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                textBox15.Text = openFileDialog2.FileName;
        }

        // Load Noisy Image
        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK) // Test result.
                textBox14.Text = openFileDialog3.FileName;
        }

        // Generate Samples
        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;

            // open the images
            Tiff cleanimage = Tiff.Open(textBox15.Text, "r");
            Tiff noisedimage = Tiff.Open(textBox14.Text, "r"); ;

            #region Error Checking
            // Error Windows when no image entered
            if (cleanimage == null || noisedimage == null)
            {
                button6.Enabled = true;
                MessageBox.Show("Invalid or no image entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no radio button checked
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                button6.Enabled = true;
                MessageBox.Show("No inplementation checked.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no number of samples entered
            if (comboBox8.Text == "")
            {
                button6.Enabled = true;
                MessageBox.Show("Please enter the number of samples.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            int sSize = Convert.ToInt32(comboBox8.Text);

            // Obtain basic tag information of the image
            #region GetTagInfo
            int width = cleanimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = cleanimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            byte bits = cleanimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
            byte pixel = cleanimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
            double dpiX = cleanimage.GetField(TiffTag.XRESOLUTION)[0].ToDouble();
            double dpiY = cleanimage.GetField(TiffTag.YRESOLUTION)[0].ToDouble();
            #endregion

            // The clean image
            byte[,] clean = new byte[height, width];
            clean = Functions.Tiff2Array(cleanimage, height, width);

            // The noisy image
            byte[,] noised = new byte[height, width];
            noised = Functions.Tiff2Array(noisedimage, height, width);

            #region Samples using Pixels
            if (radioButton1.Checked) // Process using pixels
            {
                // combobox values
                int kernel;
                if (comboBox6.SelectedIndex == 0)
                {
                    kernel = 3;
                }

                else if (comboBox6.SelectedIndex == 1)
                {
                    kernel = 5;
                }

                else if (comboBox6.SelectedIndex == 2)
                {
                    kernel = 7;
                }
                else
                {
                    button6.Enabled = true;
                    MessageBox.Show("No kernel size selected.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string fileName = "Samples_" + comboBox8.Text + "_" + Path.GetFileNameWithoutExtension(textBox15.Text) + "_Pixels" + ".txt";

                saveFileDialog1.FileName = fileName;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Functions.LearnSet(clean, noised, kernel, sSize, saveFileDialog1.FileName);
                }
            }
            #endregion

            #region Samples using Patches
            else if (radioButton2.Checked) // Process using patches
            {
                // combobox values
                int kernel = 0;
                // ************ No longer needed
                //if (comboBox7.SelectedIndex == 0)
                //{
                //    kernel = 9;
                //}

                //else if (comboBox7.SelectedIndex == 1)
                //{
                //    kernel = 11;
                //}

                //else if (comboBox7.SelectedIndex == 2)
                //{
                //    kernel = 13;
                //}

                //else if (comboBox7.SelectedIndex == 3)
                //{
                //    kernel = 15;
                //}

                //else if (comboBox7.SelectedIndex == 4)
                //{
                //    kernel = 17;
                //}
                if (comboBox7.Text == "")
                {
                    // Error Windows when no number of samples entered
                    button6.Enabled = true;
                    MessageBox.Show("No kernel size selected.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else 
                {                 
                    char[] c = comboBox7.Text.ToCharArray(); // seperates compbox elements into an array


                    //for (int i = c.Length - 1; i >= 0; i--)
                    for (int i = 0; i < c.Length; i++)
                    {
                        //if (c[i].ToString() == " " || c[i].ToString() == "x" || c[i].ToString() == "X")
                        if (!char.IsDigit(c[i]))
                            break;
                        else
                            kernel = Convert.ToInt32(comboBox7.Text.Substring(0, i + 1));
                    }

                    // check if number are entered
                    if (kernel == 0)
                    {
                        button6.Enabled = true;
                        MessageBox.Show("Please enter an odd number", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // ************************  Let the user enter any odd number as size of the patch
                    // In this case the user can only enter the one dimension
                    if (kernel % 2 != 1)
                    {
                        button6.Enabled = true;
                        MessageBox.Show("Please enter an odd number", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                

                string fileName = "Samples_"+ comboBox8.Text + "_" + Path.GetFileNameWithoutExtension(textBox15.Text) + "_Patches" + ".txt";

                saveFileDialog1.FileName = fileName;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Functions.LearnSetPatch(clean, noised, kernel, sSize, saveFileDialog1.FileName);
                }
            }
            #endregion

            button6.Enabled = true;
        }

        // Learn button
        private async void button7_Click(object sender, EventArgs e)
        {
            this.button7.Enabled = false;       

            #region Error checking
            // Error Windows when no image entered
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                button7.Enabled = true;
                MessageBox.Show("Samples file not entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                button7.Enabled = true;
                MessageBox.Show("No weight type selected.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox1.SelectedIndex == 1 && string.IsNullOrEmpty(textBox6.Text))
            {
                button7.Enabled = true;
                MessageBox.Show("Selected existing weights, but no weights entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no radio button checked
            if (!maskedTextBox4.MaskCompleted || !maskedTextBox3.MaskCompleted || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox7.Text))
            {
                button7.Enabled = true;
                MessageBox.Show("Check for empty parameters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                button7.Enabled = true;
                MessageBox.Show("No output type selected.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox3.SelectedIndex == -1)
            {
                button7.Enabled = true;
                MessageBox.Show("No stopping criteria algorithm selected.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
            {
                button7.Enabled = true;
                MessageBox.Show("Check for empty threshold values.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region Variable Initiallization
            string Weights = textBox6.Text;

            string Samples = textBox2.Text;

            // New cancellation token
            cTokenSource2 = new CancellationTokenSource();

            // Create a cancellation token from CancellationTokenSource
            var cToken = cTokenSource2.Token;

            // New pause token
            pTokenSource2 = new PauseTokenSource();

            // Create a pause token from PauseTokenSource
            var pToken = pTokenSource2.Token;

            bool randomWeights = false;
            if (comboBox1.SelectedIndex == 0)
            {
                randomWeights = true;
            }
            if (comboBox1.SelectedIndex == 1)
            {
                randomWeights = false;
            }

            int NumberofSamples = Convert.ToInt32(textBox8.Text);

            double GlobalThreshold = Convert.ToInt32(textBox4.Text);
            double LocalThreshold = Convert.ToInt32(textBox5.Text);

            // convert string array to int
            string[] a = maskedTextBox3.Text.Split(',');
            int[] networkSize = new int[4];
            for (int i = 0; i < a.Length; i++)
            {
                networkSize[i] = Convert.ToInt32(a[i]);
            }

            string[] b = maskedTextBox4.Text.Split(',');
            int[] inputsPerSample = new int[4];
            for (int i = 0; i < b.Length; i++)
            {
                inputsPerSample[i] = Convert.ToInt32(b[i]);
            }
            int NumberofSectors = Convert.ToInt32(textBox7.Text);
            #endregion

            this.button15.Enabled = true;
            this.checkBox6.Enabled = true;
            this.button7.Enabled = false;
            this.timer9.Enabled = true;


            //bool RandomWeights = false;            
            
            // simulates work

            //for (int i = 0; i < 20; i++)
            //{
            //    SetText2("Iteration = " + i + " RMSE = null" + Environment.NewLine);
            //    await Task.Delay(1200);
            //}
            //int[,] output = await Task.Run(() =>  mlmvn.MLMVN_TEST(Samples, NumberofSamples, Weights, 4, networkSize, inputsPerSample, NumberofSectors));
            this.Text = Title + " (Working)";
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            try
            {
                Complex[][,] weights = await Task.Run(() => mlmvn.Learning(Samples, NumberofSamples, Weights, 4, networkSize, inputsPerSample, NumberofSectors, GlobalThreshold, LocalThreshold, randomWeights, cTokenSource2.Token, pTokenSource2.Token));
            }
            catch (OperationCanceledException)
            {
                SetText2("\r\nProgress canceled.\r\n");
                // Set the CancellationTokenSource to null when the work is complete.
                cTokenSource1 = null;
            }
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);

            cTokenSource1 = null;
            this.button7.Enabled = true;

            this.Text = Title;
        }


        // Load Noisy Image
        private void button8_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK) // Test result.
                textBox9.Text = openFileDialog3.FileName;
        }

        // Load Weights
        private void button9_Click(object sender, EventArgs e)
        {
            if (openFileDialog4.ShowDialog() == DialogResult.OK) // Test result.
                textBox10.Text = openFileDialog4.FileName;
        }

        // Process Image
        private async void button10_Click(object sender, EventArgs e)
        {
            button10.Enabled = false; // Process Image button
            button11.Enabled = true; // Cancel button
            checkBox2.Checked = false; // Pause button; unchecked            
            checkBox2.Enabled = true; // Pause button; enabled
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button17.Enabled = false;

            // open the noisy image
            Tiff noisyimage = Tiff.Open(textBox9.Text, "r");

            // open the weights
            string weights = textBox10.Text;

            #region Error checking
            // Error Windows when no image entered
            if (noisyimage == null)
            {
                button10.Enabled = true;
                button11.Enabled = false;
                checkBox2.Enabled = false;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                button17.Enabled = true;
                MessageBox.Show("Invalid or no image entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (weights == "")
            {
                button10.Enabled = true;
                button11.Enabled = false;
                checkBox2.Enabled = false;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                button17.Enabled = true;
                MessageBox.Show("No weights entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no radio button checked
            if (!radioButton3.Checked && !radioButton4.Checked)
            {
                button10.Enabled = true;
                button11.Enabled = false;
                checkBox2.Enabled = false;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                button17.Enabled = true;
                MessageBox.Show("No inplementation checked.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            // New cancellation token
            cTokenSource1 = new CancellationTokenSource();

            // Create a cancellation token from CancellationTokenSource
            var cToken = cTokenSource1.Token;

            // New pause token
            pTokenSource1 = new PauseTokenSource();

            // Create a pause token from PauseTokenSource
            var pToken = pTokenSource1.Token;

            // Obtain basic tag information of the image
            #region GetTagInfo
            int width = noisyimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = noisyimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            byte bits = noisyimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
            byte pixel = noisyimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
            double dpiX = noisyimage.GetField(TiffTag.XRESOLUTION)[0].ToDouble();
            double dpiY = noisyimage.GetField(TiffTag.YRESOLUTION)[0].ToDouble();
            #endregion

            // Display information
            SetText1("Image information:" + Environment.NewLine);
            SetText1("Width is : " + width + "\r\nHeight is: " + height + "\r\nDpi is: " + dpiX
                + "\r\nThe scanline is " + noisyimage.ScanlineSize() + ".\r\nBits per Sample is: " + bits + "\r\nSample per pixel is: " + pixel + "\r\n" + Environment.NewLine);

            // Store the intensity values of the image to 2d array                              
            byte[,] noisy = new byte[height, width];
            noisy = Functions.Tiff2Array(noisyimage, height, width);

            // Update title text
            this.Text = Title + " (Working)";

            #region Process using pixels
            if (radioButton3.Checked) // Process using pixels
            {
                try
                {
                    if (string.IsNullOrEmpty(textBox13.Text) || string.IsNullOrEmpty(textBox16.Text) || string.IsNullOrEmpty(textBox17.Text) ||
                        comboBox4.SelectedIndex == -1)
                    {
                        button10.Enabled = true;
                        button11.Enabled = false;
                        checkBox2.Enabled = false;
                        radioButton3.Enabled = true;
                        radioButton4.Enabled = true;
                        button17.Enabled = true;
                        MessageBox.Show("Please load or enter parameters.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // Enable the resize event
                    this.timer10.Enabled = true;

                    // parameters
                    int numberofsectors = Convert.ToInt32(textBox13.Text);
                    int inLayerSize = Convert.ToInt32(textBox16.Text);
                    int hidLayerSize = Convert.ToInt32(textBox17.Text);
                    int kernel;
                    if (comboBox4.SelectedIndex == 0)
                    {
                        kernel = 3; // 3x3 window
                    }

                    else if (comboBox4.SelectedIndex == 1)
                    {
                        kernel = 5; // 5x5 window
                    }

                    else if (comboBox4.SelectedIndex == 2)
                    {
                        kernel = 7; //7x7 window
                    }
                    else
                    {
                        MessageBox.Show("No kernel size selected.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        button10.Enabled = true;
                        button11.Enabled = false;
                        checkBox2.Enabled = false;
                        radioButton3.Enabled = true;
                        radioButton4.Enabled = true;
                        button17.Enabled = true;
                        this.Text = Title;
                        return;
                    }

                    // Initiallization of progress bar elements
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                    progressBar1.Maximum = height + 8;
                    progressBar1.Value = 0;
                    TaskbarManager.Instance.SetProgressValue(0, progressBar1.Maximum);
                    NativeMethods.SetState(progressBar1, 1);

                    progressBar1.Value += 4;
                    TaskbarManager.Instance.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    byte[,] denoised = await Task.Run(() => mlmvn.Activation(noisy, kernel, weights, numberofsectors, inLayerSize, hidLayerSize, cTokenSource1.Token, pTokenSource1.Token, progressBar1.Value, progressBar1.Maximum));

                    // Stop timing
                    stopwatch.Stop();

                    // Write result
                    SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);

                    string fileName = Path.GetFileNameWithoutExtension(textBox9.Text) + "_Processed_Pixels" + ".tif";

                    saveFileDialog2.FileName = fileName;

                    if (saveFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                    {

                        functions.WriteToFile(denoised, width, height, bits, pixel, dpiX, dpiY, saveFileDialog2.FileName);

                    }       
                }
                catch (OperationCanceledException)
                {
                    SetText1("\r\nProgress canceled.\r\n");
                    // Set the CancellationTokenSource to null when the work is complete.
                    cTokenSource1 = null;

                    this.Text = Title;
                    // Stop timing
                    stopwatch.Stop();

                    // Write result
                    SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);
                    button10.Enabled = true;
                    button11.Enabled = false;
                    checkBox2.Enabled = false;
                    radioButton3.Enabled = true;
                    radioButton4.Enabled = true;
                    button17.Enabled = true;
                    return;
                }
            }
            #endregion

            #region Process using patches
            else if (radioButton4.Checked) // Process using patches
            {
                try
                {
                    if (string.IsNullOrEmpty(textBox13.Text) || string.IsNullOrEmpty(textBox16.Text) || string.IsNullOrEmpty(textBox17.Text) ||
                        !maskedTextBox1.MaskCompleted || !maskedTextBox2.MaskCompleted)
                    {
                        MessageBox.Show("Please load or enter parameters.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        button10.Enabled = true;
                        button11.Enabled = false;
                        checkBox2.Enabled = false;
                        radioButton3.Enabled = true;
                        radioButton4.Enabled = true;
                        button17.Enabled = true;
                        return;
                    }

                    // Enable the resize event
                    this.timer10.Enabled = true;

                    // parameters
                    int numberofsectors = Convert.ToInt32(textBox13.Text);
                    int step = Convert.ToInt32(textBox16.Text);
                    int layer = Convert.ToInt32(textBox17.Text);
                    

                    // convert string array to int
                    string[] a = maskedTextBox1.Text.Split(',');
                    int[] networkSize = new int[4];
                    for (int i = 0; i < a.Length; i++)
                    {
                        networkSize[i] = Convert.ToInt32(a[i]);
                    }

                    string[] b = maskedTextBox2.Text.Split(',');
                    int[] inputsPerSample = new int[4];
                    for (int i = 0; i < b.Length; i++)
                    {
                        inputsPerSample[i] = Convert.ToInt32(b[i]);
                    }

                    int pSize = (int)Math.Sqrt(inputsPerSample[0] - 1);
                    int range_y = (height - pSize) / step + 2;
                    int range_x = (width - pSize) / step + 2;


                    // Initiallization of progress bar elements
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                    progressBar1.Maximum = (range_x * range_y) + 4;// * ( 4 + range_y % 4) + 4; // range_x * (range into fourths + range_Y % 4) + 4
                    progressBar1.Step = range_x;
                    progressBar1.Value = 0;
                    TaskbarManager.Instance.SetProgressValue(0, progressBar1.Maximum);
                    NativeMethods.SetState(progressBar1, 1);
                    progressBar1.Value += 2;                    
                    TaskbarManager.Instance.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                    
                    byte[,] denoised = await Task.Run(() => mlmvn.fdenoiseNeural(noisy, step, weights, layer, networkSize, inputsPerSample, numberofsectors, cTokenSource1.Token, pTokenSource1.Token, progressBar1.Value, progressBar1.Maximum));

                    string fileName = Path.GetFileNameWithoutExtension(textBox9.Text) + "_Processed_Patch" + ".tif";

                    // Stop timing
                    stopwatch.Stop();

                    // Write result
                    SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);

                    saveFileDialog2.FileName = fileName;

                    if (saveFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                    {
                        using (Tiff output = Tiff.Open(saveFileDialog2.FileName, "w"))
                        {
                            functions.WriteToFile(denoised, width, height, bits, pixel, dpiX, dpiY, saveFileDialog2.FileName);                            
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    SetText1("\r\nProgress canceled.\r\n");
                    // Set the CancellationTokenSource to null when the work is complete.
                    cTokenSource1 = null;

                    this.Text = Title;
                    // Stop timing
                    stopwatch.Stop();

                    // Write result
                    SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);
                    button10.Enabled = true;
                    button11.Enabled = false;
                    checkBox2.Enabled = false;
                    radioButton3.Enabled = true;
                    radioButton4.Enabled = true;
                    button17.Enabled = true;
                    return;
                }
            }
            #endregion

            // Set the CancellationTokenSource to null when the work is complete.
            cTokenSource1 = null;

            this.Text = Title;         

            button10.Enabled = true;
            button11.Enabled = false;
            checkBox2.Enabled = false;
            radioButton3.Enabled = true;
            radioButton4.Enabled = true;
            button17.Enabled = true;
            progressBar1.Value = 0;
            TaskbarManager.Instance.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
        }

        // Cancel Button
        private void button11_Click(object sender, EventArgs e)
        {
            if (cTokenSource1 != null)
            {
                // progressBar color to red
                NativeMethods.SetState(progressBar1, 2);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused); // Fix to Windows 7 Progressbar bug
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                cTokenSource1.Cancel();
            }
        }

        // Test Weights
        private async void button12_Click(object sender, EventArgs e)
        {
            //this.button7.Enabled = false;
            this.button12.Enabled = false;
            #region Variable Initiallization
            string Weights = textBox6.Text;

            string Samples = textBox2.Text;
            #endregion            
            
            #region Error checking
            // Error Windows when no image entered
            if (Samples == "")
            {
                //button7.Enabled = true;
                button12.Enabled = true;
                MessageBox.Show("Samples file not entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Weights == "")
            {
                //button7.Enabled = true;
                button12.Enabled = true;
                MessageBox.Show("Please input existing weights for testing.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no radio button checked
            if (!maskedTextBox4.MaskCompleted || !maskedTextBox3.MaskCompleted || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox7.Text))
            {
                //button7.Enabled = true;
                button12.Enabled = true;
                MessageBox.Show("Check for empty parameters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            int NumberofSamples = Convert.ToInt32(textBox8.Text);

            // convert string array to int
            string[] a = maskedTextBox3.Text.Split(',');
            int[] networkSize = new int[4];
            for (int i = 0; i < a.Length; i++)
            {
                networkSize[i] = Convert.ToInt32(a[i]);
            }

            string[] b = maskedTextBox4.Text.Split(',');
            int[] inputsPerSample = new int[4];
            for (int i = 0; i < b.Length; i++)
            {
                inputsPerSample[i] = Convert.ToInt32(b[i]);
            }
            int NumberofSectors = Convert.ToInt32(textBox7.Text);
            
            //this.button7.Enabled = false;
            this.button12.Enabled = false;
            this.timer16.Enabled = true;

            //int[] networkSizez = new int[4] { 511, 511, 511, 169 };
            //int[] inputsPerSamplez = new int[4] { 170, 512, 512, 512 };

            //int[,] output = mlmvn.MLMVN_TEST("Lena_Y_gauss_0.1.tif_Kernel_13_LearnSet_Patch(corrected).txt", 200, "Lena_Y_gauss_0.1_rmse_3.0_session_1.wgt", 4, networkSizez, inputsPerSamplez, 384);

            int[,] output = await Task.Run(() =>  mlmvn.TEST(Samples, NumberofSamples, Weights, 4, networkSize, inputsPerSample, NumberofSectors));

            //this.button7.Enabled = true;
            this.button12.Enabled = true;
        }

        // Load Samples
        private void button13_Click(object sender, EventArgs e)
        {
            if (openFileDialog6.ShowDialog() == DialogResult.OK) // Test result.
                textBox2.Text = openFileDialog6.FileName;
        }

        // Load Weights
        private void button19_Click(object sender, EventArgs e)
        {
            if (openFileDialog4.ShowDialog() == DialogResult.OK) // Test result.
                textBox6.Text = openFileDialog4.FileName;
        }

        // Easter egg =P
        private void label22_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("TextLenaHalf.tif");
        }

        // Pause Button - Indeed!
        public void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked == true)
            {
                this.Text = Title + " (Paused)";
                SetText1("Process is paused." + Environment.NewLine);
                this.checkBox2.Text = "Resume";
                NativeMethods.SetState(progressBar1, 3);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error); // Fix to Windows 7 Progressbar bug
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                button11.Enabled = false;
                pTokenSource1.IsPaused = !pTokenSource1.IsPaused;              
            }
            if (this.checkBox2.Checked == false)
            {
                this.Text = Title + " (Working)";
                SetText1("Process is resumed." + Environment.NewLine);
                this.checkBox2.Text = "Pause";
                NativeMethods.SetState(progressBar1, 1);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                button11.Enabled = true;
                pTokenSource1.IsPaused = !pTokenSource1.IsPaused;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked == true)
            {
                textBox18.Size = new Size(365, 20);
                textBox18.Location = new Point(87, 27);
                label29.Text = "Color Image:";
                checkBox3.Text = "Add Gaussian noise to color image";
            }
            if (this.checkBox4.Checked == false)
            {
                textBox18.Size = new Size(342, 20);
                textBox18.Location = new Point(110, 27);
                label29.Text = "Grayscale Image:";
                checkBox3.Text = "Add Gaussian noise to grayscale image";                         
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox5.Checked == true)
            {
                this.timer12.Enabled = true;
            }
            if (this.checkBox5.Checked == false)
            {
                timer1.Enabled = true;
            }
        }

        // Loading the parameters using an xml file
        private void button17_Click(object sender, EventArgs e)
        {
            // string textbox13, string textbox16, string textbox17, int combobox4)
            if (openFileDialog5.ShowDialog() == DialogResult.OK) // Load image parameters
            {
                xmlread(openFileDialog5.FileName);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) // Test result.
                textBox18.Text = openFileDialog1.FileName;
        }

        #endregion

        #region drag and drop

        /* Consists of 2 parts:
         *      Entering the region within an object
         *      Dragging and dropping an element onto the object
         */

        private void textBox11_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox11_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox11.Text = fileName;
            }
        }

        private void textBox15_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox15_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox15.Text = fileName;
            }
        }

        private void textBox14_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox14_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox14.Text = fileName;
            }
        }

        private void textBox9_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox9_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox9.Text = fileName;
            }
        }

        private void textBox10_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox10_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox10.Text = fileName;
            }
        }

        private void textBox18_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }
        private void textBox18_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox18.Text = fileName;
            }
        }       

        private void groupBox4_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void groupBox4_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                xmlread(fileName);
            }
        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox2.Text = fileName;
            }
        }

        private void textBox6_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox6_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox6.Text = fileName;
            }
        }

        private void groupBox6_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void groupBox6_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                xmlparameters(fileName);
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox6.Checked == true)
            {
                this.Text = Title + " (Paused)";
                SetText2("Process is paused." + Environment.NewLine);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                this.checkBox6.Text = "Resume";
                button15.Enabled = false;
                pTokenSource2.IsPaused = !pTokenSource2.IsPaused;
            }
            if (this.checkBox6.Checked == false)
            {
                this.Text = Title + " (Working)";
                SetText2("Process is resumed." + Environment.NewLine);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
                this.checkBox6.Text = "Pause";
                button15.Enabled = true;
                pTokenSource2.IsPaused = !pTokenSource2.IsPaused;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (cTokenSource2 != null)
            {
                cTokenSource2.Cancel();
            }
        }
        #endregion

        #region Form Functions

        delegate void SetTextCallback(string text);

        // Textbox for tab 4
        public void SetText1(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox12.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText1);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox12.AppendText(text);
            }
        }

        // Textbox for tab 3
        public void SetText2(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox12.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox3.AppendText(text);
            }
        }

        delegate int SetProgressCallback(int value);

        // Progressbar for tab 4
        public int SetProgress1(int value)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (progressBar1.InvokeRequired)
            {
                SetProgressCallback e = new SetProgressCallback(SetProgress1);
                this.Invoke(e, new object[] { value });
            }
            else
            {
                this.progressBar1.Value += value;
            }
            return progressBar1.Value;
        }

        // read parameters from xml file
        public void xmlread(string FileName)
        {
            // Loading from a file
            XmlReader Xml = XmlReader.Create(FileName);

            while (Xml.Read())
            {
                if (Xml.NodeType == XmlNodeType.Element && Xml.Name == "Method")
                {
                    if (Xml.GetAttribute(0) == "Kernel")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "Parameters")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.Name == "numberofsectors")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox13.Text = Xml.Value; // Number of sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "inLayerSize")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox16.Text = Xml.Value; // Input layer size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "hidLayerSize")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox17.Text = Xml.Value; // Hidden layer size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "kernel")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox4.SelectedIndex = Convert.ToInt32(Xml.Value); // Number of sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                }
                            }
                        }
                    }
                    else if (Xml.GetAttribute(0) == "Patch")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "Parameters")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.Name == "numberofsectors")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox13.Text = Xml.Value; // Number of sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "step")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox16.Text = Xml.Value; // Input layer size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "layer")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox17.Text = Xml.Value; // Hidden layer size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "networkSize")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                maskedTextBox1.Text = Xml.Value; // Network size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "inputsPerSample")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                maskedTextBox2.Text = Xml.Value; // Inputs per sample
                                            }
                                        }
                                        Xml.Read();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void xmlparameters(string FileName)
        {
            // Loading from a file
            XmlReader Xml = XmlReader.Create(FileName);

            while (Xml.Read())
            {
                while (Xml.NodeType != XmlNodeType.EndElement)
                {
                    Xml.Read();
                    if (Xml.Name == "Parameters")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "inputspersample")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.NodeType == XmlNodeType.Text)
                                    {
                                        maskedTextBox4.Text = Xml.Value; // Number of sectors
                                    }
                                }
                                Xml.Read();
                            }
                            if (Xml.Name == "sizeofnetwork")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.NodeType == XmlNodeType.Text)
                                    {
                                        maskedTextBox3.Text = Xml.Value; // Input layer size
                                    }
                                }
                                Xml.Read();
                            }
                            if (Xml.Name == "output")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.NodeType == XmlNodeType.Text)
                                    {
                                        comboBox2.SelectedIndex = Convert.ToInt32(Xml.Value); // Hidden layer size
                                    }
                                }
                                Xml.Read();
                            }
                            if (Xml.Name == "samples")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.NodeType == XmlNodeType.Text)
                                    {
                                        textBox8.Text = Xml.Value;
                                    }
                                }
                                Xml.Read();
                            }
                            if (Xml.Name == "numberofsectors")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.NodeType == XmlNodeType.Text)
                                    {
                                        textBox7.Text = Xml.Value;
                                    }
                                }
                                Xml.Read();
                            }
                        }
                    }
                }
            }                        
        }
        #endregion                                                          

    }

    public static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
