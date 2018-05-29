using System.Collections.Generic;
using hva_som_skjer.Models;

namespace hva_som_skjer.Models
{
    public class ClubViewModel
    {
        // Search form
        public string Search { get; set; }
        public int Limit { get; set; }
        public string OrderBy { get; set; }
        public string Type { get; set; }
        public int Founded { get; set; }

        // Results
        public List<ClubModel> Results { get; set; }
    }
}

/*


<table class="table">
  <tr><th>Name</th><th>Category</th><th>Founded</th></tr>
  @foreach (var p in Model.Results)
  {
    <tr><td>@p.Name</td><td>@p.Category</td><td>@p.Founded</td></tr>
  }
</table>
 */