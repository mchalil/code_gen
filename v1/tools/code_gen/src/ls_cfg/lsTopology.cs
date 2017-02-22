using ls_code_gen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ls_cfg
{
    public class lsTopology
    {
        public class Item
        {
            public int Name { get; private set; }
            public Item[] Dependencies { get; private set; }

            public Item(int name, params Item[] dependencies)
            {
                Name = name;
                Dependencies = dependencies;
            }
            public void update(Item[] dependencies)
            {
                Dependencies = dependencies;
            }
            public override string ToString()
            {
                return Name.ToString();// + " + : " + String.Join(",", Dependencies.ToString());
            }
        }

        public static Dictionary<string, Module> getTopOrder(ModuleList<Module> modules, Digraph dg, List<IEnumerable<int>> paths)
        {
            Dictionary<string, Module> dicTopOrder = new Dictionary<string, Module>();
            var pathall = paths[0];
            foreach (var path in paths)
            {
                pathall = pathall.Union(path);
            }
            int maxBuffers = pathall.Max() + 1;
            Item[] buffers = Enumerable.Range(0, maxBuffers).Select(n => new Item(n)).ToArray();
            //pathall.Select(s => new Item(s)).ToArray();

            foreach (int i in pathall)
            {
                var p = dg.Predecessor(i);

                Item[] dep = p.Select(ss => buffers[ss]).ToArray();
                buffers[i].update(dep);
#if debug
                string s = String.Join(",", p);
                Console.WriteLine(i + ": " + s);
#endif
            }
#if debug
             string str = String.Join("\n", sorted);
            Console.WriteLine("[ " + str + " ]");
#endif
            var sorted = lsTopology.Sort(buffers, x => x.Dependencies);
            var modulesTopOrder = Enumerable.Range(0, maxBuffers).Select(n => new Module()).ToArray();

            foreach (var item in sorted) {
                foreach (var m in modules)
                {
                    if(m.outputs != null)
                    if (m.outputs.Contains(item.Name))
                    {
                        Module m1;
                        if (!dicTopOrder.TryGetValue(m.Name, out m1))
                        {
                            dicTopOrder.Add(m.Name, m);
                        }
                        break;
                    }
            }
            }
            return dicTopOrder;
        }

        public static void tst()
        {
            var a = new Item(1);
            var c = new Item(3);
            var f = new Item(2);
            var h = new Item(5);
            var d = new Item(11, a);
            var g = new Item(12, f, h);
            var e = new Item(13, d, g);
            var b = new Item(14, c, e, a);

            var unsorted = new[] { a, b, c, d, e, f, g, h };

            var sorted = lsTopology.Sort(unsorted, x => x.Dependencies);
        }
        public static IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        public static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found.");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}
