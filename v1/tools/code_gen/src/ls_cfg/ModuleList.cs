using ls_code_gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ls_cfg
{
    public class ModuleList<T> : List<T>
    {
        public int OutputPinCountCur;
        int OutputPinCountCurMax;
        int OutputPinCountMax;
        int ModuleId;
        public List<Module> aOrderedModules;

        public string ToHeader()
        {
            IEnumerable<string> algos = aOrderedModules.Take(OutputPinCountCur).Select(x => x.AlgoName).Distinct();
            string str1 = "{ " + string.Join<string>("()", algos) + " \n";

            return str1;
        }

        public ModuleList()
             : base()
        {
            ModuleId = 0;
            OutputPinCountCur = 0;
            OutputPinCountCurMax = 0;
            OutputPinCountMax = 1000; // should be fine 
        }
        public ModuleList(int maxBufferCount)
     : base()
        {
            OutputPinCountCur = 0;
            OutputPinCountMax = maxBufferCount; // should be fine 
        }
        public new void Add(T obj)
        {
            if (base.Count < 1000) // or whatever limit            
                base.Add(obj);
            Module m = obj as Module;
            if(m.outputStr != null)
                OutputPinCountCurMax += m.outputStr.Length;
            m.Id = ModuleId++;
        }
        public int addPin2OrdredModules(int pin, Module m)
        {
            if(aOrderedModules == null)
            {
                getOrdredModules(); //allocate 
            }
            aOrderedModules[pin] = m;
            OutputPinCountCur++;
            return OutputPinCountCur;

        }
        public List<Module> getOrdredModules()
        {
            aOrderedModules = new List<Module>(base.Count);

            for (int m =0; m < OutputPinCountMax; m++)
            {
                aOrderedModules.Add(null);
            }
            return aOrderedModules;
        }
}
}
