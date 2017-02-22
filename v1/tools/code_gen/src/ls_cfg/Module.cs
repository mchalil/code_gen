using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ls_code_gen
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AlgoName { get; set; }
        public string FullName { get { return AlgoName + "_" + Name; } }
        public List<Int32> inputs;
        public List<Int32> outputs;
        public List<string> inputsPinName;
        public List<string> outputsPinName;
        public string[] inputStr;
        public string[] outputStr;

        public enum eType
        {
            Input,
            Output,
            Regular,
            FWOut,
            FWIn,
            None,
            Source
        };
        public eType type;

        public Module(string name, string algoname, string[] inputs, string[] outputs, Module.eType Pintype)
        {
            Name = name;
            AlgoName = algoname;
            inputStr = inputs;
            outputStr = outputs;
            type = Pintype;

        }
        public Module()
        {
            Name = "none";
            type = eType.None;
        }
        public override string ToString()
        {
            string str1 = "{ " + string.Join<int>(",", inputs) + " }";
            string str2 = "{ " + string.Join<int>(",", outputs) + "}";
            string str =  String.Format("{0,8} : {1,-24} : {2,-24} {3,16} {4,16}", Id, Name, AlgoName, str1, str2);
            return str;
        }
 
        public string ToCode()
        {
            string str1 = "{ " + string.Join<int>(",", inputs) + " }";
            string str2 = "{ " + string.Join<int>(",", outputs) + "}";
            string str = String.Format("{2}( {1}, {3}, {4}); // {0}", Id, Name, AlgoName, str1, str2);
            return str;
        }
        private class BufferTupple
        {
            int nBuffCount;
            int nStride;
            public BufferTupple(int num, int stride)
            {
                nBuffCount = num;
                nStride = stride;
            }
            public override string ToString()
            {
                return String.Format("{{{0}, {1}}}", nBuffCount, nStride);
            }
        }
        public string ToAPIData()
        {
            // string str1 = "{ " + string.Join<int>(",", inputs) + " }";
            //string str2 = "{ " + string.Join<int>(",", outputs) + "}";
            BufferTupple[] IIbuffers = inputs.Select(n => new BufferTupple(n, 1)).ToArray();
            string str1 = "{ " + string.Join<BufferTupple>(",", IIbuffers) + " }";
            BufferTupple[] OObuffers = outputs.Select(n => new BufferTupple(n, 1)).ToArray();
            string str2 = "{ " + string.Join<BufferTupple>(",", OObuffers) + " }";
            // string str = String.Format("{2}( {1}, {3}, {4}); // {0}", Id, Name, AlgoName, str1, str2);
            string str = String.Format("tLsBufferInfo pII_{1}_{2}[] = {3};\ntLsBufferInfo pOO_{1}_{2}[] = {4}; // {0}\n\n", Id, AlgoName, Name, str1, str2);
            return str;
        }
        public string ToAPIDataFWIn(string sch_name)
        {
            string str1 = "";
            if (outputs.Count != 0)
            {
                BufferTupple[] IIbuffers = outputs.Select(n => new BufferTupple(n, 1)).ToArray();
                str1 = "{ " + string.Join<BufferTupple>(",", IIbuffers) + " }";
            }
            else
            {
                str1 = "{{0, 1}}; // null";
            }
            string str = String.Format("tLsBufferInfo pII_{1}_{2}[] = {3}; // {0}\n", Id, Name, sch_name, str1);
            return str;
        }
        public string ToAPIDataFWOut(string sch_name)
        {
            string str1 = "";
            if (inputs.Count != 0)
            {
                BufferTupple[] IIbuffers = inputs.Select(n => new BufferTupple(n, 1)).ToArray();
                str1 = "{ " + string.Join<BufferTupple>(",", IIbuffers) + " }";
            }
            else
            {
                str1 = "{{0, 1}}; // null";
            }
            string str = String.Format("tLsBufferInfo pOO_{1}_{2}[] = {3}; // {0}\n", Id, Name, sch_name, str1);
            return str;
        }
        public string ToAPICode()
        {
            string str = String.Format("\tlss_module_{2} ((void*)&{2}_{1}, pII_{2}_{1}, pOO_{2}_{1}); // {0}\n", Id, Name, AlgoName);
            return str;
        }
        public string ToAPICodeInit()
        {
            string str = String.Format("\tlss_module_{2}_init ((void*)&{2}_{1}); // {0}\n", Id, Name, AlgoName);
            return str;
        }
        public string ToAPIData_m()
        {

            BufferTupple[] IIbuffers = inputs.Select(n => new BufferTupple(n, 1)).ToArray();
            string str1 = "{ " + string.Join<BufferTupple>(",", IIbuffers) + " }";
            BufferTupple[] OObuffers = outputs.Select(n => new BufferTupple(n, 1)).ToArray();
            string str2 = "{ " + string.Join<BufferTupple>(",", OObuffers) + " }";
            // string str = String.Format("tLsBufferInfo pII_{1}[] = {2};\ntLsBufferInfo pOO_{1}[] = {3}; // {0}\n\n", Id, Name, str1, str2);
            // string str = String.Format("set_pointers(obj, \'{2}_{1}\', {3}, {4}); % {0}\n", Id, Name, AlgoName, str1, str2);
            string str = String.Format("{1}_{0}.addp(\'pII\', {2});\n", Name, AlgoName, str1);
            str += String.Format("{1}_{0}.addp(\'pOO\', {2});\n\n", Name, AlgoName, str2);
            //nco_bram_iq_nco_a.addp(sprintf('pII_%s', 'nco_a'), { { 0, 1} });
            //nco_bram_iq_nco_a.addp(sprintf('pOO_%s', 'nco_a'), { { 1, 1},{ 2, 1} });


            return str;
        }
        public string ToAPICode_m()
        {
            // ls_algo.nco_bram_iq(lssys1.nco_bram_iq_nco_a, lssys1.pII_nco_a, lssys1.pOO_nco_a); % 1

            string str = String.Format("\tls_algo.{2} (lssys1.{2}_{1}); % {0}\n", Id, Name, AlgoName);
            return str;
        }
        public string ToParamCode(Schematic.ModuleParam p)
        {
            string str;
            str = p.ToString();
            return str;
        }
        public string printNodes()
        {
            string ss = "in : ";

            return ss;
        }
        public void print()
        {
            Type t = this.GetType();//where obj is object whose properties you need.
            PropertyInfo[] pi = t.GetProperties();
            foreach (PropertyInfo p in pi)
            {
                System.Console.WriteLine(p.Name + "   " + p.GetType().ToString());
            }
        }
    }


}
