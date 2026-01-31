using MathNet.Numerics.LinearAlgebra;
using OrgStructBuilder.HierarchyEngine;
using OrgStructBuilder.LayoutEngine;
using OrgStructBuilder.LayoutEngine.OrgStructBuilder;
using OrgStructBuilder.OwnershipEngine;

namespace OrgStructBuilder
{
    public class OrgStructBuilder : IOrgStructBuilder
    {
        private readonly MatrixConvergenceEngine _convergenceEngine;
        private readonly FullControlStrategy _fullControlStrategy;
        private readonly LayoutBuilder _layoutEngine;
        private readonly HierarchyBuilder _hierarchyBuilder;

        public OrgStructBuilder()
        {
            _convergenceEngine = new MatrixConvergenceEngine();
            _fullControlStrategy = new FullControlStrategy();
            _layoutEngine = new LayoutBuilder();
            _hierarchyBuilder = new HierarchyBuilder();
        }

        /// <summary>
        /// 1. 특정 노드 간의 직/간접 지분율을 계산합니다.
        /// </summary>
        public EdgesDTO CalculateIndirectOwnership(
            List<DirectEdgeDTO> dtoList,
            int fromId,
            int toId,
            OwnershipBuildOption option
        )
        {
            var context = PrepareMatrixContext(dtoList);
            var W = context.D.Clone();

            if (option.UseFullControlLogic)
            {
                _fullControlStrategy.Apply(W, option);
            }

            var R = _convergenceEngine.Execute(W, context.Size);

            return new EdgesDTO
            {
                FromId = fromId,
                ToId = toId,
                DirectOwnershipPercent = context.D[
                    context.IdToIndex[fromId],
                    context.IdToIndex[toId]
                ],
                TotalOwnershipPercent = R[context.IdToIndex[fromId], context.IdToIndex[toId]],
            };
        }

        /// <summary>
        /// 2. 지배구조 시각화에 필요한 노드별 배치 정보(Tier, LeafCount, Depth)를 분석합니다.
        /// </summary>
        public List<EntityLayoutInfo> GetLayoutAnalysis(List<DirectEdgeDTO> dtoList)
        {
            var context = PrepareMatrixContext(dtoList);

            // 지배 여부 진리값 행렬 (지분 > 0 이면 1)
            var D_bool = context.D.PointwiseSign();
            var R_bool = _convergenceEngine.Execute(D_bool, context.Size).PointwiseSign();

            // 회계사님 전용 로직 엔진 호출
            var leaves = _layoutEngine.CalculateLeafCounts(D_bool, context.Size);
            var tiers = _layoutEngine.CalculateTiers(R_bool);
            var depths = _layoutEngine.CalculateDepths(D_bool, context.Size);

            return context
                .EntityIds.Select(id => new EntityLayoutInfo
                {
                    EntityId = id,
                    Tier = (int)tiers[context.IdToIndex[id]],
                    LeafCount = (int)leaves[context.IdToIndex[id]],
                    Depth = (int)depths[context.IdToIndex[id]],
                })
                .ToList();
        }

        /// <summary>
        /// 3. D3.js 등에서 사용 가능한 트리 구조(Hierarchy)를 생성합니다.
        /// </summary>
        public HierarchyNode BuildHierarchy(
            List<DirectEdgeDTO> dtoList,
            string virtualRootName = "Others"
        )
        {
            // 1. 최상위 노드(지배받지 않는 노드) 식별
            var allTargets = dtoList.Select(e => e.ToId).ToHashSet();
            var rootIds = dtoList
                .Select(e => e.FromId)
                .Distinct()
                .Where(id => !allTargets.Contains(id))
                .ToList();

            // 2. 루트 노드가 없는 경우 (전체 순환 출자 등)
            if (rootIds.Count == 0 && dtoList.Any())
            {
                // 모든 노드 중 첫 번째 노드를 임시 루트로 간주하거나,
                // 전체를 아우르는 '기타' 노드 아래에 모든 독립 노드를 배치합니다.
                var orphanNodes = dtoList.Select(e => e.FromId).Distinct().ToList();
                return _hierarchyBuilder.BuildWithVirtualRoot(
                    orphanNodes,
                    dtoList,
                    virtualRootName
                );
            }

            // 3. 루트 노드가 여러 개인 경우 '기타' 노드로 묶어 단일 루트 보장
            if (rootIds.Count > 1)
            {
                return _hierarchyBuilder.BuildWithVirtualRoot(rootIds, dtoList, virtualRootName);
            }

            // 4. 단일 루트인 경우 그대로 반환
            return _hierarchyBuilder.Build(rootIds.First(), dtoList);
        }

        // 공통 행렬 준비 로직
        private static MatrixContext PrepareMatrixContext(List<DirectEdgeDTO> dtoList)
        {
            var entityIds = dtoList
                .SelectMany(d => new[] { d.FromId, d.ToId })
                .Distinct()
                .OrderBy(id => id)
                .ToList();

            var idToIndex = entityIds
                .Select((id, idx) => (id, idx))
                .ToDictionary(x => x.id, x => x.idx);

            int n = entityIds.Count;
            var D = Matrix<double>.Build.Sparse(n, n);
            foreach (var d in dtoList)
                D[idToIndex[d.FromId], idToIndex[d.ToId]] = d.OwnershipPercent;

            return new MatrixContext
            {
                D = D,
                IdToIndex = idToIndex,
                EntityIds = entityIds,
                Size = n,
            };
        }

        private class MatrixContext
        {
            public required Matrix<double> D { get; set; }
            public required Dictionary<int, int> IdToIndex { get; set; }
            public required List<int> EntityIds { get; set; }
            public int Size { get; set; }
        }
    }
}
