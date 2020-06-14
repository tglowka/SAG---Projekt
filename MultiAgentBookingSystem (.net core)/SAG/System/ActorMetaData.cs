using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.System
{
    public class ActorMetaData
    {
        public string Name { get; private set; }
        public ActorMetaData Parent { get; set; }
        public string Path { get; private set; }

        public ActorMetaData(string name, ActorMetaData parent = null)
        {
            this.Name = name;
            this.Parent = parent;

            string parentPath = parent != null ? parent.Path : "/user";

            this.Path = string.Format("{0}/{1}", parentPath, this.Name);
        }
    }
}
