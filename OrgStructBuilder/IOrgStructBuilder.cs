using System;
using System.Collections.Generic;

namespace OrgStructBuilder
{
    public interface IOrgStructBuilder
    {
        EdgesDTO CalculateIndirectOwnership(List<DirectEdgeDTO> dtoList, int fromId, int toId, OrgStructBuildOption option);
    }
}
