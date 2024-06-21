

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

    connection.on("EstadisticasActualizadas", function (turnos) {
        actualizarEstadisticas(turnos);
    });

    start();

    // Función para cargar datos iniciales de la API
    async function cargarDatosIniciales() {
        try {
            const response = await fetch('https://BancoMexicoAPI.websitos256.com/api/Turno');
            if (response.ok) {
                const turnos = await response.json();
                actualizarEstadisticas(turnos);
            } else {
                console.error('Error al obtener datos iniciales.');
            }
        } catch (error) {
            console.error('Error al conectar con la API:', error);
        }
    }

    // Función para actualizar las estadísticas y la tabla
    function actualizarEstadisticas(turnos) {
        document.getElementById('turnosAtendidos').innerText = turnos.atendidos;
        document.getElementById('turnosCancelados').innerText = turnos.cancelados;

        const tabla = document.getElementById('turnosTabla');
        tabla.innerHTML = ''; // Limpiar la tabla

        turnos.turnosInf.forEach(item => {
            const row = tabla.insertRow();
            row.insertCell(0).innerText = item.numero;
            row.insertCell(1).innerText = item.cajaNombre;
            row.insertCell(2).innerText = item.estado;
            row.insertCell(3).innerText = new Date(item.fechaAtencion).toLocaleString();
            row.insertCell(4).innerText = new Date(item.fechaTermino).toLocaleString();
            row.insertCell(5).innerText = `${item.tiempo.minutos} minutos ${item.tiempo.segundos} segundos`;
        });
    }

    // Cargar datos iniciales al cargar la página
    window.onload = cargarDatosIniciales;

