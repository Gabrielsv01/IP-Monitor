using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IP_Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static bool EnProcesso = false;
        static bool IpRequests = false;
        private DadosIPS dados;

        public class DadosIPS
        {
            public string tagServidor1 { get; set; }
            public string Servidor1 { get; set; }
            public int bytes1 { get; set; }
            public int tempoResposta1 { get; set; }
            public int pingMS1 { get; set; }


            public string tagServidor2 { get; set; }
            public string Servidor2 { get; set; }
            public int bytes2 { get; set; }
            public int tempoResposta2 { get; set; }
            public int pingMS2 { get; set; }

            public string tagServidor3 { get; set; }
            public string Servidor3 { get; set; }
            public int bytes3 { get; set; }
            public int tempoResposta3 { get; set; }
            public int pingMS3 { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var requisicaoWeb = WebRequest.CreateHttp("https://drive.google.com/uc?id=13rCaWszlOG-orveRo5pSPDCh2j74sKJU&export=download");
            
            requisicaoWeb.Method = "GET";
            requisicaoWeb.UserAgent = "RequisicaoWebDemo";


            try
            {
                using (var resposta = requisicaoWeb.GetResponse())
                {
                    var streamDados = resposta.GetResponseStream();
                    StreamReader reader = new StreamReader(streamDados);
                    object objResponse = reader.ReadToEnd();

                    dados = JsonConvert.DeserializeObject<DadosIPS>(objResponse.ToString());
                    MessageBox.Show("IPs Atualizados com Sucesso! Pressione OK ");

                    streamDados.Close();
                    resposta.Close();

                    IpRequests = true;

                }

            }
            catch { MessageBox.Show("Falha ao atualizar IPs \n Verifique sua conexão com a internet"); }




            if (!EnProcesso && IpRequests)
            {
                EnProcesso = true;

                Servidor servidor1 = new Servidor(
                    dados.tagServidor1,
                    dados.Servidor1,
                    dados.bytes1,
                    dados.tempoResposta1,
                    dados.pingMS1,
                    Indicador1,
                    tempo1,
                    nome1,
                    semResposta1,
                    pacotes1,
                    bytes1,
                    this.Dispatcher);
                Thread ping1 = new Thread(new ThreadStart(servidor1.PingTeste));
                ping1.Start();

                Servidor servidor2 = new Servidor(
                    dados.tagServidor2,
                    dados.Servidor2,
                    dados.bytes2,
                    dados.tempoResposta2,
                    dados.pingMS2,
                    Indicador2,
                    tempo2,
                    nome2,
                    semResposta2,
                    pacotes2,
                    bytes2,
                    this.Dispatcher);
                Thread ping2 = new Thread(new ThreadStart(servidor2.PingTeste));
                ping2.Start();

                Servidor servidor3 = new Servidor(
                    dados.tagServidor3,
                    dados.Servidor3,
                    dados.bytes3,
                    dados.tempoResposta3,
                    dados.pingMS3,
                    Indicador3,
                    tempo3,
                    nome3,
                    semResposta3,
                    pacotes3,
                    bytes3,
                    this.Dispatcher);
                Thread ping3 = new Thread(new ThreadStart(servidor3.PingTeste));
                ping3.Start();
            }
        }


        public class Servidor
        {

            private int atraso;
            private int tempoResposta;
            private int pingMS;
            System.Windows.Threading.Dispatcher Dispatcher;
            private Rectangle indicador;
            private System.Windows.Controls.Label labelTempo;
            private System.Windows.Controls.Label labelnome;
            private System.Windows.Controls.Label labelsemResposta;
            private System.Windows.Controls.Label labelpacotes;
            private System.Windows.Controls.Label labelbytes;
            private string link;
            private System.Collections.Generic.List<int> listaTempo;
            private double countTimeout = 0;
            private int countTotalTimeout = 0;
            private int pacotesEnviado = 0;
            private int packetSize;
            private string tag;
            private int calc;
            private int limitePerdaVariavel = 0;


            public Servidor(
                string tag,
                string link,
                int bytesPacote,
                int tempoResposta,
                int pingMS,
                Rectangle indicador1,
                System.Windows.Controls.Label labelTempo,
                System.Windows.Controls.Label labelnome,
                System.Windows.Controls.Label labelsemResposta,
                System.Windows.Controls.Label labelpacotes,
                System.Windows.Controls.Label labelbytes,
                Dispatcher dispatcher)

            {
                this.tag = tag;
                this.link = link;
                this.packetSize = bytesPacote;
                this.tempoResposta = tempoResposta;
                this.pingMS = pingMS;
                this.indicador = indicador1;
                this.labelTempo = labelTempo;
                this.labelnome = labelnome;
                this.labelsemResposta = labelsemResposta;
                this.labelpacotes = labelpacotes;
                this.labelbytes = labelbytes;
                this.Dispatcher = dispatcher;
                this.listaTempo = new System.Collections.Generic.List<int>();
            }

            public void PingTeste()
            {
                Ping obj = new Ping();
                while (EnProcesso)
                {

                    PingReply resposta;

                    try
                    {
                        byte[] packet = new byte[packetSize];
                        resposta = obj.Send(link, tempoResposta, packet);

                        this.atraso = (int)resposta.RoundtripTime;
                        this.pacotesEnviado += 1;

                        if (resposta.Status.ToString() == "Success")
                        {
                            this.listaTempo.Add(this.atraso);
                        }
                        else if (resposta.Status.ToString() == "TimedOut")
                        {
                            this.countTimeout += 1;
                            this.countTotalTimeout += 1;
                            this.listaTempo.Add(this.atraso);
                        }
                    }
                    catch
                    {}

                    MostrarInfor(this.atraso, this.link);
                    Thread.Sleep(this.pingMS);

                }
            }

            private void MostrarInfor(int atraso, string link)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (countTimeout != 0)
                    {
                        calc = (int)(listaTempo.Sum() + (listaTempo.Sum() * (countTimeout / 6)));
                    }
                    else
                    {
                        calc = listaTempo.Sum();
                    }


                    this.labelTempo.Content = "Tempo: " + atraso + " ms ";
                    this.labelsemResposta.Content = "Sem resposta: " + countTotalTimeout;
                    this.labelpacotes.Content = "Enviados: " + pacotesEnviado;
                    this.labelbytes.Content = "Bytes: " + packetSize;

                    if (tag == "" || tag == " " || tag == "null" || tag == " null " || tag == "nulo" || tag == " nulo ")
                    {
                        labelnome.Content = "Servidor: " + link;
                    }
                    else { labelnome.Content = tag + link; }

                    // Criando a rotação
                    RotateTransform myRotateTransform = new RotateTransform();
                    // Criando o grupo de rotação
                    TransformGroup myTransformGroup = new TransformGroup();
                   

                    if ((listaTempo.Count == 5) && (((listaTempo.Sum() / listaTempo.Count) > 180) || (listaTempo.Sum() == 0)))
                    {
                        // Definindo a rotação
                        myRotateTransform.Angle = 175;

                        // Adicionando ao grupo de rotação
                        myTransformGroup.Children.Add(myRotateTransform);

                        // aplicando a rotação
                        indicador.RenderTransform = myTransformGroup;

                        if (this.limitePerdaVariavel == 1)
                        {
                            this.limitePerdaVariavel = 0;
                            EnProcesso = false;
                            MessageBox.Show("Verifique sua conexão com a internet! \n Falha ao conectar com o servidor! ");

                        }
                        else
                        {
                            this.limitePerdaVariavel += 1;
                        }

                    }
                    else if (listaTempo.Count > 5)
                    {
                        // resetando a lista e o countTimeout
                        listaTempo = new System.Collections.Generic.List<int>();
                        this.countTimeout = 0;

                    }
                    else if (listaTempo.Count == 5)
                    {

                        // Definindo a rotação
                        myRotateTransform.Angle = calc / (listaTempo.Count - 1);

                        // adicionado o grupo de rotação
                        myTransformGroup.Children.Add(myRotateTransform);

                        // aplicando a rotação
                        indicador.RenderTransform = myTransformGroup;

                    }

                   

                }));
            }

        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EnProcesso = false;


        }
    }
}
