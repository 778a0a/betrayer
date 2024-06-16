using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SavedCountry
{
    public int Id { get; set; }
    public int ColorIndex { get; set; }
    public List<SavedMapPosition> Areas { get; set; }
}

public class SavedCountries
{
    public List<SavedCountry> Countries { get; set; }
}
