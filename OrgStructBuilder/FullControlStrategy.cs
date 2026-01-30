using System;
using System.Collections.Generic;
using System.Text;

namespace OrgStructBuilder
{
    internal class FullControlStrategy : IOwnershipCalculationStrategy
    {
        public double[,] ProcessMatrix(double[,] matrix, OrgStructBuildOption option)
        {
            int n = matrix.GetLength(0);
            double[,] processed = (double[,])matrix.Clone();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    // 옵션에 설정된 임계값(예: 0.5) 이상이면 1.0으로 간주
                    if (processed[i, j] >= option.FullControlThreshold)
                    {
                        processed[i, j] = 1.0;
                    }
                }
            }
            return processed;
        }
    }
}
