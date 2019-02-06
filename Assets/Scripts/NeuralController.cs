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

    NeuralNetwork bestNetwork = new NeuralNetwork(new int[4] { 5, 3, 2, 1 });
    float bestScore = -1f;    
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
            float best = agents[0].currentDistance;
            Transform a = agents[0].transform;
            for (int i = 1; i < generationSize; i++)
            {
                if (best < agents[i].currentDistance && agents[i].IsMoving)
                {
                    best = agents[i].currentDistance;
                    a = agents[i].transform;
                }
            }
            return a;
        }
    }

    public void Death(int id)
    {
        if (agents[id].maxDistance > bestScore)
        {
            bestScore = agents[id].maxDistance;
            bestNetwork = agents[id].myNeuralNet;
            
        }
        deathCount++;        
        if (deathCount == generationSize)
            Invoke("NextGeneration", 0.5f);
    }

    void NextGeneration()
    {
        
        deathCount = 0;
        agents[0].myNeuralNet = new NeuralNetwork(bestNetwork);
        //Debug.Log(bestScore);
        agents[0].Respawn();        
        for (int i = 1; i < generationSize; i++)
        {            
            agents[i].myNeuralNet = new NeuralNetwork(bestNetwork);
            agents[i].myNeuralNet.MutateNodes();
            agents[i].Respawn();
        }
        generationCount++;
        genText.text = "Generation: " + generationCount;
        //Debug.Log(generationCount);
    }
    

    
       

}
