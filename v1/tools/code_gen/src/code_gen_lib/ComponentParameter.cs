
namespace code_gen_lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public class ComponentParameter
    {
        public String ParameterName { get; set; }
        public Object ParameterValue { get; set; }

        public ComponentParameter(String paramName, Object paramValue)
        {
            this.ParameterName = paramName;
            this.ParameterValue = paramValue;
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}", ParameterName, ParameterValue);
        }

    }
}
