using System;
using System.Collections.Generic;

namespace ООО_Поломка.DB;

public partial class ProductPhoto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string PhotoPath { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
