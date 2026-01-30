using System;
using System.Collections.Generic;
using System.Text;

namespace OrgStructBuilder
{
    internal interface IOwnershipCalculationStrategy
    {
        double[,] ProcessMatrix(double[,] matrix, OrgStructBuildOption option);
    }
}
