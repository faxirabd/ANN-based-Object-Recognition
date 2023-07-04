using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoModul;
using System.Threading;
using System.Speech.Synthesis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WebCamNN_V6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Creating a 19200 -inputs, 24-hidden, 8-output neural network
        //Using sigmoid function for input-to-hidden activation
        //Using sigmoid function for hidden-to-output activation
        ANeuralNetwork nn = new ANeuralNetwork(19200, 24, 8);
        WebCamService webcamservice = null;
        double[] pattern = null;
        double[][] trainpattern = null;
        string[] trainpatternnames = null;
        double[][] desiredValues = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            trainpatternnames = new string[8];

            //initialize ComboBox
            cmbx_patternnumber.Items.Add("Pattern 0");
            cmbx_patternnumber.Items.Add("Pattern 1");
            cmbx_patternnumber.Items.Add("Pattern 2");
            cmbx_patternnumber.Items.Add("Pattern 3");
            cmbx_patternnumber.Items.Add("Pattern 4");
            cmbx_patternnumber.Items.Add("Pattern 5");
            cmbx_patternnumber.Items.Add("Pattern 6");
            cmbx_patternnumber.Items.Add("Pattern 7");

            cmbx_showpattern.Items.Add("Pattern 0");
            cmbx_showpattern.Items.Add("Pattern 1");
            cmbx_showpattern.Items.Add("Pattern 2");
            cmbx_showpattern.Items.Add("Pattern 3");
            cmbx_showpattern.Items.Add("Pattern 4");
            cmbx_showpattern.Items.Add("Pattern 5");
            cmbx_showpattern.Items.Add("Pattern 6");
            cmbx_showpattern.Items.Add("Pattern 7");

            //initialize the neural network !!!!!
            //calculate numbers of weights and biases needed for the neural network
            int weightsBiasesCount = ((19200) * 24) + (24 * 80) + 24 + 80;

            double[] weights = new double[weightsBiasesCount];

            // randomly choose weights and biases
            Random rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < weightsBiasesCount; i++)
                weights[i] = ((double)(rnd.Next(-10, 11))) / 10.0;


            //Loading neural network weights and biases
            nn.SetWeights(weights);

            pattern = new double[19200];
            //prepair the pattern which has to be recognized....
            desiredValues = new double[8][];
            desiredValues[0] = new double[] { 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };// 0
            desiredValues[1] = new double[] { 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };// 1
            desiredValues[2] = new double[] { 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0 };// 2
            desiredValues[3] = new double[] { 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0 };// 3
            desiredValues[4] = new double[] { 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0 };// 4
            desiredValues[5] = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0 };// 5
            desiredValues[6] = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0 };// 6
            desiredValues[7] = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0 };// 7

            trainpattern = new double[8][];
            // pictureBox1.Image = MakeGrayscale(new Bitmap("C:\\tmp\\Penguins.bmp"));
            webcamservice = new WebCamService(pictureBox1);
            lbl_brightness.Text = "Brightness: " + (trcbr_brightness.Value = webcamservice.BrightnessControl = 70);
            
            webcamservice.Start();
        }

        private void btn_snapshoot_Click(object sender, EventArgs e)
        {
            txb_patternname.Text = "";
            webcamservice.Stop();
            Bitmap pic = (Bitmap)pictureBox1.Image;
            int index = 0;
            for (int i = 0; i < pic.Width; i++)
            {
                for (int j = 0; j < pic.Height; j++)
                {
                    //get the pixel from the original image
                    Color originalColor = pic.GetPixel(i, j);

                    int value = (int)(originalColor.R);
                    if (value == 255)
                        pattern[index] = 0.0;
                    else
                        pattern[index] = 1.0;

                    index++;
                }
            }
        }

        Thread thread = null;
        private void btn_train_Click(object sender, EventArgs e)
        {
            //loadPattern_3_Automatically(); //just for testing

            btn_train.Enabled = false;

            thread = new Thread(new ParameterizedThreadStart(Run));
            //in order to terminate the thread as soon as the WPF Window closed 
            thread.IsBackground = true;
            thread.Start(int.Parse(txb_numbIterations.Text));
        }

        static double Error(double[] target, double[] output) // sum absolute error. could put into NeuralNetwork class.
        {
            double sum = 0.0;
            for (int i = 0; i < target.Length; ++i)
                sum += Math.Abs(target[i] - output[i]);
            return sum;
        }


        private void Run(object numiteration)
        {
            try
            {
                Random rnd = new Random();
                uint ctr = 0;
                int num = (int)numiteration;
                double[] yValues = null;

                for (int i = 0; i < 8; i++)
                    yValues = nn.ComputeOutputs(trainpattern[i]); // prime the back-propagation loop

                while (ctr <= num)
                {
                    lbl_iteration.Text = "Iteration: " + ctr;

                    ctr++;
                    int i = rnd.Next(0, trainpattern.Length);
                    //int i = (int)ctr % 8;

                    double eta = 0.1;  // learning rate - controls the maginitude of the increase in the change in weights. found by trial and error.
                    double alpha = 0.04; // momentum - to discourage oscillation. found by trial and error.

                    //Entering main back-propagation compute-update cycle
                    //Stopping when sum absolute error <= 0.01 or 1,000 iterations

                    //Computing new outputs:
                    yValues = nn.ComputeOutputs(trainpattern[i]);
                    double error = Error(desiredValues[i], yValues);
                    //to update the Label from an thread we have to delegate
                    //the update to UIthread...

                    switch (i)
                    {
                        case 0:
                            lbl_trainerror0.Text = "Error0: " + error;
                            break;
                        case 1:
                            lbl_trainerror1.Text = trainpatternnames[i] + ":  " + error;
                            break;
                        case 2:
                            lbl_trainerror2.Text = trainpatternnames[i] + ":  " + error;
                            break;
                        case 3:
                            lbl_trainerror3.Text = trainpatternnames[i] + ":  " + error;
                            break;
                        case 4:
                            lbl_trainerror4.Text = trainpatternnames[i] + ":  " + error;
                            break;
                        case 5:
                            lbl_trainerror5.Text = trainpatternnames[i] + ":  " + error;
                            break;
                        case 6:
                            lbl_trainerror6.Text = trainpatternnames[i] + ":  " + error;
                            break;
                        case 7:
                            lbl_trainerror7.Text = trainpatternnames[i] + ":  " + error;
                            break;
                    };




                    if (error < 0.001)
                        break;

                    //Updating weights and biases using back-propagation
                    nn.UpdateWeights(desiredValues[i], eta, alpha);

                }

                btn_train.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal: " + ex.Message);
            }

        }

        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private void btn_recognize_Click(object sender, EventArgs e)
        {

            /*****************************************************************/
            lbl_recognrror0.ForeColor = Color.Black;
            lbl_recognrror0.Font = new Font(lbl_recognrror0.Font.FontFamily, 8.25F);

            lbl_recognrror1.ForeColor = Color.Black;
            lbl_recognrror1.Font = new Font(lbl_recognrror0.Font.FontFamily, 8.25F);

            lbl_recognrror2.ForeColor = Color.Black;
            lbl_recognrror2.Font = new Font(lbl_recognrror0.Font.FontFamily, 8.25F);

            lbl_recognrror3.ForeColor = Color.Black;
            lbl_recognrror3.Font = new Font(lbl_recognrror0.Font.FontFamily, 8.25F);

            lbl_recognrror4.ForeColor = Color.Black;
            lbl_recognrror4.Font = new Font(lbl_recognrror0.Font.FontFamily, 8.25F);

            lbl_recognrror5.ForeColor = Color.Black;
            lbl_recognrror5.Font = new Font(lbl_recognrror0.Font.FontFamily, 8.25F);

            lbl_recognrror6.ForeColor = Color.Black;
            lbl_recognrror6.Font = new Font(lbl_recognrror0.Font.FontFamily, 8.25F);

            lbl_recognrror7.ForeColor = Color.Black;
            lbl_recognrror7.Font = new Font(lbl_recognrror0.Font.FontFamily, 8.25F);
            /*****************************************************************/
            double[] Errors = new double[8];
            int found = 0;

            for (int p = 0; p < 8; p++)
            {
                double[] yValues = nn.ComputeOutputs(pattern); // prime the back-propagation loop

                //Computing new error
                Errors[p] = Error(desiredValues[p], yValues);

                if (Errors[p] < Errors[found])
                    found = p;

              
                switch (p)
                {
                    case 0:
                       
                        lbl_recognrror0.Text = trainpatternnames[p] + ":  " + Errors[p];
                        break;
                    case 1:


                        lbl_recognrror1.Text = trainpatternnames[p] + ":  " + Errors[p];
                        break;
                    case 2:

                        lbl_recognrror2.Text = trainpatternnames[p] + ":  " + Errors[p];
                        break;
                    case 3:

                        lbl_recognrror3.Text = trainpatternnames[p] + ":  " + Errors[p];
                        break;
                    case 4:

                        lbl_recognrror4.Text = trainpatternnames[p] + ":  " + Errors[p];
                        break;
                    case 5:

                        lbl_recognrror5.Text = trainpatternnames[p] + ":  " + Errors[p];
                        break;
                    case 6:

                        lbl_recognrror6.Text = trainpatternnames[p] + ":  " + Errors[p];
                        break;
                    case 7:

                        lbl_recognrror7.Text = trainpatternnames[p] + ":  " + Errors[p];
                        break;
                };
            }

            this.Text = found.ToString();

            switch (found)
                {
                    case 0:
                        lbl_recognrror0.ForeColor = Color.DarkGreen;
                        lbl_recognrror0.Font = new Font(lbl_recognrror0.Font.FontFamily, 16);
                        break;
                   case 1:
                        lbl_recognrror1.ForeColor = Color.DarkGreen;
                        lbl_recognrror1.Font = new Font(lbl_recognrror0.Font.FontFamily, 16);
                        break;
                    case 2:
                        lbl_recognrror2.ForeColor = Color.DarkGreen;
                        lbl_recognrror2.Font = new Font(lbl_recognrror0.Font.FontFamily, 16);
                        break;
                    case 3:
                        lbl_recognrror3.ForeColor = Color.DarkGreen;
                        lbl_recognrror3.Font = new Font(lbl_recognrror0.Font.FontFamily, 16);
                        break;
                    case 4:
                        lbl_recognrror4.ForeColor = Color.DarkGreen;
                        lbl_recognrror4.Font = new Font(lbl_recognrror0.Font.FontFamily, 16);
                        break;
                    case 5:
                        lbl_recognrror5.ForeColor = Color.DarkGreen;
                        lbl_recognrror5.Font = new Font(lbl_recognrror0.Font.FontFamily, 16);
                        break;
                    case 6:
                        lbl_recognrror6.ForeColor = Color.DarkGreen;
                        lbl_recognrror6.Font = new Font(lbl_recognrror0.Font.FontFamily, 16);
                        break;
                    case 7:
                        lbl_recognrror7.ForeColor = Color.DarkGreen;
                        lbl_recognrror7.Font = new Font(lbl_recognrror0.Font.FontFamily, 16);
                        break;
                };

            //speek say the name of recognized image
            PromptBuilder prompt = new PromptBuilder();

            PromptStyle style = new PromptStyle();
            style.Rate = PromptRate.ExtraSlow;
            style.Emphasis = PromptEmphasis.Strong;
            prompt.StartStyle(style);
            prompt.AppendText(trainpatternnames[found]);
            prompt.EndStyle();

            synthesizer.SpeakAsync(prompt);
        }

        private void btn_startvideo_Click(object sender, EventArgs e)
        {
            cmbx_showpattern.Enabled = false;
            webcamservice = new WebCamService(pictureBox1);
            webcamservice.BrightnessControl = trcbr_brightness.Value;
            webcamservice.Start();
        }

        int patternNr = 0;
        private void btn_addpattern_Click(object sender, EventArgs e)
        {
            if (txb_patternname.Text == "")
            {
                MessageBox.Show("Please give the actual Pattern a name!");
                return;
            }

            trainpatternnames[patternNr] = txb_patternname.Text;

            lbl_patterncount.Text = "Pattern Number: " + patternNr;

            trainpattern[patternNr] = (double[])pattern.Clone();

            cmbx_patternnumber.SelectedIndex = patternNr;
            patternNr++;
            if (patternNr >= 8)
                btn_addpattern.Enabled = false;
        }

        void loadPattern_3_Automatically()
        {
            trainpattern[0] = new double[192] {
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                                           };

            trainpattern[1] = new double[192] {
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                                            };

            trainpattern[2] = new double[192] {
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                                            };

            trainpattern[3] = new double[192] {
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                                           };

            trainpattern[4] = new double[192] {
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                                            };

            trainpattern[5] = new double[192] {
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                                           };

            trainpattern[6] = new double[192] {
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                                            };

            trainpattern[7] = new double[192] {
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                            0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0,
                                                           };
        }

        void loadPattern_4_Automatically()
        {
            trainpattern[0] = new double[192] {
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                                           };

            trainpattern[1] = new double[192] {
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                                           };


            trainpattern[2] = new double[192] {
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                                           };

            trainpattern[3] = new double[192] {
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0, -1.0, -1.0,
                                                           };

            trainpattern[4] = new double[192] {
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0, -1.0, -1.0,
                                                           };

            trainpattern[5] = new double[192] {
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, +1.0, +1.0,
                                                           };

            trainpattern[6] = new double[192] {
                                            +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0,
                                            +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                                           };

            trainpattern[7] = new double[192] {
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0,
                                            +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0, +1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                            -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0,
                                                           };
        }

        private void cmbx_patternnumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_addpattern.Enabled = true;
            patternNr = cmbx_patternnumber.SelectedIndex;

            lbl_patterncount.Text = "Pattern Number: " + patternNr;
        }

        private void btn_showpattern_Click(object sender, EventArgs e)
        {
            webcamservice.Stop();
            cmbx_showpattern.Enabled = true;
        }

        private void cmbx_showpattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pNr = cmbx_showpattern.SelectedIndex;

           

            Bitmap pic =  (Bitmap)pictureBox1.Image;
            Bitmap pic2 = new Bitmap(pic.Width, pic.Height);

            if (trainpattern[pNr] == null)
            {
                pictureBox1.Image = pic2;
                return;
            }

            int index = 0;
            for (int i = 0; i < pic.Width; i++)
            {
                for (int j = 0; j < pic.Height; j++)
                {
                    int value = 0;
                    if (trainpattern[pNr][index] == 0.0)
                        value = 255;
                    else 
                        value = 0;

                    //create the color object
                    Color newColor = Color.FromArgb(value, value, value);
                    pic2.SetPixel(i, j, newColor);
                    index++;
                }
            }

            pictureBox1.Image = pic2;

            pattern = (double[])trainpattern[pNr].Clone();
        }

        private void trcbr_brightness_Scroll(object sender, EventArgs e)
        {
            lbl_brightness.Text = "Brightness: " + trcbr_brightness.Value;

            webcamservice.BrightnessControl = trcbr_brightness.Value;
        }

        private void saveNN_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "NN files (*.nn)|*.nn";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                Stream stream = File.Open(path, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, nn);
                stream.Close();
            }
        }

        private void btn_loadNN_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "NN files (*.nn)|*.nn";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                nn = null;

                Stream stream = File.Open(path, FileMode.Open);
                BinaryFormatter bformatter = new BinaryFormatter();

                nn = (ANeuralNetwork)bformatter.Deserialize(stream);
                stream.Close();
            }
        }

        private void btn_savepattern_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Pattern files (*.ptr)|*.ptr";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                Stream stream = File.Open(path, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, trainpattern);
                bformatter.Serialize(stream, trainpatternnames);
                stream.Close();
            }
        }

        private void btn_loadpattern_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Pattern files (*.ptr)|*.ptr";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                trainpattern = null;
                trainpatternnames = null;

                Stream stream = File.Open(path, FileMode.Open);
                BinaryFormatter bformatter = new BinaryFormatter();

                trainpattern = (double[][])bformatter.Deserialize(stream);
                trainpatternnames = (string[])bformatter.Deserialize(stream);
                stream.Close();
            }
        }
    }
}
