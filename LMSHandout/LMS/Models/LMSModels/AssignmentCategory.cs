using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategory
    {
        public AssignmentCategory()
        {
            Assignments = new HashSet<Assignment>();
        }

        public uint AcId { get; set; }
        public byte GradingWeight { get; set; }
        public string AcName { get; set; } = null!;
        public uint ClassId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
