using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public uint CourseId { get; set; }
        public string CName { get; set; } = null!;
        public ushort CNumber { get; set; }
        public string Abbreviation { get; set; } = null!;

        public virtual Department AbbreviationNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
