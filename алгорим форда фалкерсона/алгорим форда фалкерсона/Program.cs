using System;
using System.Collections.Generic;

namespace алгорим_форда_фалкерсона
{
    public class MaxFlow
    {
        static readonly int V = 7; //Количество вершин в графе
        bool bfs(int[,] rGraph, int s, int t, int[] parent)  // Возвращает true, если есть путь из S в T в остаточном графе
        {
            bool[] visited = new bool[V];
            for (int i = 0; i < V; ++i)
                visited[i] = false;

            List<int> queue = new List<int>();   // Создаём список, помещаем туда стоковую вершинy и помечаем её как посещённую
            queue.Add(s);
            visited[s] = true;
            parent[s] = -1;

            while (queue.Count != 0) // Поиск в ширину
            {
                int u = queue[0];
                queue.RemoveAt(0);
                for (int v = 0; v < V; v++)
                {
                    if (visited[v] == false && rGraph[u, v] > 0)
                    {
                        queue.Add(v);
                        parent[v] = u;
                        visited[v] = true;
                    }
                }
            }
            return (visited[t] == true);
        }

        int FordFulkerson(int[,] graph, int s, int t) 
        {
            int u, v;
            int[,] rGraph = new int[V, V]; 
            for (u = 0; u < V; u++)
                for (v = 0; v < V; v++)
                    rGraph[u, v] = graph[u, v];
            int[] parent = new int[V];  
            int max_flow = 0; 
            while (bfs(rGraph, s, t, parent))   // пока есть путь из истока в сток
            {
                int path_flow = int.MaxValue;
                for (v = t; v != s; v = parent[v])    // Находим мин пропускную способность по ребру и пропускаем по нему макс поток
                {
                    u = parent[v];
                    path_flow = Math.Min(path_flow, rGraph[u, v]);
                }
                for (v = t; v != s; v = parent[v])    // Обновляем остаточные мощности
                {
                    u = parent[v];
                    rGraph[u, v] -= path_flow;
                    rGraph[v, u] += path_flow;
                }
                max_flow += path_flow;
            }
            return max_flow;
        }

        class Program
        {
            static void Main(string[] args)
            {
                int[,] graph = new int[7, 7]
                    { { 0, 2, 0, 7, 5, 0, 0},
                    { 0, 0, 4, 0, 0, 0, 0},
                    { 0, 0, 0, 0, 0, 0, 7},
                    { 0, 6, 9, 0, 0, 1, 1},
                    { 0, 0, 0, 8, 0, 6, 0},
                    { 0, 0, 0, 0, 0, 0, 8},
                    { 0, 0, 0, 0, 0, 0, 0} };
                MaxFlow m = new MaxFlow();
                Console.WriteLine("Максимальный поток = " + m.FordFulkerson(graph, 0, 6));
            }
        }
    }
}
