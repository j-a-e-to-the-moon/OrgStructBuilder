using System.Collections.Generic;
using System.Linq;
using OrgStructBuilder.OwnershipEngine;

namespace OrgStructBuilder.HierarchyEngine
{
    internal class HierarchyBuilder
    {
        public HierarchyNode Build(int rootId, List<DirectEdgeDTO> edges)
        {
            var node = new HierarchyNode { Name = rootId.ToString() };

            var childIds = edges.Where(e => e.FromId == rootId).Select(e => e.ToId).ToList();

            foreach (var childId in childIds)
            {
                node.Children.Add(Build(childId, edges));
            }

            return node;
        }

        // 지배자가 없는 노드가 여러 개일 때 '기타' 노드로 묶어주는 기능
        public HierarchyNode BuildWithVirtualRoot(
            List<int> rootIds,
            List<DirectEdgeDTO> edges,
            string virtualName = "Others"
        )
        {
            var virtualRoot = new HierarchyNode { Name = virtualName };

            foreach (var rootId in rootIds)
            {
                virtualRoot.Children.Add(Build(rootId, edges));
            }

            return virtualRoot;
        }
    }
}
