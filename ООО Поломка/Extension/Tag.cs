using System.ComponentModel.DataAnnotations.Schema;
using c = System.Windows.Media;


namespace ООО_Поломка.DB
{
    public partial class Tag
    {
        [NotMapped]
        public c.Brush BrushColor 
        { 
            get 
            {
                byte r = Convert.ToByte(Color.Substring(0, 2), 16);
                byte g = Convert.ToByte(Color.Substring(2, 2), 16);
                byte b = Convert.ToByte(Color.Substring(4, 2), 16);
                return new c.SolidColorBrush(c.Color.FromRgb(r,g,b));
            }
        }
    }
}
