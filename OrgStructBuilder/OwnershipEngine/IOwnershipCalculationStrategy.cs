using System;
using System.Collections.Generic;
using System.Text;

namespace OrgStructBuilder.OwnershipEngine
{
    internal interface IOwnershipCalculationStrategy
    {
        double[,] ProcessMatrix(double[,] matrix, OwnershipBuildOption option);
    }
}
