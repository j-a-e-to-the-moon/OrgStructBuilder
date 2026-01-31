using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgStructBuilder.HierarchyEngine
{
    public class HierarchyNode
    {
        public required string Name { get; set; }
        public List<HierarchyNode> Children { get; set; } = new List<HierarchyNode>();
    }
}
