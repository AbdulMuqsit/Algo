using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoProject.Models
{
    public static class BellmanFord
    {
        //pass a graph containing directed edges and source of the path to this function
        public static bool MakeBellmanFordTree(List<Edge> graph, Tile source)
        {
            initializeSingalSource(graph, source);

            //for the cardinality of set of vertices in the graph -1 times
            //relax each and every edge in the collection of edges
            for (int i = 0; i < graph.Count-1; i++)
            {
                foreach (Edge edge in graph)
                {
                    relax(edge.U, edge.V, edge.Cost);
                }
            }
            return true;
        }

        //for given two vertices if cost to reach to 2nd vertex from source is greater than the "cost to reach to first vertex from source + the cost to reach to 2nd vertex from 1st vertex " then we have found a new short route
        // so change the cost to reach to 2nd vertex from source to new found cost
        //also set the previous vertex of 2nd Vertex to be 1st vertex
        private static void relax(Tile U, Tile V, int Cost)
        {
            if(V.CostSoFar >  U.CostSoFar + Cost)
            {
                V.CostSoFar = U.CostSoFar + Cost;
                V.Parent = U;
            }
        }


        //to initialize the algorithm maximize the cost to reach to each edge and also set the preceeding vertex of each vertex as null 
        private static void initializeSingalSource(List<Edge> graph, Tile source)
        {
            foreach (Edge edge in graph)
            {
                edge.U.CostSoFar = Double.PositiveInfinity;
                edge.V.CostSoFar = Double.PositiveInfinity;
                edge.U.Parent = null;   
                edge.V.Parent = null;
            }
            //source.Parent = null;
            source.CostSoFar = 0;
        }
    }
}
