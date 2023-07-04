using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace BackPropagationNNClass_V3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Thread thread = null;

        double[,] pattern = null;

        private void btn_begintrain_Click(object sender, RoutedEventArgs e)
        {
            lsbx_output0.Items[3] = "Desired:" + txb_desiredoutput0.Text;
            lsbx_output1.Items[3] = "Desired:" + txb_desiredoutput1.Text;

            txb_input0.Text = txb_inputpattern0.Text;
            txb_input1.Text = txb_inputpattern1.Text;
            txb_input2.Text = txb_inputpattern2.Text;

            btn_begintrain.IsEnabled = false;

            double[] values = new double[]{ double.Parse(txb_inputpattern0.Text), double.Parse(txb_inputpattern1.Text), double.Parse(txb_inputpattern2.Text),
                double.Parse(txb_desiredoutput0.Text), double.Parse(txb_desiredoutput1.Text) };

            thread = new Thread(new ThreadStart(Run));
            //in order to terminate the thread as soon as the WPF Window closed 
            thread.IsBackground = true;
            thread.Start();
        }

        //"Creating a 3-input, 4-hidden, 2-output neural network
        //Using sigmoid function for input-to-hidden activation
        //Using tanh function for hidden-to-output activation
        ANeuralNetwork nn = new ANeuralNetwork(3, 4, 2);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initScreen();
            // arbitrary weights and biases
            //double[] weights = new double[] {
            //      0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.1, 1.2,
            //      -2.0, -6.0, -1.0, -7.0,
            //      1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0,
            //      -2.5, -5.0 };

            double[] weights = new double[] {
                  0.01, 0.12, 0.3, 0.4, 0.5, 0.6, 0.2, 0.5, 0.09, 0.1, 2.1, 0.2,
                  3.0, 6.0, -1.0, -6.0,
                  1.3, 2.4, 1.5, 0.6, 1.7, 1.8, 1.9, 2.0,
                  -3.5, 3.5 };

            //Loading neural network weights and biases
            nn.SetWeights(weights);

            pattern = new double[4, 5] { 
                                        { 1.0, 0.0, 1.0, 1.0, 0.0 },
                                        { 0.0, 1.0, 0.0, 0.0, 1.0 },
                                        { 1.0, 1.0, 1.0, 1.0, 1.0 },
                                        { 1.0, 1.0, 0.0, 0.0, 0.0 },
                                        };
        }

        private void initScreen()
        {
            lsbx_hidden0.Items.Add("Bias:0.0");
            lsbx_hidden0.Items.Add("In:0.0");
            lsbx_hidden0.Items.Add("Out:0.0");

            lsbx_hidden1.Items.Add("Bias:0.0");
            lsbx_hidden1.Items.Add("In:0.0");
            lsbx_hidden1.Items.Add("Out:0.0");

            lsbx_hidden2.Items.Add("Bias:0.0");
            lsbx_hidden2.Items.Add("In:0.0");
            lsbx_hidden2.Items.Add("Out:0.0");

            lsbx_hidden3.Items.Add("Bias:0.0");
            lsbx_hidden3.Items.Add("In:0.0");
            lsbx_hidden3.Items.Add("Out:0.0");

            lsbx_output0.Items.Add("Bias:0.0");
            lsbx_output0.Items.Add("In:0.0");
            lsbx_output0.Items.Add("Out:0.0");
            lsbx_output0.Items.Add("Desired:0.0");
            lsbx_output0.Items.Add("Error:0.0");

            lsbx_output1.Items.Add("Bias:0.0");
            lsbx_output1.Items.Add("In:0.0");
            lsbx_output1.Items.Add("Out:0.0");
            lsbx_output1.Items.Add("Desired:0.0");
            lsbx_output1.Items.Add("Error:0.0");
        }


        static double Error(double[] target, double[] output) // sum absolute error. could put into NeuralNetwork class.
        {
            double sum = 0.0;
            for (int i = 0; i < target.Length; ++i)
                sum += Math.Abs(target[i] - output[i]);
            return sum;
        }

        private void Run()
        {
            try
            {
                Random rnd = new Random();
                uint ctr = 0;
                while (ctr < 1000)
                {
                    ctr++;
                    //for (int i = 0; i < pattern.GetLength(0); i++)
                    //{
                    int i = rnd.Next(0, pattern.GetLength(0));
                    //Setting inputs:
                    double[] xValues = new double[] { pattern[i, 0], pattern[i, 1], pattern[i, 2] };

                    //double[] initialOutputs = nn.ComputeOutputs(xValues);

                    double[] tValues = new double[] { pattern[i, 3], pattern[i, 4] }; // target (desired) values. note these only make sense for tanh output activation

                    double eta = 0.90;  // learning rate - controls the maginitude of the increase in the change in weights. found by trial and error.
                    double alpha = 0.04; // momentum - to discourage oscillation. found by trial and error.

                    //Entering main back-propagation compute-update cycle
                    //Stopping when sum absolute error <= 0.01 or 1,000 iterations

                    double[] yValues = nn.ComputeOutputs(xValues); // prime the back-propagation loop
                    double error = Error(tValues, yValues);
                    //to update the Label from an thread we have to delegate
                    //the update to UIthread...
                    lbl_iterations.Dispatcher.Invoke(
                          System.Windows.Threading.DispatcherPriority.Normal,
                          new Action(
                            delegate()
                            {
                                lbl_iterations.Content = "Trainning Iterations: " + ctr;
                            }
                        ));

                    //Updating weights and biases using back-propagation
                    nn.UpdateWeights(tValues, eta, alpha);
                    //Computing new outputs:
                    yValues = nn.ComputeOutputs(xValues);

                    //Computing new error
                    error = Error(tValues, yValues);
                    //Console.WriteLine("Error = " + error.ToString("F4"));
                    ActualizeLabel(lbl_totalerror, "Total Error: " + error);
                    //}
                }

                //ActualizeListBox(lsbx_output0, "Error:" + (tValues[0] - yValues[0]), 4);
                //ActualizeListBox(lsbx_output1, "Error:" + (tValues[1] - yValues[1]), 4);

                //Best weights and biases found:
                double[] bestWeights = nn.GetWeights();
                //Helpers.ShowVector(bestWeights, 2, true);
                ShowWeights(bestWeights);
                //to update the Button from an thread we have to delegate
                //the update to UIthread...
                btn_begintrain.Dispatcher.Invoke(
                      System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(
                        delegate()
                        {
                            btn_begintrain.IsEnabled = true;
                        }
                    ));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal: " + ex.Message);
            }

        }

        private void ShowWeights(double[] weights)
        {
            ActualizeTextBox(txb_weight00, weights[0].ToString());
            ActualizeTextBox(txb_weight10, weights[1].ToString());
            ActualizeTextBox(txb_weight20, weights[2].ToString());
            ActualizeTextBox(txb_weight30, weights[3].ToString());

            ActualizeTextBox(txb_weight01, weights[4].ToString());
            ActualizeTextBox(txb_weight11, weights[5].ToString());
            ActualizeTextBox(txb_weight21, weights[6].ToString());
            ActualizeTextBox(txb_weight31, weights[7].ToString());

            ActualizeTextBox(txb_weight02, weights[8].ToString());
            ActualizeTextBox(txb_weight12, weights[9].ToString());
            ActualizeTextBox(txb_weight22, weights[10].ToString());
            ActualizeTextBox(txb_weight32, weights[11].ToString());

            ActualizeListBox(lsbx_hidden0, "Bias:" + weights[12], 0);
            ActualizeListBox(lsbx_hidden1, "Bias:" + weights[13], 0);
            ActualizeListBox(lsbx_hidden2, "Bias:" + weights[14], 0);
            ActualizeListBox(lsbx_hidden3, "Bias:" + weights[15], 0);

            ActualizeTextBox(txb_outweight00, weights[16].ToString());
            ActualizeTextBox(txb_outweight10, weights[17].ToString());

            ActualizeTextBox(txb_outweight01, weights[18].ToString());
            ActualizeTextBox(txb_outweight11, weights[19].ToString());

            ActualizeTextBox(txb_outweight02, weights[20].ToString());
            ActualizeTextBox(txb_outweight12, weights[21].ToString());

            ActualizeTextBox(txb_outweight03, weights[22].ToString());
            ActualizeTextBox(txb_outweight13, weights[23].ToString());

            ActualizeListBox(lsbx_output0, "Bias:" + weights[24], 0);
            ActualizeListBox(lsbx_output1, "Bias:" + weights[25], 0);
        }

        private void ActualizeListBox(ListBox lsbx, string msg, int i)
        {
            //to update the TextBox from an thread we have to delegate
            //the update to UIthread...
            btn_begintrain.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        lsbx.Items[i] = msg;
                    }
                ));

        }

        private void ActualizeTextBox(TextBox txb, string msg)
        {
            //to update the Label from an thread we have to delegate
            //the update to UIthread...
            btn_begintrain.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        txb.Text = msg;
                    }
                ));
        }

        private void ActualizeLabel(Label lbl, string msg)
        {
            //to update the Label from an thread we have to delegate
            //the update to UIthread...
            btn_begintrain.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        lbl.Content = msg;
                    }
                ));
        }

        private void btn_endtrain_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ActualizeWeightBias()
        {
        }

        private void btn_calculate_Click(object sender, RoutedEventArgs e)
        {
            //Setting inputs:
            double[] xValues = new double[] { double.Parse(txb_input0.Text), double.Parse(txb_input1.Text), double.Parse(txb_input2.Text) };

            double[] initialOutputs = nn.ComputeOutputs(xValues);

            double[] tValues = new double[] { double.Parse(txb_desiredout0.Text), double.Parse(txb_desiredout1.Text) }; // target (desired) values. note these only make sense for tanh output activation

            double[] yValues = nn.ComputeOutputs(xValues); // prime the back-propagation loop

            ActualizeListBox(lsbx_output0, "Error:" + (tValues[0] - yValues[0]), 4);
            ActualizeListBox(lsbx_output1, "Error:" + (tValues[1] - yValues[1]), 4);

            //Computing new error
            double error = Error(tValues, yValues);
            //Console.WriteLine("Error = " + error.ToString("F4"));
            lbl_totalerrorOut.Content = "Total Error: " + error;
        }
    }
}