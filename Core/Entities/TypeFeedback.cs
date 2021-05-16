using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class TypeFeedback
    {
        [Key]
        public int TypeID { get; set; }
        [MaxLength(50)]
        public string TypeName { get; set; }
        public bool IsDelete { get; set; }
        public ICollection<Feedback> MyProperty { get; set; }
    }
}