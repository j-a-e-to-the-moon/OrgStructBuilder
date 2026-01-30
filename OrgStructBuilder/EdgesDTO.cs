namespace OrgStructBuilder
{
    public class EdgesDTO
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public double DIrectOwnershipPercent { get; set; }
        public double IndirectOwnershipPercent => TotalOwnershipPercent - DIrectOwnershipPercent;
        public double TotalOwnershipPercent { get; set; }
    }

    public class DirectEdgeDTO
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public double OwnershipPercent { get; set; }
    }
}