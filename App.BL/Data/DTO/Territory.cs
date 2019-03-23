using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.BL.Data.DTO
{
    [Table("Territories")]
    public class Territory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(20)]
        public string TerritoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string TerritoryDescription { get; set; }

        [ForeignKey("Region")]
        public int RegionID { get; set; }

        public virtual Region Region { get; set; }
    }
}
