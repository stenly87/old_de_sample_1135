using System;
using System.Collections.Generic;

namespace ООО_Поломка.DB;

public partial class Service
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public decimal Cost { get; set; }

    public int DurationInSeconds { get; set; }

    public string? Description { get; set; }

    public double? Discount { get; set; }

    public string? MainImagePath { get; set; }

    public virtual ICollection<ClientService> ClientServices { get; set; } = new List<ClientService>();

    public virtual ICollection<ServicePhoto> ServicePhotos { get; set; } = new List<ServicePhoto>();
}
