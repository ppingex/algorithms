using System;
using System.Collections.Generic;

namespace basaker
{
    class Program
    {
        const int potok = 6;
        const int V = 7; //количество вершин в графе
        static int[,] bandwidth; //матрица пропускных способностей
        static int[,] cost; //матрица стоимостей
        static int[,] flow; //матрица пропущенного потока
        static void Main(string[] args)
        {
            bandwidth = new int[,] { {0, 4, 0, 4, 3, 0, 0},
                                     {0, 0, 0, 0, 0, 0, 3},
                                     {0, 4, 0, 0, 0, 0, 4},
                                     {0, 0, 3, 0, 0, 2, 0},
                                     {0, 0, 0, 0, 0, 4, 0},
                                     {0, 0, 0, 0, 0, 0, 3},
                                     {0, 0, 0, 0, 0, 0, 0} };

            cost = new int[,] { {0, 2, 0, 1, 1, 0, 0},
                                {0, 0, 0, 0, 0, 0, 4},
                                {0, 2, 0, 0, 0, 0, 1},
                                {0, 0, 2, 0, 0, 3, 0},
                                {0, 0, 0, 0, 0, 2, 0},
                                {0, 0, 0, 0, 0, 0, 1},
                                {0, 0, 0, 0, 0, 0, 0} };
            Basaker(potok);
        }
        public static void Basaker(int potok)
        {
            flow = new int[V, V];
            for (int i = 0; i < V; i++)
            {
                for (int j = 0; j < V; j++)
                {
                    flow[i, j] = 0;
                }
            }
            List<int> way = new List<int>();
            do
            {
                int currentFlow = 0;
                int[,] modcost = new int[V, V];
                for (int i = 0; i < V; i++)
                {
                    for (int j = 0; j < V; j++)
                    {
                        if (flow[i, j] == 0 && modcost[i, j] == 0) //если поток по ребру равен 0, оставляем без изменений
                        {
                            modcost[i, j] = cost[i, j];
                        }
                        else if (flow[i, j] == bandwidth[i, j] && modcost[i, j] == 0) //если поток по ребру максимальный, изменяем направлене ребра и изменяем знак стоимости
                        {
                            modcost[j, i] = -cost[i, j];
                        }
                        else if (flow[i, j] < bandwidth[i, j] && modcost[i, j] == 0) //если поток по ребру не максимальный, добавляем обратное ребро с отрицательной стоимостью
                        {
                            modcost[i, j] = cost[i, j];
                            modcost[j, i] = -cost[i, j];
                        }
                    }
                }
                way = FloydWarshall(modcost); //поиск пути и запись его в список
                UpFlow(way); // насыщение потока
                for (int j = 0; j < V; j++)
                {
                    currentFlow += flow[0, j];
                }
                if (currentFlow >= potok)
                {
                    OutPutFlow(currentFlow); // вывод текущего потока
                    OveralCost(); //вывод стоимости
                    break;
                }
                else
                {
                    Console.WriteLine($"Текущий поток {currentFlow} < {potok} => продолжаем");
                    Console.WriteLine();
                }
            } while (way.Count != 0);
        }

        public static void OutPutFlow(int currentFlow)
        {
            Console.WriteLine("Текущий поток на сети равен {0}", currentFlow);
        }

        public static void OveralCost()
        {
            int ocost = 0;
            for (int i = 0; i < V; i++)
            {
                for (int j = 0; j < V; j++)
                {
                    ocost += flow[i, j] * cost[i, j];
                }
            }
            Console.WriteLine("Стоимость пути равна {0}", ocost);
        }

        public static void UpFlow(List<int> way) //насыщение потока
        {
            int maxflow = 1000; //полагаем минимальный допустимый пропускаемый поток бесконечности
            for (int i = 0; i < way.Count - 1; i++)
            {
                if (bandwidth[way[i], way[i + 1]] == 0) //если нет пропущенного потока, то берем его f
                {
                    if (maxflow > flow[way[i + 1], way[i]])
                    {
                        Console.WriteLine("{0} -> {1} = {2}", way[i] + 1, way[i + 1] + 1, flow[way[i + 1], way[i]]);
                    }
                }
                else if (maxflow > bandwidth[way[i], way[i + 1]] - flow[way[i], way[i + 1]]) //если есть пропущенный поток, то берем f-c
                {
                    Console.WriteLine("{0} -> {1} = {2}", way[i] + 1, way[i + 1] + 1, bandwidth[way[i], way[i + 1]] - flow[way[i], way[i + 1]]);
                }
            }
            for (int i = 0; i < way.Count - 1; i++)
            {
                if (bandwidth[way[i], way[i + 1]] == 0) //если нет пропущенного потока, то берем его f
                {
                    if (maxflow > flow[way[i + 1], way[i]])
                    {
                        maxflow = flow[way[i + 1], way[i]];
                    }
                }
                else if (maxflow > bandwidth[way[i], way[i + 1]] - flow[way[i], way[i + 1]]) //если есть пропущенный поток, то берем f-c
                {
                    maxflow = bandwidth[way[i], way[i + 1]] - flow[way[i], way[i + 1]];
                }
            }
            if (maxflow != 1000)
            {
                Console.WriteLine("Насыщаем поток на {0}", maxflow);
            }

            for (int i = 0; i < way.Count - 1; i++)
            {
                if (bandwidth[way[i], way[i + 1]] != 0) //если ребро прямое прибавляем минимальный поток
                {
                    flow[way[i], way[i + 1]] += maxflow;
                }
                else //иначе отнимаем минимальный поток
                {
                    flow[way[i + 1], way[i]] -= maxflow;
                }
            }
        }
        public static List<int> FloydWarshall(int[,] modcost) //Нахождение кратчайшего пути Методом Флойда-Уоршелла
        {
            List<int> way = new List<int>();
            int[,] distance = new int[V, V];
            int[,] costF = new int[V, V];

            for (int i = 0; i < V; ++i)
            {
                for (int j = 0; j < V; ++j)
                {
                    if (modcost[i, j] == 0)
                    {
                        distance[i, j] = 1000;
                    }
                    else
                    {

                        distance[i, j] = modcost[i, j];
                    }

                }
            }
            for (int i = 0; i < V; i++)
            {
                for (int j = 0; j < V; j++)
                {
                    costF[i, j] = j;
                }
            }
            for (int k = 0; k < V; ++k)
            {
                for (int i = 0; i < V; ++i)
                {
                    for (int j = 0; j < V; ++j)
                    {

                        if (distance[i, k] + distance[k, j] < distance[i, j])
                        {
                            distance[i, j] = distance[i, k] + distance[k, j];
                            costF[i, j] = costF[i, k];
                        }
                    }
                }
            }

            if (distance[0, V - 1] != 1000)
            {
                Console.WriteLine("Минимальный путь равен {0}", distance[0, V - 1]);
                way = FindWay(costF, distance);
                Console.WriteLine();
            }
            return way;
        }

        public static List<int> FindWay(int[,] costF, int[,] distance)
        {
            int currentNode = 0;
            List<int> way = new List<int>();
            way.Add(0);
            while (currentNode != V - 1)
            {
                way.Add(costF[currentNode, V - 1]);
                currentNode = costF[currentNode, V - 1];
            }
            foreach (int node in way)
            {
                Console.Write("{0} ", node + 1);
            }
            return way;
        }
    }
}
