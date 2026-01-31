using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgStructBuilder.LayoutEngine
{
    namespace OrgStructBuilder
    {
        public class EntityLayoutInfo
        {
            public int EntityId { get; set; }
            public int Tier { get; set; } // 레벨 (수직 위치)
            public int LeafCount { get; set; } // 단말노드 개수 (확보할 가로 간격)
            public int Depth { get; set; } // 최대 깊이 (정렬 우선순위)
        }
    }
}
