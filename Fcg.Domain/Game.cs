using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Domain;

public class Game
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Promotion> Promotions { get; set; }

}
