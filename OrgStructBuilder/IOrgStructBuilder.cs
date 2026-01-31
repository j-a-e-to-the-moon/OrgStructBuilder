using System;
using System.Collections.Generic;
using OrgStructBuilder.OwnershipEngine;

namespace OrgStructBuilder
{
    public interface IOrgStructBuilder
    {
        EdgesDTO CalculateIndirectOwnership(
            List<DirectEdgeDTO> dtoList,
            int fromId,
            int toId,
            OwnershipBuildOption option
        );
    }
}
