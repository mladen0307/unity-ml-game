using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuralController : MonoBehaviour {

    [SerializeField]
    int generationSize;

    [SerializeField]
    Movement agentPrefab;

    public Text genText;

    int generationCount = 0;
    List<Movement> agents;
    Movement agent;

    NeuralNetwork bestNetwork = new NeuralNetwork(new int[3] { 5, 3, 1 });
    NeuralNetwork runnerUpNetwork = new NeuralNetwork(new int[3] { 5, 3, 1 });
    float bestFitness = -1f;
    float runnerUpFitness = -1f;
    int deathCount;
    
    void Start()
    {       
        deathCount = 0;
        agents = new List<Movement>(generationSize);
        for (int i = 0; i < generationSize; i++)
        {
            agent = Instantiate(agentPrefab, transform.position, transform.rotation);           
            agent.transform.parent = gameObject.transform;
            agent.myNeuralNet = new NeuralNetwork(new int[3]{5, 3, 1});
            agent.myNeuralNet.Mutate();
            agent.id = i;
            agents.Add(agent);
        }
    }

    public Transform AgentToFollow
    {
        get {
            float best = agents[0].fitness;
            Transform a = agents[0].transform;
            for (int i = 1; i < generationSize; i++)
            {
                if (best < agents[i].fitness && agents[i].IsMoving)
                {
                    best = agents[i].fitness;
                    a = agents[i].transform;
                }
            }
            return a;
        }
    }

    public void Death(int id)
    {
        if (agents[id].fitness > bestFitness)
        {
            runnerUpNetwork = bestNetwork;
            runnerUpFitness = bestFitness;
            bestFitness = agents[id].fitness;
            bestNetwork = agents[id].myNeuralNet;
        }
        else if (agents[id].fitness > runnerUpFitness)
        {
            runnerUpFitness = agents[id].fitness;
            runnerUpNetwork = agents[id].myNeuralNet;
        }
        
        deathCount++;        
        if (deathCount == generationSize)
            Invoke("NextGeneration", 0.3f);
    }

    void NextGeneration()
    {
        transform.Rotate(Vector3.up, 180f);
        deathCount = 0;
        agents[0].myNeuralNet = new NeuralNetwork(bestNetwork);        
        agents[0].Respawn();
        //Debug.Log(bestFitness);

        //rekombinacije najbolja 2
        for (int i = 1; i < generationSize/3; i++)
        {            
            agents[i].myNeuralNet = new NeuralNetwork(runnerUpNetwork);
            agents[i].myNeuralNet.Crossover(bestNetwork);
            agents[i].Respawn();
        }
        //dristicna mutacije najboljeg
        for (int i = generationSize / 3; i < 2*generationSize/3; i++)
        {
            agents[i].myNeuralNet = new NeuralNetwork(bestNetwork);
            agents[i].myNeuralNet.Mutate(0.3,2.0);
            agents[i].Respawn();
        }
        //blaga mutacije najboljeg
        for (int i = 2 * generationSize / 3; i <generationSize; i++)
        {
            agents[i].myNeuralNet = new NeuralNetwork(bestNetwork);
            agents[i].myNeuralNet.Mutate(0.3, 1.0);
            agents[i].Respawn();
        }
        generationCount++;
        genText.text = "Generation: " + generationCount;
        //Debug.Log(generationCount);
    }
    

    
       

}
