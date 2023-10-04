namespace ThucHanh4.Models
{
    public class Major
    {
        public int MajorID { get; set; }
        public string MajorName { get; set; }
        public virtual ICollection<Learner> Learners { get; set; }
    }
}
