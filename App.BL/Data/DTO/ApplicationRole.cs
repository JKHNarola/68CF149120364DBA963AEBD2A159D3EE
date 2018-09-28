using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.BL.Data.DTO
{
    public class ApplicationRole : IdentityRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Column(TypeName = "nvarchar(100)")]
        public string DisplayName { get; set; }
    }
}
