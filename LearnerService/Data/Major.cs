using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LearnerService.Data
{
    public partial class Major
    {
        public Major()
        {
            Learners = new HashSet<Learner>();
        }

        public int MajorId { get; set; }
        public string MajorName { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Learner>? Learners { get; set; }
    }
}
