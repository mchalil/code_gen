using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ls_code_gen
{
    /*
    * Use a breadth first method to find a path from source vertex s to target vertex v in a directed
    * graph (Digraph).  See a previous post for code for Digraph.  We also add a new array, _distTo (compared
    * to DepthFirstDirectedPaths, to keep track of the length of the shortest paths.
    * */
    public class BreadthFirstDirectedPaths
    {
        private static readonly Int32 INFINITY = Int32.MaxValue;
        private Boolean[] _marked;  //keep track of whether vertex v is reachable from s
        private int[] _edgeTo;      //keep track of the last edge on path s to v
        private int[] _distTo;      //keep track of length of shortest s->v path

        //Single source breadth first search
        public BreadthFirstDirectedPaths(Digraph G, int s)
        {
            _marked = new Boolean[G.V()]; //create a boolean array for all vertices
            _distTo = new int[G.V()]; //create a boolean array for all vertices
            _edgeTo = new int[G.V()]; //create a boolean array for all vertices
                                      //initialize each element of _distTo to Int32.MaxValue
            for (int v = 0; v < G.V(); v++) _distTo[v] = INFINITY;
            BFS(G, s);
        }

        /*
        * A function to do breadth first search.  In this case we use a for loop rather than a
        * recursive function to find the shortest path from s to v.
        * We start with the source vertex s.  Rather than "fanning out" from each vertex recursively
        * we travel along a single path (in turn) adding connected vertices to the q Queue until
        * all vertices have been reached.
        * We avoid "going backwards" or needlessly looking at all paths by keeping track of 
        * which vertices we've already visited using the _marked[] array.
        * We keep track of how we're moving through the graph (from s to v) using _edgeTo[].
        * We keep track of how far we've traveled using _distTo[w].
        */
        private void BFS(Digraph G, int s)
        {
            /*
                * Helps us keep track of what path to go down.
                * Add each new connected vertex to the end of the 
                * queue.  Once we travel down it
                * remove it from the queue.
                * */
            Queue<Int32> q = new Queue<Int32>();
            _marked[s] = true;
            _distTo[s] = 0;
            q.Enqueue(s);
            while (q.Count > 0)
            {
                int v = q.Dequeue();
                foreach (int w in G.Adj(v))
                {
                    if (!_marked[w])
                    {
                        _edgeTo[w] = v;
                        _distTo[w] = _distTo[v] + 1;
                        _marked[w] = true;
                        q.Enqueue(w);
                    }
                }
            }
        }

        /*
        * In the BFS method we've kept track of the shortest path from s to all connected vertices
        * using the _distTo[] array.
        * */
        public int DistTo(int v)
        {
            return _distTo[v];
        }

        /*
        * In the BFS method we've kept track of vertices connected to the source s
        * using the _marked[] array.
        * */
        public Boolean HasPathTo(int v)
        {
            return _marked[v];
        }

        /*
        * We can find the path from s to v working backwards using the _edgeTo array.
        * For example, if we want to find the path from 3 to 0.  We look at _edgeTo[0] which gives us
        * a vertex, say 2.  We then look at _edgeTo[2] and so on until _edgeTo[x] equals 3 (our 
        * source vertex).
        * */
        public IEnumerable<Int32> PathTo(int v)
        {
            if (!HasPathTo(v)) return null;
            Stack<int> path = new Stack<int>();
            int x;
            for (x = v; _distTo[x] != 0; x = _edgeTo[x])
                path.Push(x);
            path.Push(x);
            return path;
        }
    }
}
