using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace hva_som_skjer.Models.ManageViewModels
{
    public class ChangePictureViewModel
    {
        public string OldPicture { get; set; }

        public string NewPicture { get; set; }

    }
}