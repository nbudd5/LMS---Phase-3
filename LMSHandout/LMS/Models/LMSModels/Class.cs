using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            EnrollmentGrades = new HashSet<EnrollmentGrade>();
        }

        public uint ClassId { get; set; }
        public byte SemesterYear { get; set; }
        public string SemesterSeason { get; set; } = null!;
        public string Loc { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public uint CourseId { get; set; }
        public string ProfId { get; set; } = null!;

        public virtual Course Course { get; set; } = null!;
        public virtual Professor Prof { get; set; } = null!;
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<EnrollmentGrade> EnrollmentGrades { get; set; }
    }
}
