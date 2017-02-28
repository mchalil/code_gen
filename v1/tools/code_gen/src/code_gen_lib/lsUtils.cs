using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace code_gen_lib
{
    static class lsUtils
    {
        static public string getPinPrefix(string pinName)
        {
            string[] ss = pinName.Split('-');
            return ss[0];
        }
        static public bool isInConnectionName(string connection, string pinName)
        {
            bool res = false;
            string[] ss = connection.Split('-');
            res = (ss[0].StartsWith("in") || ss[0].StartsWith("axi") || ss[0].StartsWith("param") || ss[0].StartsWith("cfg")) && 
                    ss[1].Contains(pinName);

            return res;
        }
        static public Component getComponentWhichTakesPinAsInput(string pinName, List<Component> Components)
        {
            Component component1 = null;
            foreach (Component component in Components)
            {
                foreach (string ic in component.InputConnections)
                {
                    if (isInConnectionName(ic, pinName))
                    {
                        component1 = component;
                        break;
                    }
                }
            }

            return component1;
        }

    }
}
