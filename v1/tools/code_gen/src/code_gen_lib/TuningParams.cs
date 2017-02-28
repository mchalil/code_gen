using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace code_gen_lib
{
    public class TuningParams
    {
        public TuningParams()
        {

        }


        public string insertTuningParams_m(string moduleName, string instanceName, string moduleParam)
        {
            string str = "";
            switch (moduleName)
            {
                case "nco_bram":
                    lsNco nco1;
                    nco1 = new lsNco("..\\..\\..\\..\\data\\" + instanceName + ".xml");

                    str += nco1.m(instanceName);
                    break;
                case "cicdec":
                    lsCic cic1;
                    cic1 = new lsCic("..\\..\\..\\..\\data\\" + instanceName + ".xml");
                    str += cic1.m(instanceName);
                    break;
                case "genfiraxi":
                    str += String.Format("{0}.addp('aTuningParams', {{ {{ }}, {{ }} }});\n", instanceName);
                    break;
                default:
                    str += moduleParam;
                    break;
            }
            return str;
        }
        public string insertTuningParams_c(string moduleName, string instanceName, string moduleParam)
        {
            string str = "";
            switch (moduleName)
            {
                case "nco_bram":
                    lsNco nco1;
                    nco1 = new lsNco("..\\..\\..\\..\\data\\" + instanceName + ".xml");
                    str += nco1.c(instanceName, moduleName);
                    break;
                case "cicdec":
                    lsCic cic1;
                    cic1 = new lsCic("..\\..\\..\\..\\data\\" + instanceName + ".xml");
                    str += cic1.c(instanceName, moduleName);
                    break;
                case "genfiraxi":
                    lsFir fir1;
                    fir1 = new lsFir("..\\..\\..\\..\\data\\" + instanceName + ".xml");
                    str += fir1.c(instanceName, moduleName);
                    break;
                default:
                    str += moduleParam;
                    break;
            }
            return str;
        }

    }
}
