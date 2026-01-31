using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace OrgStructBuilder.OwnershipEngine
{
    internal class MatrixConvergenceEngine
    {
        private const int MaxIterations = 1000;
        private const double Epsilon = 1e-8;

        public Matrix<double> Execute(Matrix<double> W, int n)
        {
            var R = W.Clone();
            var R_for_mul = Matrix<double>.Build.Dense(n, n);

            for (int iter = 0; iter < MaxIterations; iter++)
            {
                var R_old = R.Clone();

                // 대각 성분을 1로 치환하여 (Rh + I) 효과 유도
                R.CopyTo(R_for_mul);
                for (int i = 0; i < n; i++)
                    R_for_mul[i, i] = 1.0;

                var R_new = R_for_mul * W;

                if ((R_new - R_old).PointwiseAbs().InfinityNorm() < Epsilon)
                    return R_new;

                R = R_new;
            }
            return R;
        }
    }
}
