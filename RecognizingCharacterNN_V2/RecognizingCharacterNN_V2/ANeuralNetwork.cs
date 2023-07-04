using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecognizingCharacterNN_V2
{
    class ANeuralNetwork
    {
        private int inputCount;
        private int hiddenCount;
        private int outputCount;

        private double[] inputs;
        private double[,] iToHWeights; // input-to-hidden weights
        private double[] iToHSums;
        private double[] iToHBiases;
        private double[] iToHOutputs;

        private double[,] hToOutWeights;  // hidden-to-output
        private double[] hToOutSums;
        private double[] hToOutBiases;
        private double[] outputs;

        private double[] outGrads; // output gradients for back-propagation
        private double[] hiddenGrads; // hidden gradients for back-propagation

        private double[,] iToHPrevDeltaWeights;  // for momentum with back-propagation
        private double[] iToHPrevDeltaBiases; // for momentum with back-propagation

        private double[,] hToOutPrevDeltaWeights; // for momentum with back-propagation
        private double[] hToOutPrevDeltaBiases; // for momentum with back-propagation

        public ANeuralNetwork(int inputCount, int hiddenCount, int outputCount)
        {
            this.inputCount = inputCount;
            this.hiddenCount = hiddenCount;
            this.outputCount = outputCount;

            inputs = new double[inputCount];
            iToHWeights = new double[inputCount, hiddenCount];
            iToHSums = new double[hiddenCount];
            iToHBiases = new double[hiddenCount];
            iToHOutputs = new double[hiddenCount];
            hToOutWeights = new double[hiddenCount, outputCount];
            hToOutSums = new double[outputCount];
            hToOutBiases = new double[outputCount];
            outputs = new double[outputCount];

            outGrads = new double[outputCount];
            hiddenGrads = new double[hiddenCount];

            iToHPrevDeltaWeights = new double[inputCount, hiddenCount];
            iToHPrevDeltaBiases = new double[hiddenCount];
            hToOutPrevDeltaWeights = new double[hiddenCount, outputCount];
            hToOutPrevDeltaBiases = new double[outputCount];
        }

        // change the weights and biases using back-propagation, with desired values, eta (learning rate), alpha (momentum)
        public void UpdateWeights(double[] desiredValues, double eta, double alpha)
        {
            // 1. compute output gradients
            for (int i = 0; i < outGrads.Length; ++i)
            {
                double derivative = (1 - outputs[i]) * (1 + outputs[i]); // derivative of tanh
                outGrads[i] = derivative * (desiredValues[i] - outputs[i]); //equations 4.17 from EBook
            }

            // 2. compute hidden gradients
            for (int i = 0; i < hiddenGrads.Length; ++i)
            {
                double derivative = (1 - iToHOutputs[i]) * iToHOutputs[i]; // (1 / 1 + exp(-x))' Equations 4.19 from EBook
                double sum = 0.0;
                //The error signal for a hidden unit is determined recursively in terms of error signals of the
                //units to which it directly connects and the weights of those connections. For the sigmoid
                //activation function:
                //Equations 4.21 from EBook
                for (int j = 0; j < outputCount; ++j) // each hidden delta is the sum of numOutput terms
                    sum += outGrads[j] * hToOutWeights[i, j]; // each downstream gradient * outgoing weight
                hiddenGrads[i] = derivative * sum;
            }

            // 3. update input to hidden weights
            for (int i = 0; i < iToHWeights.GetLength(0); ++i)
            {
                for (int j = 0; j < iToHWeights.GetLength(1); ++j)
                {
                    double delta = eta * hiddenGrads[j] * inputs[i]; //compute the new delta equation 4.16 from EBook
                    //calculate new weight and add momentum using previous delta
                    //equation 4.22 from EBook
                    iToHWeights[i, j] += delta + alpha * iToHPrevDeltaWeights[i, j];
                }
            }

            // 3b. update input to hidden biases
            for (int i = 0; i < iToHBiases.Length; ++i)
            {
                double delta = eta * hiddenGrads[i] * 1.0; // the 1.0 is the constant input for any bias
                iToHBiases[i] += delta;
                iToHBiases[i] += alpha * iToHPrevDeltaBiases[i];
            }

            // 4. update hidden to output weights
            for (int i = 0; i < hToOutWeights.GetLength(0); ++i)  // 0..3 (4)
            {
                for (int j = 0; j < hToOutWeights.GetLength(1); ++j) // 0..1 (2)
                {
                    double delta = eta * outGrads[j] * iToHOutputs[i];  // see above: ihOutputs are inputs to next layer
                    hToOutWeights[i, j] += delta;
                    hToOutWeights[i, j] += alpha * hToOutPrevDeltaWeights[i, j];
                    hToOutPrevDeltaWeights[i, j] = delta;
                }
            }

            // 4b. update hidden to output biases
            for (int i = 0; i < hToOutBiases.Length; ++i)
            {
                double delta = eta * outGrads[i] * 1.0;
                hToOutBiases[i] += delta;
                hToOutBiases[i] += alpha * hToOutPrevDeltaBiases[i];
                hToOutPrevDeltaBiases[i] = delta;
            }
        } // UpdateWeights

        public void SetWeights(double[] weights)
        {
            // copy weights and biases in weights[] array to i-h weights, i-h biases, h-o weights, h-o biases
            int WeightsCount = (inputCount * hiddenCount) + (hiddenCount * outputCount) + hiddenCount + outputCount;

            int k = 0; // points into weights param

            for (int i = 0; i < inputCount; ++i)
                for (int j = 0; j < hiddenCount; ++j)
                    iToHWeights[i, j] = weights[k++];

            for (int i = 0; i < hiddenCount; ++i)
                iToHBiases[i] = weights[k++];

            for (int i = 0; i < hiddenCount; ++i)
                for (int j = 0; j < outputCount; ++j)
                    hToOutWeights[i, j] = weights[k++];

            for (int i = 0; i < outputCount; ++i)
                hToOutBiases[i] = weights[k++];
        }

        public double[] GetWeights()
        {
            //to calculate total number of weights and biases
            int weightsCount = (inputCount * hiddenCount) + (hiddenCount * outputCount) + hiddenCount + outputCount;
            double[] result = new double[weightsCount];
            int k = 0;
            for (int i = 0; i < iToHWeights.GetLength(0); ++i)
                for (int j = 0; j < iToHWeights.GetLength(1); ++j)
                    result[k++] = iToHWeights[i, j];
            for (int i = 0; i < iToHBiases.Length; ++i)
                result[k++] = iToHBiases[i];
            for (int i = 0; i < hToOutWeights.GetLength(0); ++i)
                for (int j = 0; j < hToOutWeights.GetLength(1); ++j)
                    result[k++] = hToOutWeights[i, j];
            for (int i = 0; i < hToOutBiases.Length; ++i)
                result[k++] = hToOutBiases[i];
            return result;
        }

        public double[] ComputeOutputs(double[] xValues)
        {
            for (int i = 0; i < hiddenCount; ++i)
                iToHSums[i] = 0.0;
            for (int i = 0; i < outputCount; ++i)
                hToOutSums[i] = 0.0;

            for (int i = 0; i < xValues.Length; ++i) // copy x-values to inputs
                this.inputs[i] = xValues[i];

            for (int j = 0; j < hiddenCount; ++j)  // compute input-to-hidden weighted sums
                for (int i = 0; i < inputCount; ++i)
                    iToHSums[j] += this.inputs[i] * iToHWeights[i, j];

            for (int i = 0; i < hiddenCount; ++i)  // add biases to input-to-hidden sums
                iToHSums[i] += iToHBiases[i];

            for (int i = 0; i < hiddenCount; ++i)   // determine input-to-hidden output
                iToHOutputs[i] = SigmoidFunction(iToHSums[i]);

            for (int j = 0; j < outputCount; ++j)   // compute hidden-to-output weighted sums
                for (int i = 0; i < hiddenCount; ++i)
                    hToOutSums[j] += iToHOutputs[i] * hToOutWeights[i, j];

            for (int i = 0; i < outputCount; ++i)  // add biases to input-to-hidden sums
                hToOutSums[i] += hToOutBiases[i];

            for (int i = 0; i < outputCount; ++i)   // determine hidden-to-output result
                this.outputs[i] = SigmoidFunction(hToOutSums[i]);
                //this.outputs[i] = HyperTanFunction(hToOutSums[i]);

            double[] result = new double[outputCount]; // could define a GetOutputs method instead
            this.outputs.CopyTo(result, 0);

            return result;
        } // ComputeOutputs

        private static double StepFunction(double x) // an activation function that isn't compatible with back-propagation bcause it isn't differentiable
        {
            if (x > 0.0) return 1.0;
            else return 0.0;
        }

        private static double SigmoidFunction(double x)
        {
            if (x < -45.0) return 0.0;
            else if (x > 45.0) return 1.0;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }

        private static double HyperTanFunction(double x)
        {
            if (x < -10.0) return -1.0;
            else if (x > 10.0) return 1.0;
            else return Math.Tanh(x);
        }
    }
}
