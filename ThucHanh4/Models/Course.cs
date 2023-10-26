using System.ComponentModel.DataAnnotations.Schema;

namespace ThucHanh4.Models
{
    public class Course
    {
        public Course()
        {
            Enrollments = new List<Enrollment>();   
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
