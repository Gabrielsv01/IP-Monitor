using System;
using System.Linq;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!EnProcesso)
            {
                EnProcesso = true;
                //Thread ping1 = new Thread(new ThreadStart(Ping1));
                //ping1.Start();
                Servidor servidor1 = new Servidor(
                    "", 
                    "168.195.212.50",
                    32, 
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
                    "",
                    "190.15.120.138",
                    32, 
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
                    "SIGA - ",
                    "10.0.4.12", 
                    32,
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
            System.Windows.Threading.Dispatcher Dispatcher;
            private Rectangle indicador;
            private System.Windows.Controls.Label tempo;
            private System.Windows.Controls.Label nome;
            private System.Windows.Controls.Label semResposta;
            private System.Windows.Controls.Label pacotes;
            private System.Windows.Controls.Label bytes;
            private string link;
            private System.Collections.Generic.List<int> listaTempo;
            private double countTimeout = 0;
            private int countTotalTimeout = 0;
            private int pacotesEnviado = 0;
            private int packetSize;
            private string tag;
            private int calc;


            public Servidor(
                string tag,
                string link, 
                int bytesPacote,
                Rectangle indicador1,
                System.Windows.Controls.Label tempo,
                System.Windows.Controls.Label nome,
                System.Windows.Controls.Label semResposta,
                System.Windows.Controls.Label pacotes,
                System.Windows.Controls.Label bytes,
                Dispatcher dispatcher)
            
            {
                this.tag = tag;
                this.link = link;
                this.packetSize = bytesPacote;
                this.indicador = indicador1;
                this.tempo = tempo;
                this.nome = nome;
                this.semResposta = semResposta;
                this.pacotes = pacotes;
                this.bytes = bytes;      
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
                        resposta = obj.Send(link, 120, packet);

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
                    {


                    }

                    MostrarInfor(this.atraso, this.link);
                    Thread.Sleep(500);

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


                    this.tempo.Content = "Tempo: " + atraso + " ms ";
                    this.semResposta.Content = "Sem resposta: " + countTotalTimeout;
                    this.pacotes.Content = "Enviados: " + pacotesEnviado;
                    this.bytes.Content = "Bytes: " + packetSize;

                    if (tag == "")
                    {
                        nome.Content = "Servidor: " + link;
                    }
                    else { nome.Content = tag + link; }

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
