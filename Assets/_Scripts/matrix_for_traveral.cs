using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

//public class Matrix : MonoBehaviour
//{
//    public double[][] am;
//    public UnityEvent carryTraversal;
//    private Dijkstra dijkstra;

//    public static double EuclideanDistance(double[] point1, double[] point2)
//    {
//        if (point1.Length != 3 || point2.Length != 3)
//        {
//            throw new ArgumentException("Points must be three-dimensional (have three coordinates)");
//        }

//        double sum = 0;
//        for (int i = 0; i < 3; i++)
//        {
//            sum += Math.Pow(point2[i] - point1[i], 2);
//        }

//        return Math.Sqrt(sum);
//    }

//    public static double[,] BuildAdjacencyMatrix(double[][] points)
//    {
//        int numPoints = points.Length;
//        double[,] adjacencyMatrix = new double[numPoints, numPoints];

//        for (int i = 0; i < numPoints; i++)
//        {
//            for (int j = i + 1; j < numPoints; j++)
//            {
//                double distance = EuclideanDistance(points[i], points[j]);
//                adjacencyMatrix[i, j] = distance;
//                adjacencyMatrix[j, i] = distance; // Symmetric matrix
//            }
//        }

//        return adjacencyMatrix;
//    }

//    double[][] ReadCSVAndPlaceObjects(string filePath)
//    {
//        string[] lines = File.ReadAllLines(filePath);
//        double[][] positionValues = new double[lines.Length][];

//        for (int i = 0; i < lines.Length; i++)
//        {
//            string[] values = lines[i].Split(',');

//            // Create inner array and initialize it
//            positionValues[i] = new double[3]; // Assuming there are 3 coordinates (x, y, z)

//            // Adjusting the loop condition for j and starting from index 1 to skip the index
//            for (int j = 1; j <= 3; j++)
//            {
//                // Extract position data
//                positionValues[i][j - 1] = double.Parse(values[j]); // Subtracting 1 to start from index 0
//            }
//        }
//        return positionValues;
//    }

//    private void Start()
//    {
//        dijkstra = GetComponent<Dijkstra>();
//    }

//    void MapReady()
//    {
//        //carryTraversal.Invoke();

//        //dijkstra.FindShortestPath(am, 0);
//    }

//    void Awake()
//    {
//        //double[][] data = {
//        //    new double[] {1, -0.4788473, 1.1236, 0.8431017},
//        //    new double[] {2, 0.8081815, 1.1236, 0.7799187},
//        //    new double[] {3, -0.56209, 1.1236, -0.2303917},
//        //    new double[] {4, 0.2425411, 1.1236, -0.1319482},
//        //    new double[] {5, 0.07138279, 1.1236, 0.2956725},
//        //    new double[] {6, -0.4410695, 1.1236, 0.257916},
//        //    new double[] {7, -0.4613852, 1.1236, 0.7778232},
//        //    new double[] {8, 0.004018113, 1.1236, 0.8296145},
//        //    new double[] {9, 0.1530583, 1.1236, 0.6373396},
//        //    new double[] {10, 0.002217069, 1.1236, 0.4179556},
//        //    new double[] {11, -0.2579177, 1.1236, 0.3716281},
//        //    new double[] {12, -0.6400596, 1.1236, 0.2755805},
//        //    new double[] {13, -0.8828001, 1.1236, -0.3721753},
//        //    new double[] {14, 0.4739525, 1.1236, -0.01307175},
//        //};
//        string assetsPath = Application.dataPath;
//        string filePath = Path.Combine(assetsPath, "../data/placedObjectData.csv");
//        double[][] data = ReadCSVAndPlaceObjects(filePath);

//        double[,] adjacencyMatrix = BuildAdjacencyMatrix(data);

//        int rows = adjacencyMatrix.GetLength(0);
//        int cols = adjacencyMatrix.GetLength(1);
//        am = new double[rows][];
//        for (int i = 0; i < rows; i++)
//        {
//            am[i] = new double[cols];
//            for (int j = 0; j < cols; j++)
//            {
//                am[i][j] = adjacencyMatrix[i, j];
//            }
//        }

//        Invoke("MapReady", 2f);

//        Debug.Log("Adjacency Matrix:");
//        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
//        {
//            string row = "";
//            for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
//            {
//                row += adjacencyMatrix[i, j] + " ";
//            }
//            Debug.Log(row);
//        }

//    }
//}


public class Matrix : MonoBehaviour
{
    public int[,] map;

    void Start()
    {
        // Define the path to the CSV file
        string assetsPath = Application.dataPath;
        string filePath = Path.Combine(Application.persistentDataPath, "placedObjectData.csv");


        // Read all lines from the CSV file
        string[] lines = File.ReadAllLines(filePath);

        // Determine the number of nodes
        int numNodes = lines.Length;

        // Initialize the adjacency matrix
        int[,] adjacencyMatrix = new int[numNodes, numNodes];

        // Process the CSV data and fill the adjacency matrix
        for (int i = 0; i < numNodes; i++)
        {
            string[] currentNode = lines[i].Split(',');

            int currentNodeNumber = int.Parse(currentNode[0]);

            // Set the immediate previous node
            if (i > 0)
            {
                string[] previousNode = lines[i - 1].Split(',');
                int previousNodeNumber = int.Parse(previousNode[0]);
                adjacencyMatrix[currentNodeNumber - 1, previousNodeNumber - 1] = 1;
            }

            // Set the immediate next node
            if (i < numNodes - 1)
            {
                string[] nextNode = lines[i + 1].Split(',');
                int nextNodeNumber = int.Parse(nextNode[0]);
                adjacencyMatrix[currentNodeNumber - 1, nextNodeNumber - 1] = 1;
            }
        }

        // Remove nodes without adjacent edges
        RemoveIsolatedNodes(ref adjacencyMatrix);

        // Print the adjacency matrix (for debugging)
        Debug.Log("Adjacency Matrix:");
        for (int i = 0; i < numNodes; i++)
        {
            string row = "";
            for (int j = 0; j < numNodes; j++)
            {
                row += adjacencyMatrix[i, j] + " ";
            }
            Debug.Log(row);
        }
        map = adjacencyMatrix;
    }

    void RemoveIsolatedNodes(ref int[,] adjacencyMatrix)
    {
        int numNodes = adjacencyMatrix.GetLength(0);
        List<int> nodesToRemove = new List<int>();

        // Check for isolated nodes (nodes with no adjacent edges)
        for (int i = 0; i < numNodes; i++)
        {
            bool hasAdjacentEdge = false;
            for (int j = 0; j < numNodes; j++)
            {
                if (adjacencyMatrix[i, j] == 1 || adjacencyMatrix[j, i] == 1)
                {
                    hasAdjacentEdge = true;
                    break;
                }
            }
            if (!hasAdjacentEdge)
            {
                nodesToRemove.Add(i);
            }
        }

        // Remove isolated nodes from the adjacency matrix
        foreach (int nodeIndex in nodesToRemove)
        {
            for (int i = 0; i < numNodes; i++)
            {
                adjacencyMatrix[nodeIndex, i] = 0;
                adjacencyMatrix[i, nodeIndex] = 0;
            }
        }
    }
}
