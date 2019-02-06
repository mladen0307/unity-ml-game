using System;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    private class NeuralSection
    {
        private double[][] _weights;        
        
        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                    res += _weights[i][j];
                res += "\n";
            }
            return res;
        }


        public NeuralSection(int inputCount, int outputCount)
        {

            if (inputCount <= 0)
                throw new ArgumentException("You cannot create a Neural Layer with no input neurons.", "InputCount");
            else if (outputCount <= 0)
                throw new ArgumentException("You cannot create a Neural Layer with no output neurons.", "OutputCount");
           
            _weights = new double[inputCount + 1][]; // +1 for the Bias Neuron

            for (int i = 0; i < _weights.Length; i++)
                _weights[i] = new double[outputCount];

            for (int i = 0; i < _weights.Length; i++)
                for (int j = 0; j < _weights[i].Length; j++)
                    _weights[i][j] = new System.Random().NextDouble() * 2 - 1f;
                  
        }


        public NeuralSection(NeuralSection main)
        {

            _weights = new double[main._weights.Length][];

            for (int i = 0; i < _weights.Length; i++)
                _weights[i] = new double[main._weights[0].Length];

            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    _weights[i][j] = main._weights[i][j];
                }
            }
        }

        public double[] FeedForward(double[] input)
        {
            // Validation Checks
            if (input == null)
                throw new ArgumentException("The input array cannot be set to null.", "Input");
            else if (input.Length != _weights.Length - 1)
                throw new ArgumentException("The input array's length does not match the number of neurons in the input layer.", "Input");

            // Initialize Output Array
            double[] Output = new double[_weights[0].Length];

            // Calculate Value
            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    if (i == _weights.Length - 1) // If is Bias Neuron
                        Output[j] += _weights[i][j]; // Then, the value of the neuron is equal to one
                    else
                        Output[j] += _weights[i][j] * input[i];
                }
            }

            // Apply Activation Function
            for (int i = 0; i < Output.Length; i++)
                Output[i] = ReLU(Output[i]);

            // Return Output
            return Output;
        }

        /// <summary>
        /// Mutate the NeuralSection.
        /// </summary>
        /// <param name="mutationProbablity">The probability that a weight is going to be mutated. (Ranges 0-1)</param>
        /// <param name="mutationAmount">The maximum amount a Mutated Weight would change.</param>
        public void Mutate(double mutationProbablity, double mutationAmount)
        {
            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    if (UnityEngine.Random.value < mutationProbablity)
                        _weights[i][j] = UnityEngine.Random.value * (mutationAmount * 2) - mutationAmount;
                }
            }
        }

        public void MutateNodes(double MutationProbablity, double MutationAmount)
        {
            for (int j = 0; j < _weights[0].Length; j++) // For each output node
            {
                if (UnityEngine.Random.value < MutationProbablity) // Check if we are going to mutate this node
                {
                    for (int i = 0; i < _weights.Length; i++) // For each input node connected to the current output node
                    {
                        _weights[i][j] = UnityEngine.Random.value * (MutationAmount * 2) - MutationAmount; // Mutate the weight connecting both nodes
                    }
                }
            }
        }


        private double ReLU(double x)
        {
            if (x >= 0)
                return x;
            else
                return x / 20;
        }
    }

    public int[] Topology
    {
        get
        {
            return _topology.ToArray();
        }
    }

    List<int> _topology;
    NeuralSection[] _sections;    

    public NeuralNetwork(int[] topology)
    {
        // Validation Checks
        if (topology.Length < 2)
            throw new ArgumentException("A Neural Network cannot contain less than 2 Layers.", "Topology");

        for (int i = 0; i < topology.Length; i++)
        {
            if (topology[i] < 1)
                throw new ArgumentException("A single layer of neurons must contain, at least, one neuron.", "Topology");
        }

        // Set Topology
        _topology = new List<int>(topology);

        // Initialize Sections
        _sections = new NeuralSection[_topology.Count - 1];

        // Set the Sections
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i] = new NeuralSection(_topology[i], _topology[i + 1]);
        }
    }

    /// <summary>
    /// Initiates an independent Deep-Copy of the Neural Network provided.
    /// </summary>
    /// <param name="main">The Neural Network that should be cloned.</param>
    public NeuralNetwork(NeuralNetwork main)
    {
       
        // Set Topology
        _topology = main._topology;

        // Initialize Sections
        _sections = new NeuralSection[_topology.Count - 1];

        // Set the Sections
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i] = new NeuralSection(main._sections[i]);
        }
    }


    public double[] FeedForward(double[] input)
    {
        // Validation Checks
        if (input == null)
            throw new ArgumentException("The input array cannot be set to null.", "Input");
        else if (input.Length != _topology[0])
            throw new ArgumentException("The input array's length does not match the number of neurons in the input layer.", "Input");

        double[] Output = input;

        // Feed values through all sections
        for (int i = 0; i < _sections.Length; i++)
        {
            Output = _sections[i].FeedForward(Output);
        }

        return Output;
    }

    /// <summary>
    /// Mutate the NeuralNetwork.
    /// </summary>
    /// <param name="mutationProbablity">The probability that a weight is going to be mutated. (Ranges 0-1)</param>
    /// <param name="mutationAmount">The maximum amount a mutated weight would change.</param>
    public void Mutate(double mutationProbablity = 0.3, double mutationAmount = 2.0)
    {
        // Mutate each section
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i].Mutate(mutationProbablity, mutationAmount);
        }
    }

    public void MutateNodes(double mutationProbablity = 0.3, double mutationAmount = 2.0)
    {
        // Mutate each section
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i].MutateNodes(mutationProbablity, mutationAmount);
        }
    }

    public override string ToString()
    {
        string res = "";
        for (int i = 0; i < _sections.Length; i++)
        {
            res = res + _sections[i].ToString() + "\n";
        }
        return res;
    }
}