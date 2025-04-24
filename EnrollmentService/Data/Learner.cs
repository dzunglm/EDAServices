using System;
using System.Collections.Generic;

namespace EnrollmentService.Data
{
    public partial class Learner
    {
        public Learner()
        {
            Enrollments = new HashSet<Enrollment>();
        }

        public int LearnerId { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstMidName { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public int MajorId { get; set; }

        public virtual Major Major { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
