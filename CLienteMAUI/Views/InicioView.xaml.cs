using CLienteMAUI.ViewModel;

namespace CLienteMAUI.Views;

public partial class InicioView : ContentPage
{
    public InicioView()
    {
        InitializeComponent();

        //barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        //{
        //    Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
        //    AutoRotate = true,
        //    Multiple = true
        //};

        //this.Loaded += InicioView_Loaded;
    }

    //private void InicioView_Loaded(object? sender, EventArgs e)
    //{
    //    boton.IsEnabled = true;
    //    barcodeReader.IsDetecting = true;

    //}

    //private readonly string key = "ClienteServidor2024";
    //private void Button_Clicked(object sender, EventArgs e)
    //{
    //    escanearCodigo.IsVisible = true;
    //    barcodeReader.IsDetecting = true;
    //}

    //bool escaneado = false;
    //private void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    //{
    //    var vm = this.BindingContext as TurnoViewModel;
    //    if (vm == null)
    //        return;

    //    var barcode = e.Results.FirstOrDefault();
    //    if (barcode is null)
    //        return;
    //    MainThread.BeginInvokeOnMainThread(() =>
    //    {
    //        if (barcode.Value == key)
    //        {
    //            if (!escaneado)
    //            {
    //                vm.GenerarTurnoCommand.Execute(null);
    //                escaneado = true;
    //            }
    //            escanearCodigo.IsVisible = false;
    //            barcodeReader.IsDetecting = false;
    //            return;

    //        }
    //    });

    }
