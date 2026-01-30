namespace OrgStructBuilder
{
    public class OrgStructBuildOption
    {
        // 특정 비율 이상이면 100%로 간주할 기준 (예: 0.5)
        public double FullControlThreshold { get; set; } = 1.0;
        public bool UseFullControlLogic => FullControlThreshold < 1.0;
    }
}