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

namespace Perceptron_Threshold_P23
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

        private void ellipse1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double w1 = 0, w2 = 0, i1 = 0, i2 = 0, y = 0, bias = 0;
            i1 = Convert.ToDouble(txb_input1.Text);
            i2 = Convert.ToDouble(txb_input2.Text);
            w1 = Convert.ToDouble(txb_weight1.Text);
            w2 = Convert.ToDouble(txb_weight2.Text);
            bias = Convert.ToDouble(txb_bias.Text);

            y = i1 * w1 + i2 * w2 + bias;

            if (y > 0)
                y = 1;
            else
                y = -1;

            lbl_output.Content = y.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txb_bias.Text = "-2.0";
            txb_weight1.Text = "1.0";
            txb_weight2.Text = "2.0";
            txb_input1.Text = "1.0";
            txb_input2.Text = "1.0";
        }

        private void btn_train_Click(object sender, RoutedEventArgs e)
        {   
            double w1 = 0, w2 = 0, i1 = 0, i2 = 0,y=0, dy = 0, bias = 0, dw1, dw2;
            i1 = Convert.ToDouble(txb_inputpattern1.Text);
            i2 = Convert.ToDouble(txb_inputpattern2.Text);
            w1 = Convert.ToDouble(txb_weight1.Text);
            w2 = Convert.ToDouble(txb_weight2.Text);
            bias = Convert.ToDouble(txb_bias.Text);
            dy = Convert.ToDouble(txb_desiredoutput.Text);
            //calculate actual output
            y = i1 * w1 + i2 * w2 + bias;

            if (y > 0)
                y = 1;
            else
                y = -1;
            //when Desired output is equal to actual output then no adjustments need
            if(dy != y)
            {
                //calculate new bias
                bias = bias + dy;// bias = dy  here was an ERROR!!!!!!!!!!!!!!!!!!
                txb_bias.Text = bias.ToString();
                //
                dw1 = dy * i1;
                dw2 = dy * i2;
                //calculate new wights
                w1 = w1 + dw1;
                w2 = w2 + dw2;
                txb_weight1.Text = w1.ToString();
                txb_weight2.Text = w2.ToString();
                //trainning is finshed for one pattern.....
            }

        }
    }
}
