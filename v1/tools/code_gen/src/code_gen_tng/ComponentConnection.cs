namespace SchematicScriptCreator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ComponentConnection
    {
        public String PinName { get; set; }
        public String NetName { get; set; }

        public ComponentConnection(String pinName, String netName)
        {
            this.PinName = pinName;
            this.NetName = netName;
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}", PinName, NetName);
        }
    }
}
