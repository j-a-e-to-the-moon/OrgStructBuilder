using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrgStructBuilder
{
    public class OrgStructBuilder : IOrgStructBuilder
    {
        private const int MaxIterations = 1000;

        public EdgesDTO CalculateIndirectOwnership(List<DirectEdgeDTO> dtoList, int fromId, int toId, OrgStructBuildOption option)
        {
            var entityIds = dtoList.SelectMany(d => new[] { d.FromId, d.ToId }).Distinct().OrderBy(id => id).ToList();
            var idToIndex = entityIds.Select((id, idx) => (id, idx)).ToDictionary(x => x.id, x => x.idx);
            int n = entityIds.Count;

            // 1. 직접 지분 행렬 D
            var D = Matrix<double>.Build.Sparse(n, n);
            foreach (var d in dtoList)
                D[idToIndex[d.FromId], idToIndex[d.ToId]] = d.OwnershipPercent;

            // 2. 가공된 지분 행렬 W (Full Control 반영)
            var W = D.Clone();
            if (option.UseFullControlLogic)
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (W[i, j] >= option.FullControlThreshold) W[i, j] = 1.0;
            }

            // 3. 루프 로직: RESULT_new = (RESULT_old ∘ H + I) * W
            var R = W.Clone();
            var I = Matrix<double>.Build.DiagonalIdentity(n);

            // H matrix: 주대각선은 0, 나머지는 1인 행렬
            var H = Matrix<double>.Build.Dense(n, n, 1.0);
            for (int i = 0; i < n; i++) H[i, i] = 0.0;

            double epsilon = Math.Pow(10, -8);
            for (int iter = 0; iter < MaxIterations; iter++)
            {
                var R_old = R.Clone();

                // (R ∘ H) : 주대각성분을 0으로 만든 결과 행렬
                var Rh = R.PointwiseMultiply(H);

                // RESULT_new = (Rh + I) * W
                var R_new = (Rh + I) * W;

                if ((R_new - R_old).PointwiseAbs().InfinityNorm() < epsilon)
                {
                    R = R_new;
                    break;
                }
                R = R_new;
            }

            int fIdx = idToIndex[fromId];
            int tIdx = idToIndex[toId];

            double totalOwnership = R[fIdx, tIdx];
            double directOwnership = D[fIdx, tIdx];

            return new EdgesDTO
            {
                FromId = fromId,
                ToId = toId,
                DirectOwnershipPercent = directOwnership,
                TotalOwnershipPercent = totalOwnership,
            };
        }
    }
}