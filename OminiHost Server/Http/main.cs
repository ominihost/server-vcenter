using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace OminiHost_Server.Http
{
    class main
    {
        TelaLoad telaLoad;
        Inicio telaInicial;

        public HttpListener listener = new HttpListener();
        Thread ServerListener;

        public void startServer(TelaLoad t)
        {
            this.telaLoad = t;
            this.telaInicial = this.telaLoad.TelaInicial;
            
            try
            {
                listener.Prefixes.Add(this.telaLoad.config.httpLink);
                listener.Start();
                telaInicial.Write("HTTP API Server inciado "+ this.telaLoad.config.httpLink);
            }
            catch (HttpListenerException hlex)
            {
                telaInicial.WriteConsole("HTTP SERVER, ERRO: " + hlex.Message);
                telaInicial.WriteConsole("Verifique a configuração e tente iniciar novamente");
                telaInicial.WriteConsole("\nLembrete: O URL deve terminar com /");
                return;
            }
            ServerListen sl = new ServerListen(this.telaLoad);
            ServerListener = new Thread( sl.startListen );
            ServerListener.Start( listener );
        }
        public void stopServer()
        {
            ServerListener.Abort();
            listener.Stop();
        }
    }
}
