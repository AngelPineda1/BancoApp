using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLienteMAUI.ViewModel
{
    public partial class TurnoViewModel: ObservableObject
    {







        [RelayCommand]
        public async Task NavegarVista(string vista)
        {
            await Shell.Current.GoToAsync($"//{vista}");

        }


    }
}
