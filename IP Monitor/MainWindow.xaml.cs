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
                Servidor servidor1 = new Servidor("168.195.212.50", Indicador1, tempo1, nome1, this.Dispatcher);
                Thread ping1 = new Thread(new ThreadStart(servidor1.PingTeste));
                ping1.Start();

                Servidor servidor2 = new Servidor("190.15.120.138", Indicador2, tempo2, nome2, this.Dispatcher);
                Thread ping2 = new Thread(new ThreadStart(servidor2.PingTeste));
                ping2.Start();

                Servidor servidor3 = new Servidor("10.0.4.12", Indicador3, tempo3, nome3, this.Dispatcher);
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
            private string link;
            private System.Collections.Generic.List<int> listaTempo;


            public Servidor(string link, Rectangle indicador1, System.Windows.Controls.Label tempo, System.Windows.Controls.Label nome, Dispatcher dispatcher)
            {
                this.indicador = indicador1;
                this.tempo = tempo;
                this.nome = nome;
                this.link = link;
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
                        resposta = obj.Send(link, 120);

                        this.atraso = (int)resposta.RoundtripTime;
                        this.listaTempo.Add(this.atraso);
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
                    tempo.Content = "Tempo: " + atraso + " ms ";
                    nome.Content = "Servidor: " + link;

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
                        // resetando a lista
                        listaTempo = new System.Collections.Generic.List<int>();

                    }
                    else if (listaTempo.Count == 5)
                    {

                        // Definindo a rotação
                        myRotateTransform.Angle = listaTempo.Sum() / (listaTempo.Count - 1);

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
