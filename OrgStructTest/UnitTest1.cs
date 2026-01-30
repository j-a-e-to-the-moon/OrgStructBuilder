using Xunit;
using OrgStructBuilder;
using System.Collections.Generic;

namespace OrgStructBuilder.Tests
{
    public class OrgStructBuilderTests
    {
        [Fact]
        public void Calculate_NormalOwnership_ReturnsExpectedValue()
        {
            // 1. 준비 (Arrange)
            var builder = new OrgStructBuilder();
            var data = new List<DirectEdgeDTO>
            {
                new DirectEdgeDTO { FromId = 1, ToId = 2, OwnershipPercent = 0.5 }, // A -> B (50%)
                new DirectEdgeDTO { FromId = 2, ToId = 3, OwnershipPercent = 0.5 }  // B -> C (50%)
            };
            var option = new OrgStructBuildOption { FullControlThreshold = 1.0 }; // 옵션 미사용

            // 2. 실행 (Act)
            var result = builder.CalculateIndirectOwnership(data, 1, 3, option);

            // 3. 검증 (Assert)
            // 직접 지분은 0, 간접 지분은 0.5 * 0.5 = 0.25 (25%)여야 함
            Assert.Equal(0, result.DIrectOwnershipPercent);
            Assert.Equal(0.25, result.IndirectOwnershipPercent, 4); // 소수점 4자리까지 비교
        }

        [Fact]
        public void Calculate_WithFullControlOption_ReturnsInflatedValue()
        {
            // 1. 준비 (Arrange)
            var builder = new OrgStructBuilder();
            var data = new List<DirectEdgeDTO>
            {
                new DirectEdgeDTO { FromId = 1, ToId = 2, OwnershipPercent = 0.6 }, // A -> B (60%)
                new DirectEdgeDTO { FromId = 2, ToId = 3, OwnershipPercent = 0.4 }  // B -> C (40%)
            };

            // 50% 이상이면 100%로 간주하는 옵션 설정
            var option = new OrgStructBuildOption { FullControlThreshold = 0.5 };

            // 2. 실행 (Act)
            var result = builder.CalculateIndirectOwnership(data, 1, 3, option);

            // 3. 검증 (Assert)
            // A가 B를 60%(>50%) 소유하므로 100%로 간주. 
            // 따라서 A의 C 지분은 1.0 * 0.4 = 0.4 (40%)가 되어야 함.
            Assert.Equal(0.4, result.IndirectOwnershipPercent, 4);
        }

        [Fact]
        public void Calculate_CircularOwnership_ReflectsDirectAndTreasury()
        {
            // 1. 준비 (Arrange)
            var builder = new OrgStructBuilder();
            var data = new List<DirectEdgeDTO>
    {
        new DirectEdgeDTO { FromId = 1, ToId = 2, OwnershipPercent = 0.5 }, // 1 -> 2 (50%)
        new DirectEdgeDTO { FromId = 2, ToId = 1, OwnershipPercent = 0.1 }  // 2 -> 1 (10%)
    };
            var option = new OrgStructBuildOption();

            // 2. 실행 (Act)
            var result1To2 = builder.CalculateIndirectOwnership(data, 1, 2, option);
            var result1To1 = builder.CalculateIndirectOwnership(data, 1, 1, option); // 자기주식 확인

            // 3. 검증 (Assert)
            // 말씀하신 로직에 따르면 1 -> 2의 결과는 50%여야 함 (순환 증폭 없음)
            Assert.Equal(0.5, result1To2.TotalOwnershipPercent, 4);

            // 자기 자신(1 -> 1)에 대한 지분은 5% (0.5 * 0.1)
            Assert.Equal(0.05, result1To1.TotalOwnershipPercent, 4);
        }
    }
}