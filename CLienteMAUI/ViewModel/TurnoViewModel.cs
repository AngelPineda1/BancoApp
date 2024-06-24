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
        private string estadoTurnoMensaje = "10 minutos";
        public ObservableCollection<CajasDto2> Cajas { get; set; } = [];

        private readonly HubConnection _hubConnection;
        private readonly string _url = "https://BancoMexicoAPI.websitos256.com/turnosHub";

        [ObservableProperty]
        private bool isLoading;

        public TurnoViewModel()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_url)
                .WithAutomaticReconnect()
                .Build();


            _hubConnection.On<TurnoDto, List<CajasDto2>, string, int>("TurnoGenerado", (turnodto, cajas, estadoActual, numeroProximo) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Turno = turnodto;
                    OnPropertyChanged(nameof(Turno));

                    if (Turno.Numero == numeroProximo)
                        EstadoTurnoMensaje = "Espere un momento, Su turno es el próximo";
                    else
                    {
                        EstadoTurnoMensaje = estadoActual;
                    }


                    Cajas.Clear();
                    foreach (var item in cajas)
                    {
                        item.Nombre = item.Nombre.ToUpper();
                        Cajas.Add(item);
                    }

                });
            });



            _hubConnection.On<TurnoDto, List<CajasDto2>, int, int>("EstadoTurnoActual", (turno, cajas, idcaj, numeroFuturo) =>
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
                        EstadoTurnoMensaje = "Su turno está próximo";
                    }

                    Cajas.Clear();

                    foreach (var item in cajas)
                    {
                        item.Nombre = item.Nombre.ToUpper();
                        Cajas.Add(item);
                    }
                });
            });


            _hubConnection.On<List<CajasDto2>,int>("ActualizarCajas", (cajas, numeroProximo) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Turno.Numero == numeroProximo)
                        EstadoTurnoMensaje = "Su turno es el próximo";

                    Cajas.Clear();
                    foreach (var item in cajas)
                    {
                        item.Nombre = item.Nombre.ToUpper();
                        Cajas.Add(item);
                    }
                });
            });



            _hubConnection.On<int>("TurnoCancelado", (turnoSiguiente) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Turno.Numero == turnoSiguiente)
                        EstadoTurnoMensaje = "Su turno es el próximo";
                });
            });




            _hubConnection.On<int, int>("TurnoCanceladoCaja", (turnocancelado, turnoproximo) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Turno.Numero == turnocancelado)
                    {
                        Turno.Estado = EstadoTurno.Cancelado.ToString();
                        OnPropertyChanged(nameof(Turno));
                    }


                    if (Turno.Numero == turnoproximo)
                        EstadoTurnoMensaje = "Su turno es el próximo";
                });
            });


            Task.Run(async () =>
            {
                await _hubConnection.StartAsync();
            });

        }



        [RelayCommand]
        public async Task GenerarTurno()
        {
            try
            {
                IsLoading = true;

                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    await MostrarToast("No hay conexión a internet");
                    IsLoading = false;
                    return;
                }



                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    await _hubConnection.StartAsync();
                }

                await _hubConnection.InvokeAsync("GenerarTurno");

                await Shell.Current.GoToAsync($"//Turno");

                IsLoading = false;

            }
            catch (Exception ex)
            {
                IsLoading = false;
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
            await _hubConnection.StopAsync();
            await Shell.Current.GoToAsync($"//Gracias");
        }


        [RelayCommand]
        public async Task CancelarTurno()
        {
            try
            {
                var confirmacion = await Shell.Current.DisplayAlert("Cancelar turno", "¿Está seguro de cancelar su turno?", "Sí", "No");
                if (confirmacion)
                {
                    await _hubConnection.InvokeAsync("CancelarTurno", Turno.Id);
                    await Shell.Current.GoToAsync($"//Inicio");
                }
            }
            catch (Exception ex)
            {
                await MostrarToast(ex.Message);
            }
        }


        [RelayCommand]
        public async Task Regresar()
        {
            try
            {
                await _hubConnection.StopAsync();
                await Shell.Current.GoToAsync($"//Inicio");
            }
            catch(Exception ex)
            {
                await MostrarToast(ex.Message);
            }
        }
    }
}
