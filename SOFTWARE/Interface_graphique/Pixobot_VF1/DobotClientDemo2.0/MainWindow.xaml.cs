using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DobotAPI;

// a été effacé dans le xaml (on sait pas à quoi ça sert)
//xmlns: mc = "http://schemas.openxmlformats.org/markup-compatibility/2006"
//        xmlns: local = "clr-namespace:TermiZord"
//        mc: Ignorable = "d"

namespace DobotClientDemo
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Attention à bien verifier si vous avez branché les accessoires dans le même port que dans le code
        // Vous pouvez changer les port dans le code mais attention à bien verifier le branchement
        // Ne jamais taper une coordonnée manuellement sans être sur que cette coordonnée est accessible par le bras (pas sécurisé)

        // ================================================================================================================================
        //                                                                                         Attributs et constantes de l'application
        #region Attributs et constantes de l'application

        // Entête pour fenêtre de l'application
        private readonly string APP_COPYRIGHT = "©2019-2022 cZord IbouX      ";
        private readonly string APP_NAME_AND_VERSION = "TermiZord Project v1.05";

        private readonly CZordCommucication cZordCommucication;

        private readonly Dobot dobot;
        private readonly DispatcherTimer Timer_Dobot;
        private Accueil frame_Accueil;
        private Config frame_Congig;

        #endregion

        // ================================================================================================================================
        //                                                                                                  Initialisation de l'application
        #region Initialisation de l'application

        public MainWindow()
        {
            InitializeComponent();

            wnd_Pixobot.Title = "     " + APP_COPYRIGHT + "     " + APP_NAME_AND_VERSION + "     ";    // Titre de la fenêtre
            ResizeMode = ResizeMode.CanMinimize;        // Possibilité de réduire l'application
            ShowInTaskbar = true;                       // Icône dans la barre des tâches

            Timer_Dobot = new DispatcherTimer();
            Timer_Dobot.Tick += new EventHandler(Timer_Dobot_Tick);
            Timer_Dobot.Interval = new TimeSpan(100000000); // 100 ms
            Timer_Dobot.Start();

            dobot = Dobot.GetInstance();
            dobot.CreateSuctionCup();

            Cnv_Title_Btn_Dobot(false);

            cZordCommucication = CZordCommucication.GetInstance(cnv_SerialConnect);

            frame_Accueil = new Accueil();
            frame_Congig = Config.getInstance();
            Frame_Main.Content = frame_Accueil;
        }

        #endregion

        // ================================================================================================================================
        //                                                                                                          Cnv et Gestion du Dobot
        #region Gestion du canevas et du Dobot

        private void Timer_Dobot_Tick(object sender, EventArgs e)
        {
            if (dobot.IsConnected)
            {
                if (!dobot.CheckConnection()) // Check si le dobot est toujours connecté chaque 100ms
                {
                    Deconnection();
                    MessageBox.Show("Deconnexion");
                }
            }
        }

        private void Cnv_Title_Btn_Dobot(bool enable)
        {
            if (enable)
            {
                btn_CalibrateDobot.IsEnabled = true;
                btn_RebootDobot.IsEnabled = true;
                btn_emergencyStop.IsEnabled = true;
            }
            else
            {
                btn_CalibrateDobot.IsEnabled = false;
                btn_RebootDobot.IsEnabled = false;
                btn_emergencyStop.IsEnabled = false;
            }
        }

        private void Cnv_Title_MouseDown(object sender, MouseButtonEventArgs e) // Sert à deplacer la page quand on maintient le click de la souris sur le cnv_Title
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Btn_CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_MinimizedApp_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Connection()
        {
            if (!dobot.ArmSetMode(Dobot.ArmModeCmd.Arc)) // Je set le mode ici pour assurer qu'il soit bien dans ce mode au démarrage
            {
                MessageBox.Show("N'as pas pu set le mode au début. Relancez l'application", "ERREUR");
                return;
            }

            btn_ConnectDobot.Background = Brushes.Green;
            btn_ConnectDobot.Content = "Disconnect";
            if (!dobot.TurnOffALL())
            {
                MessageBox.Show("le dobot n'a pas pu redemarrer les objets allumé correctement", "ERROR");
            }
            Cnv_Title_Btn_Dobot(true);
        }

        private void Deconnection()
        {
            if (dobot.IsConnected) // dobot.Disconnect envoie une commande au dobot. si le dobot n'est pas connecté il crash
            {
                if (!dobot.DisconnectALL())
                {
                    MessageBox.Show("Le dobot n'as pas pu éteindre tout les objets allumés, Débranchez l'alim si nécessaire", "FAILURE");
                }
            }
            btn_ConnectDobot.Background = Brushes.Red;
            btn_ConnectDobot.Content = "Connect";
            Cnv_Title_Btn_Dobot(false);
        }

        private void Btn_ConnectDobot_Click(object sender, RoutedEventArgs e)
        {
            if (dobot.CheckConnection())
            {
                Deconnection();
                return;
            }

            dobot.CreateRobotArm();
            dobot.Connect();
            if (!dobot.CheckConnection())
            {
                Deconnection();
                MessageBox.Show("Connexion failed", "FAILURE");
            }
            else
            {
                Connection();
            }
        }

        private void Btn_CalibrateDobot_Click(object sender, RoutedEventArgs e)
        {
            if (!dobot.CheckConnection())
            {
                MessageBox.Show("Connectez le Dobot", "INFO");
                return;
            }
            //dobot.ArmSetCoordCalibrage(); // à mettre quand on veux changer les coordonnées de fin de calibrage (sauvegarder même apres avoir éteind le braas)
            dobot.ArmCalibrage();
        }

        private void Btn_RebootDobot_Click(object sender, RoutedEventArgs e)
        {
            if (!dobot.CheckConnection())
            {
                MessageBox.Show("Connectez le Dobot", "INFO");
                return;
            }

            if (!dobot.Reboot())
            {
                MessageBox.Show("Le rebout n'a pas fonctionné", "FAILURE");
            }
        }

        private void btn_StopUrgence_Click(object sender, RoutedEventArgs e)
        {
            if (!dobot.CheckConnection())
            {
                MessageBox.Show("Connectez le dobot", "INFO");
                return;
            }

            if (!dobot.ArmMove(Dobot.Axe.Idle)) // envoie au Dobot "inactif" pour l'arrêter
            {
                MessageBox.Show("Le bras n'a pas pu s'arrêter", "FAILURE");
            }
        }

        #endregion

        // ================================================================================================================================
        //                                                                             Fermeture de la fenêtre et du port série (si ouvert)
        #region Fermeture de la fenêtre et du port série (si ouvert)

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Message si port connecté
            if (cZordCommucication._cnv_SerialConnect.IsEnabled)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to exit Pixobot ?", "Quit ?",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    cZordCommucication.SerialPortClose();
                }
            }
        }

        #endregion

        // ================================================================================================================================
        //                                                                                                             Changement de frames
        #region Changement de frames

        private void Btn_Config_Click(object sender, RoutedEventArgs e)
        {
            Frame_Main.Content = frame_Congig;
        }

        private void Btn_Accueil_Click(object sender, RoutedEventArgs e)
        {
            Frame_Main.Content = frame_Accueil;
        }

        #endregion

    }
}