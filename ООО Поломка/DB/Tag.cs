using System;
using System.Collections.Generic;

namespace ООО_Поломка.DB;

public partial class Tag
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Color { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
