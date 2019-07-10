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
            
            if (input == null)
                throw new ArgumentException("The input array cannot be set to null.", "Input");
            else if (input.Length != _weights.Length - 1)
                throw new ArgumentException("The input array's length does not match the number of neurons in the input layer.", "Input");

            
            double[] Output = new double[_weights[0].Length];

            
            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    if (i == _weights.Length - 1) // If is Bias Neuron
                        Output[j] += _weights[i][j]; 
                    else
                        Output[j] += _weights[i][j] * input[i];
                }
            }

            
            for (int i = 0; i < Output.Length; i++)
                Output[i] = ReLU(Output[i]);

            
            return Output;
        }

        
        public void Crossover(NeuralSection other)
        {
            if (_weights.Length != other._weights.Length || _weights[0].Length != other._weights[0].Length)
                throw new ArgumentException("Neural Sections must be of equal size");
            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    if (UnityEngine.Random.value < 0.5f)
                        _weights[i][j] = other._weights[i][j];
                }
            }
        }

        public void Mutate(double MutationProbablity, double MutationAmount)
        {
            for (int j = 0; j < _weights[0].Length; j++) 
            {
                if (UnityEngine.Random.value < MutationProbablity) 
                {
                    for (int i = 0; i < _weights.Length; i++) 
                    {
                        _weights[i][j] = UnityEngine.Random.value * (MutationAmount * 2) - MutationAmount; 
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
        
        if (topology.Length < 2)
            throw new ArgumentException("A Neural Network cannot contain less than 2 Layers.", "Topology");

        for (int i = 0; i < topology.Length; i++)
        {
            if (topology[i] < 1)
                throw new ArgumentException("A single layer of neurons must contain, at least, one neuron.", "Topology");
        }

       
        _topology = new List<int>(topology);

        
        _sections = new NeuralSection[_topology.Count - 1];

      
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i] = new NeuralSection(_topology[i], _topology[i + 1]);
        }
    }

   
    public NeuralNetwork(NeuralNetwork main)
    {
       
      
        _topology = main._topology;

        
        _sections = new NeuralSection[_topology.Count - 1];

       
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i] = new NeuralSection(main._sections[i]);
        }
    }


    public double[] FeedForward(double[] input)
    {
        
        if (input == null)
            throw new ArgumentException("The input array cannot be set to null.", "Input");
        else if (input.Length != _topology[0])
            throw new ArgumentException("The input array's length does not match the number of neurons in the input layer.", "Input");

        double[] Output = input;

      
        for (int i = 0; i < _sections.Length; i++)
        {
            Output = _sections[i].FeedForward(Output);
        }
        return Output;
    }

    
    public void Crossover(NeuralNetwork other)
    {
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i].Crossover(other._sections[i]);
        }
    }

    public void Mutate(double mutationProbablity = 0.3, double mutationAmount = 2.0)
    {
        
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i].Mutate(mutationProbablity, mutationAmount);
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