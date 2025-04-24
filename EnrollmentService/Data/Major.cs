using System;
using System.Collections.Generic;

namespace EnrollmentService.Data
{
    public partial class Major
    {
        public Major()
        {
            Learners = new HashSet<Learner>();
        }

        public int MajorId { get; set; }
        public string MajorName { get; set; } = null!;

        public virtual ICollection<Learner> Learners { get; set; }
    }
}
