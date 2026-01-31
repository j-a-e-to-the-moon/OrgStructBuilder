using System.Collections.Generic;
using OrgStructBuilder.OwnershipEngine;
using Xunit;

namespace OrgStructBuilder.Tests
{
    public class OrgStructBuilderTests
    {
        [Fact]
        public void Calculate_NormalOwnership_ReturnsExpectedValue()
        {
            // 1. Arrange
            var builder = new OrgStructBuilder();
            var data = new List<DirectEdgeDTO>
            {
                new DirectEdgeDTO
                {
                    FromId = 1,
                    ToId = 2,
                    OwnershipPercent = 0.5,
                }, // A -> B (50%)
                new DirectEdgeDTO
                {
                    FromId = 2,
                    ToId = 3,
                    OwnershipPercent = 0.5,
                }, // B -> C (50%)
            };
            // 옵션 클래스명 수정 및 Threshold 1.0 (미사용) 설정
            var option = new OwnershipBuildOption { FullControlThreshold = 1.0 };

            // 2. Act
            var result = builder.CalculateIndirectOwnership(data, 1, 3, option);

            // 3. Assert
            // 오타 수정: DIrectOwnershipPercent -> DirectOwnershipPercent
            Assert.Equal(0, result.DirectOwnershipPercent);
            Assert.Equal(0.25, result.IndirectOwnershipPercent, 4);
        }

        [Fact]
        public void Calculate_WithFullControlOption_ReturnsInflatedValue()
        {
            // 1. Arrange
            var builder = new OrgStructBuilder();
            var data = new List<DirectEdgeDTO>
            {
                new DirectEdgeDTO
                {
                    FromId = 1,
                    ToId = 2,
                    OwnershipPercent = 0.6,
                }, // A -> B (60%)
                new DirectEdgeDTO
                {
                    FromId = 2,
                    ToId = 3,
                    OwnershipPercent = 0.4,
                }, // B -> C (40%)
            };

            // 50% 이상(MoreThanOrEqual=true)이면 100%로 간주
            var option = new OwnershipBuildOption
            {
                FullControlThreshold = 0.5,
                MoreThanOrEqual = true,
            };

            // 2. Act
            var result = builder.CalculateIndirectOwnership(data, 1, 3, option);

            // 3. Assert
            // A -> B가 100%로 간주되므로, A의 C 지분은 1.0 * 0.4 = 0.4 (40%)
            Assert.Equal(0.4, result.TotalOwnershipPercent, 4);
        }

        [Fact]
        public void GetLayoutAnalysis_ReturnsCorrectMetadata()
        {
            // 1. Arrange: A(1) -> B(2) -> C(3) 구조
            var builder = new OrgStructBuilder();
            var data = new List<DirectEdgeDTO>
            {
                new DirectEdgeDTO
                {
                    FromId = 1,
                    ToId = 2,
                    OwnershipPercent = 1.0,
                },
                new DirectEdgeDTO
                {
                    FromId = 2,
                    ToId = 3,
                    OwnershipPercent = 1.0,
                },
            };

            // 2. Act
            var analysis = builder.GetLayoutAnalysis(data);

            // 3. Assert
            var nodeC = analysis.Find(x => x.EntityId == 3);
            Assert.Equal(2, nodeC.Tier); // A(0), B(1), C(2)
            Assert.Equal(1, nodeC.LeafCount); // 단말노드이므로 1
            Assert.Equal(1, nodeC.Depth); // 가장 하단 노드
        }
    }
}
