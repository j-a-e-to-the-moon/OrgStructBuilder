using MathNet.Numerics.LinearAlgebra;

namespace OrgStructBuilder.OwnershipEngine
{
    internal class FullControlStrategy
    {
        public void Apply(Matrix<double> matrix, OwnershipBuildOption option)
        {
            int n = matrix.RowCount;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    bool isFullControl = option.MoreThanOrEqual
                        ? matrix[i, j] >= option.FullControlThreshold
                        : matrix[i, j] > option.FullControlThreshold;

                    if (isFullControl)
                        matrix[i, j] = 1.0;
                }
            }
        }
    }
}
