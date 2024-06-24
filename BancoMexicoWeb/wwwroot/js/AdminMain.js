const $turnosAtendidos = document.getElementById("turnosAtendidos");
const $turnosCancelados = document.getElementById("turnosCancelados");
const $turnosEspera = document.getElementById("turnosEspera");
const $promedioEspera = document.getElementById("promedioEspera");
const $tabla = document.querySelector("#tabla tbody");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://BancoMexicoAPI.websitos256.com/estadisticasHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
    })
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};




connection.on("OnConnected", function () {
    connection.invoke("Actualizar");
});




connection.on("ActualizarEstadisticas", function (estadisticas, cajas) {

    $promedioEspera.textContent = `${estadisticas.tiempoPromedio} minutos`;
    $turnosAtendidos.textContent = estadisticas.turnosAtendidos;
    $turnosCancelados.textContent = estadisticas.turnosCancelados;
    $turnosEspera.textContent = estadisticas.turnosPendientes;

    $tabla.innerHTML = "";
    for (let i = 0; i < cajas.length; i++) {
        let boton = document.getElementById("btnEliminar").cloneNode(true);
        boton.style.display = "inline-block";
        let td = document.createElement("td");
        td.appendChild(boton);

        var row = $tabla.insertRow();
        row.insertCell().textContent = cajas[i].id;
        row.insertCell().textContent = cajas[i].nombre;
        row.insertCell().textContent = cajas[i].estado == 0 ? "Cerrada" : cajas[i].estado == 1? "Activa": "Ocupada";
        row.insertCell().textContent = cajas[i].numeroActual;
        row.appendChild(td);

    }


});


function eliminarCaja(boton) {
    let tr = boton.parentElement.parentElement;
    let idCajaEliminar = parseInt(tr.children[0].textContent);

    if (isNaN(idCajaEliminar))
        return;

    const url = `/admin/cajeros/editar/${idCajaEliminar}`;
    window.location.href = url;


    //fetch(url, {
    //    method: 'GET'
    //})
    //    .then(response => {
    //        console.log('Success:', response);
    //    })
    //    .catch(error => {
    //        console.error('Error:', error);
    //    });
}























connection.onclose(async () => {
    await start();
});

// Start the connection.
start();