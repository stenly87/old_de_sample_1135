using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ООО_Поломка.DB
{
    public partial class Client
    {
        [NotMapped]
        public int? CountVisit { get => 
                ClientServices?.Count(); }
        [NotMapped]
        public DateTime? LastVisit { get => 
                ClientServices?.LastOrDefault()?.StartTime; }
    }
}
