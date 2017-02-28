using ls_code_gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ls_cfg
{
#if false
    public class lsGraph
    {
        public Graph graph;
        public Digraph graphModules;
        int bufCount;
        int fwBufCount;
        Dictionary<string , int > moduleLut;
        public lsGraph(ModuleList<Module> modules)
        {

            createGraph(modules);
        }
        public void addNode(Module m, string label)
        {
            BufferManager memManager = new BufferManager(100);

            NodeData data = new NodeData();
            EpForceGraphTest.Buffer buffer = memManager.Alloc(100);
            data.mass = 3.0f;
            data.label = m.Name + label;
            Node newNode;
            if (m.type == Module.eType.Input) newNode = new Node("FwBuf_" + m.Name, data);
            else newNode = new Node("buf" + label, data);
            Node createdNode = graph.AddNode(newNode);

            m.NodesOut.Add(createdNode);
        }
        public void addEdge(Module m, int src, int des)
        {
            EdgeData data = new EdgeData();
            Node desNode = graph.GetIdNode("buf" + des);

            data.label = m.Name + "_" + des + "_" + src.ToString();
            data.length = 60.0f;
            Node srcNode = graph.GetIdNode("buf" + src);
            graph.CreateEdge(srcNode, desNode, data);
        }

        public void addEdge(Module m, int des)
        {
            EdgeData data = new EdgeData();
            Node desNode = graph.GetIdNode("buf" + des);
            Node srcNode = graph.GetIdNode("FwBuf_" + m.Name);
            data.label = m.Name + "_" + des + "_" + "FwBuf_" + m.Name;
            data.length = 60.0f;
            graph.CreateEdge(srcNode, desNode, data);

        }
        public void addEdge(Module m, Node srcNode, Node desNode)
        {
            EdgeData data = new EdgeData();

            data.label = m.Name;
            data.length = 60.0f;
            graph.CreateEdge(srcNode, desNode, data);
        }
        public int createGraph(ModuleList<Module> modules1)
        {
            bufCount = 0;
            fwBufCount = 0;
            graph = new Graph();

            moduleLut = new Dictionary<string, int>();

            foreach (Module m in modules1)
            {
                int ModuleId = 0;
                if (m.outputs != null)
                    foreach (int o in m.outputs)
                    {
                        addNode(m, o.ToString());
                        bufCount++;
                    }
                ModuleId++;
                if (m.type == Module.eType.Source)
                {
                    addNode(m, m.Name);
                    fwBufCount++;
                }
            }

            foreach (Module m in modules1)
            {
                foreach(Node n in m.NodesOut)
                {
                    addEdge(m,);
                }
            }


                Edge e1 = graph.GetEdge("buf0");
            return bufCount + fwBufCount;
        }

        public string prnGraphs()
        {
            string s = "\ngraphs are :\n";
            
            foreach (Node n in graph.nodes)
            {
                s += n.ID + " { " + n.Data.label + "} ";
            }
            s += "\ndgraphs are :\n" + graphModules.ToString();
            
            return s;
        }
        public string prnEdges(int src, int des)
        {
            string s = "edges between points " + src + " and " + des;
            Node n1 = graph.GetIdNode("buf" + src);
            Node n2 = graph.GetIdNode("buf" + des);
            List<Edge> edges = graph.GetEdges(n1, n2);
            foreach(Edge e in edges)
            {
                s += e.Data.label + " "; 
            }
            return s;
        }
#if false
        public IEnumerable<int> printGraphDFToVertex(int v)
        {
            DepthFirstDirectedPaths dgpath1 = new DepthFirstDirectedPaths(graphModules, 0);
            IEnumerable<int> paths1 = dgpath1.PathTo(v);
            string s = dgpath1.connectedEdges.ToString();
            Console.WriteLine(s);
            return paths1;
        }

        public IEnumerable<int> printGraphBFToVertex(int v)
        {
            BreadthFirstDirectedPaths dgpath1 = new BreadthFirstDirectedPaths(graphModules, 0);
            IEnumerable<int> paths1 = dgpath1.PathTo(v);

            return paths1;
        }
#endif
    }
#endif
}
