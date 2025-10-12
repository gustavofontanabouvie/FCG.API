using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Domain;

public class Purchase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public decimal PricePaid { get; set; }

    public DateTime PurchaseDate { get; set; }
    public virtual User User { get; set; }

    public virtual Game Game { get; set; }


}
