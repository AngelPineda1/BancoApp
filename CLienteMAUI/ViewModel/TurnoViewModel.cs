using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Maui.Alerts;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Net;
using BancoAPI.Models.Enum;

namespace CLienteMAUI.ViewModel
{
    public partial class TurnoViewModel : ObservableObject
    {

        [ObservableProperty]
        private TurnoDto turno = new();

        [ObservableProperty]
        private string estadoTurno = "10 minutos";
        public ObservableCollection<CajasDto2> Cajas { get; set; } = [];

        private readonly HubConnection _hubConnection;
        private readonly string _url = "https://BancoMexicoAPI.websitos256.com/turnosHub";

        public TurnoViewModel()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_url)
                .WithAutomaticReconnect()
                .Build();


            _hubConnection.On<TurnoDto, List<CajasDto2>>("TurnoGenerado", (turnodto, cajas) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Turno = turnodto;
                    OnPropertyChanged(nameof(Turno));
                    
                    if (Turno.Proximo)
                        EstadoTurno = "Su turno está próximo";

                    Cajas.Clear();
                    foreach (var item in cajas)
                    {
                        item.Nombre = item.Nombre.ToUpper();
                        Cajas.Add(item);
                    }

                });
            });



            _hubConnection.On<TurnoDto, List<CajasDto2>, int, int>("TurnoAtendido", (turno, cajas, idcaj, numeroFuturo) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Turno.Numero == turno.Numero)
                    {
                        Turno.Estado = turno.Estado;
                        Turno.CajaNombre = turno.CajaNombre;
                        OnPropertyChanged(nameof(Turno));
                    }

                    if (Turno.Numero == numeroFuturo)
                    {
                        EstadoTurno = "Su turno está próximo";
                    }

                    Cajas.Clear();

                    foreach (var item in cajas)
                    {
                        item.Nombre = item.Nombre.ToUpper();
                        Cajas.Add(item);
                    }
                });
            });


            _hubConnection.On<List<CajasDto2>, int>("CajaDesconectada", (cajas, turnoSiguiente) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Turno.Numero == turnoSiguiente)
                        EstadoTurno = "Su turno está próximo";

                    Cajas.Clear();
                    foreach (var item in cajas)
                    {
                        item.Nombre = item.Nombre.ToUpper();
                        Cajas.Add(item);
                    }
                });
            });

            Task.Run(async () =>
            {
                await _hubConnection.StartAsync();
            });

        }




        [RelayCommand]
        public async Task GenerarTurno(string vista)
        {
            try
            {

                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    await MostrarToast("No hay conexión a internet");
                    return;
                }

                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    await MostrarToast("No se pudo conectar al servidor");
                    return;
                }

                await _hubConnection.InvokeAsync("GenerarTurno");

                await Shell.Current.GoToAsync($"//{vista}");

            }
            catch (Exception ex)
            {
                await MostrarToast(ex.Message);
            }
        }



        private async Task MostrarToast(string mensaje)
        {
            await Toast.Make(mensaje).Show();
        }


        [RelayCommand]
        public async Task GraciasPorSuPreferencia()
        {
            await Shell.Current.GoToAsync($"//Gracias");
        }

    }
}
