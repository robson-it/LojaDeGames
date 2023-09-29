using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BlogPessoal.Model;

namespace LojaDeGames.Model
{
    public class Categoria : Auditable
    {
        [Key] // Primary Key Id
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Identity 1,1
        public long Id { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(250)]
        public string Tipo { get; set; } = string.Empty;

        [InverseProperty("Categoria")]
        public virtual ICollection<Produto>? Produto { get; set; }
    }
}
