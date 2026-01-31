using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace OrgStructBuilder.LayoutEngine
{
    internal class LayoutBuilder
    {
        // 1. 단말노드 개수 (Leaf Count)
        public Vector<double> CalculateLeafCounts(Matrix<double> D, int n)
        {
            var T = Vector<double>.Build.Dense(n, 1.0);
            for (int iter = 0; iter < 100; iter++)
            {
                var T_next = D * T;
                for (int i = 0; i < n; i++)
                    if (T_next[i] == 0)
                        T_next[i] = 1.0;
                if ((T_next - T).Norm(2) < 1e-8)
                    break;
                T = T_next;
            }
            return T;
        }

        // 2. 티어 (Tier)
        public Vector<double> CalculateTiers(Matrix<double> R_bool) => R_bool.ColumnSums();

        // 3. 깊이 (Depth)
        public Vector<double> CalculateDepths(Matrix<double> D_bool, int n)
        {
            var depthVec = Vector<double>.Build.Dense(n, 1.0);
            var ones = Matrix<double>.Build.Dense(n, n, 1.0);
            for (int iter = 0; iter < 100; iter++)
            {
                var prev = depthVec.Clone();
                var depthMatrix = Matrix<double>.Build.Dense(n, n, (i, j) => depthVec[j]);
                var nextM = D_bool.PointwiseMultiply(depthMatrix) + ones;
                for (int i = 0; i < n; i++)
                    depthVec[i] = nextM.Row(i).Maximum();
                if ((depthVec - prev).Norm(2) < 1e-8)
                    break;
            }
            return depthVec;
        }
    }
}
