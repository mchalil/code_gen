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


        public string insertTuningParams(string moduleName, string instanceName)
        {
            string str = "";
            string paramFile = "";
            switch (moduleName)
            {
                case "nco_bram":
                    paramFile = "..\\..\\..\\..\\data\\" + instanceName + ".xml";
                    lsNco nco1;
                    try
                    {
                        nco1 = new lsNco(paramFile);
                        str += String.Format("%Reading from param file {0}; \n", paramFile);
                        str += String.Format("{0}.addp('frequency', {1}); \n", instanceName, nco1.instance.nFrequency);
                        str += String.Format("{0}.addp('amplitude', {1});\n", instanceName, nco1.instance.nAmplitude);
                        str += String.Format("{0}.addp('phstate', 0);\n", instanceName);
                    }
                    catch (Exception ex)
                    {
                        str += String.Format("{0}.addp('frequency', 13.5e6);% default values \n", instanceName);
                        str += String.Format("{0}.addp('amplitude', 1e3);\n", instanceName);
                        str += String.Format("{0}.addp('phstate', 0);\n", instanceName);
                        str += String.Format("%Unable to open param file {0} : {1}", paramFile , ex.Message);
                        return str;
                    }

                    break;
                case "cicdec":
                    lsCic cic1;
                    paramFile = "..\\..\\..\\..\\data\\" + instanceName + ".xml";
                    try
                    {
                        cic1 = new lsCic(paramFile);
                        str += String.Format("%Reading from param file {0}; \n", paramFile);

                        str += String.Format("{0}.addp('cic', ls_cic(obj.tick, {1}, {2}, 1));\n", instanceName, cic1.instance.nRate, cic1.instance.nStage);
                    }
                    catch (Exception ex)
                    {
                        str += String.Format("{0}.addp('cic', ls_cic(obj.tick, 24, 5, 1)); \n", instanceName);
                        str += String.Format("%Unable to open param file {0} : {1}", paramFile, ex.Message);
                    }
                    break;
                case "genfiraxi":
                    str += String.Format("{0}.addp('aTuningParams', {{ {{ }}, {{ }} }});\n", instanceName);
                    break;
            }

            return str;
        }

    }
}
