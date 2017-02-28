namespace SchematicScriptCreator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public class Component
    {
        public String ComponentName { get; set; }
        public String InstanceName { get; set; }
        //    public List<ComponentConnection> InputConnections { get; set; }
        public List<String> InputConnections { get; set; }
        public List<String> OutputConnections { get; set; }
        //  public List<ComponentConnection> OutputConnections { get; set; }
        public List<ComponentParameter> Parameters { get; set; }

        //public Component(String componentName, String instanceName, List<ComponentConnection> inputConnections,  List<ComponentConnection> outputConnections)
        //{
        //    this.ComponentName = componentName;
        //    this.InstanceName = instanceName;
        //    this.InputConnections = inputConnections;
        //    this.OutputConnections = outputConnections;
        //}
        public Component()
        {
            this.ComponentName = string.Empty;
            this.InstanceName = string.Empty;
            this.InputConnections = new List<String>();
            this.OutputConnections = new List<String>();
            this.Parameters = new List<ComponentParameter>();
        }

        public Component(String componentName, String instanceName)
        {
            this.ComponentName = componentName;
            this.InstanceName = instanceName;
            this.InputConnections = new List<String>();
            this.OutputConnections = new List<String>();
            this.Parameters = new List<ComponentParameter>();
        }
    }
}
