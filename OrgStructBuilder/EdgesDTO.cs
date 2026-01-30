namespace OrgStructBuilder
{
    public class EdgesDTO
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public double DirectOwnershipPercent { get; set; }
        public double IndirectOwnershipPercent => TotalOwnershipPercent - DirectOwnershipPercent;
        public double TotalOwnershipPercent { get; set; }
    }

    public class DirectEdgeDTO
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public double OwnershipPercent { get; set; }
    }
}