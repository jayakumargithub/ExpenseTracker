using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Entities
{
    public partial class ExpenseGroupStatus
    {
        public ExpenseGroupStatus()
        {
            ExpenseGroups = new HashSet<ExpenseGroup>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public virtual ICollection<ExpenseGroup> ExpenseGroups { get; set; }
    }
}
