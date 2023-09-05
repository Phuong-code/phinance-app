using System;
using System.Collections.Generic;

namespace phinance2.Models;

public partial class CryptoPrice
{
    public int Id { get; set; }

    public string Symbol { get; set; } = null!;

    public decimal Price { get; set; }
}
