using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MatrixTraversal : MonoBehaviour
{
    [SerializeField]
    Matrix matrix;

    GameObject[] allObjects; // Array to store all objects in the scene

    [SerializeField]
    PlaceCsvObj csvObj;

    public List<int> pos;

    public LineRenderer lineRenderer; // Reference to the LineRenderer component

    // Function to perform Dijkstra's algorithm and return the shortest path
    public static List<int> Dijkstra(int[,] adjacencyMatrix, int startNode, int endNode)
    {
        int numNodes = adjacencyMatrix.GetLength(0);

        // Initialize arrays for distances and previous nodes
        int[] distance = new int[numNodes];
        int[] previous = new int[numNodes];
        bool[] visited = new bool[numNodes];

        // Initialize distances to infinity and previous nodes to -1 (indicating no previous node)
        for (int i = 0; i < numNodes; i++)
        {
            distance[i] = int.MaxValue;
            previous[i] = -1;
        }

        // Distance from startNode to itself is 0
        distance[startNode] = 0;

        // Find shortest path
        for (int count = 0; count < numNodes - 1; count++)
        {
            int u = MinDistance(distance, visited);
            visited[u] = true;

            for (int v = 0; v < numNodes; v++)
            {
                if (!visited[v] && adjacencyMatrix[u, v] != 0 && distance[u] != int.MaxValue &&
                    distance[u] + adjacencyMatrix[u, v] < distance[v])
                {
                    distance[v] = distance[u] + adjacencyMatrix[u, v];
                    previous[v] = u;
                }
            }
        }

        // Reconstruct shortest path
        List<int> shortestPath = new List<int>();
        int current = endNode;
        while (current != -1)
        {
            shortestPath.Add(current);
            current = previous[current];
        }
        shortestPath.Reverse();
        return shortestPath;
    }

    // Helper function to find the node with the minimum distance
    private static int MinDistance(int[] distance, bool[] visited)
    {
        int min = int.MaxValue;
        int minIndex = -1;

        for (int v = 0; v < distance.Length; v++)
        {
            if (!visited[v] && distance[v] <= min)
            {
                min = distance[v];
                minIndex = v;
            }
        }

        return minIndex;
    }

    void Start()
    {
        // Define the adjacency matrix (replace this with your adjacency matrix)
        int[,] adjacencyMatrix = matrix.map;

        int startNode = 1; // Starting node index (0-based)
        int endNode = 10;   // Destination node index (0-based)

        // Find shortest path using Dijkstra's algorithm
        List<int> shortestPath = Dijkstra(adjacencyMatrix, startNode, endNode);
        pos = shortestPath;

        // Hide all objects initially
        allObjects = csvObj.placedGameObjects.ToArray();
        foreach (GameObject obj in allObjects)
        {
            obj.SetActive(false);
        }

        // Show only the traversed objects and draw lines between them
        for (int i = 0; i < shortestPath.Count - 1; i++)
        {
            int currentNode = shortestPath[i] - 1;
            int nextNode = shortestPath[i + 1] - 1;

            // Activate current object
            if (currentNode >= 0 && currentNode < allObjects.Length)
            {
                allObjects[currentNode].SetActive(true);
            }

            // Get positions of current and next objects
            Vector3 startPos = allObjects[currentNode].transform.position;
            Vector3 endPos = allObjects[nextNode].transform.position;

            // Draw a line between current and next object positions
            Debug.Log("start:" + startPos.x + " " + startPos.y + " " + startPos.z + "\nEnd:" + endPos.x + " " + endPos.y + " " + endPos.z);
            DrawLine(startPos, endPos); // Pass shortestPath.Count as the number of points
        }

        // If the destination node was reached, print a message
        if (shortestPath.Contains(endNode))
        {
            Debug.Log($"Shortest path from node {startNode} to node {endNode}: {string.Join(" -> ", shortestPath)}");
        }
        else
        {
            Debug.Log($"There is no path from node {startNode} to node {endNode}.");
        }
    }

    // Function to draw a line between two points
    void DrawLine(Vector3 startPos, Vector3 endPos)
    {
        // Set the number of points in the LineRenderer
        lineRenderer.positionCount = pos.Count;

        // Iterate over each position in the sequence
        for (int x = 0; x < pos.Count; x++)
        {
            // Set the position of the point at index x
            lineRenderer.SetPosition(x, allObjects[pos[x] - 1].transform.position);
            allObjects[x].SetActive(false);
        }


    }

}
