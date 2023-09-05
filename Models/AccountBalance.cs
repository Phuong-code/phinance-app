using System;
using System.Collections.Generic;

namespace phinance2.Models;

public partial class AccountBalance
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public decimal? Usd { get; set; }

    public decimal? Btc { get; set; }

    public decimal? Eth { get; set; }

    public decimal? Doge { get; set; }

    public decimal? Xrp { get; set; }

    public decimal? Bnb { get; set; }

    public virtual User? User { get; set; }
}
