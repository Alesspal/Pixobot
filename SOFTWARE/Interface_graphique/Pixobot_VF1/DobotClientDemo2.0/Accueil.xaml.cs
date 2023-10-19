using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DobotAPI;

namespace DobotClientDemo
{
    /// <summary>
    /// Logique d'interaction pour Accueil.xaml
    /// </summary>
    public partial class Accueil : Page
    {

        #region Attributs et constantes de l'application

        private CZordCommucication cZordCommucication;
        private readonly DispatcherTimer tmr_CheckPort;
        private Dobot dobot;
        private Config config;

        private enum Signature
        {
            RED = 1,
            GREEN = 2,
            BLUE = 3,
        }

        const string RED = "(R)";
        const string GREEN = "(G)";
        const string BLUE = "(B)";

        const string INACCESSIBLE = "inaccessible";

        #endregion

        #region Initialisation de la frame

        public Accueil()
        {
            InitializeComponent();

            // Pas besoin de donner le Canvas en paramètre car il a déja été donné dans le mainWindow
            cZordCommucication = CZordCommucication.GetInstance();
            dobot = Dobot.GetInstance();
            dobot.CreateSuctionCup();
            config = Config.getInstance();

            tmr_CheckPort = new DispatcherTimer();
            tmr_CheckPort.Stop();
            tmr_CheckPort.Interval = new TimeSpan(10000000);                // Interval d'appel de l'événement Tick
            tmr_CheckPort.Tick += new EventHandler(TimerCheckPort_Tick);    // Abonnement à l'événement du timer

            tmr_CheckPort.Start();

            // Initialisation de l'interface utilisateur
            // ------------------------------------------------------------------------------------------------------------
            // Lors de la réception de données, ne perds pas le focus du textBox des données à envoyer
            // (sinon problème lors de la réception par timer et maj d'un label alors que l'on saisi des données à envoyer)
            lbl_StatusRxError.Focusable = false;
            // Bouton actif par défaut lorsque l'on presse la touche Enter
            btn_Scan.IsDefault = true;
            btn_Start.IsEnabled = false;
            btn_Scan.TabIndex = 0;
            // Mise à jour de l'interface
            //ClearDisplayRx();
        }

        #endregion

        #region Gestion du Canevas

        private void TimerCheckPort_Tick(object sender, EventArgs e)
        {
            if (cZordCommucication._flagSerialPortConnected)
            {
                if (!cnv_Application.IsEnabled)
                {
                    cnv_Application.IsEnabled = true;
                }
                if (dobot.IsConnected)
                {
                    if (!btn_Start.IsEnabled)
                    {
                        btn_Start.IsEnabled = true;
                        btn_Move.IsEnabled = true;
                    }
                    tbk_Coord.Text = $"X : {dobot.ArmGetX()} Y : {dobot.ArmGetY()} Z : {dobot.ArmGetZ()} R : {dobot.ArmGetRHead()}";
                }

            }
            else
            {
                if (cnv_Application.IsEnabled)
                {
                    ClearDisplay();
                    cnv_Application.IsEnabled = false;
                }
                if (!dobot.IsConnected)
                {
                    if (btn_Start.IsEnabled)
                    {
                        btn_Start.IsEnabled = false;
                        btn_Move.IsEnabled = false;
                        tbk_Coord.Text = "";
                    }
                }
            }
        }

        #endregion

        #region recherche et manipulation des blocs

        // Demande les blocs détéctés à la caméra et attend une réponse pour l'afficher
        private void Btn_Scan_Click(object sender, RoutedEventArgs e)
        {
            if (config.IsConfigured)
            {
                ClearDisplay();
                cZordCommucication.WriteCommand("gnb");  // demande le nombre de blocs
                int nbOfBlocks = 0;
                bool enable = true;
                while (enable)  // boucle qui attend de recevoir le nombre de bloc qu'on a detecté
                {
                    string dataReceived = cZordCommucication.SerialCheckAvailableAndRead();
                    CZordCommucication.Command commandAnalyzed = cZordCommucication.AnalyzeCommand(dataReceived);
                    if (commandAnalyzed == CZordCommucication.Command.ERROR_TIME_OUT
                        || commandAnalyzed == CZordCommucication.Command.ERROR_PORT
                        || commandAnalyzed == CZordCommucication.Command.UNKNOWN_COMMAND)
                    {
                        enable = false;
                    }
                    else if (commandAnalyzed == CZordCommucication.Command.NB_OF_BLOCS_DETECTED)
                    {
                        nbOfBlocks = Convert.ToInt32(dataReceived.Substring(2));
                        enable = false;
                    }
                    ProcessingCommand(commandAnalyzed, dataReceived);
                }
                int cpt = 0;
                if (nbOfBlocks > 0)
                {
                    cZordCommucication.WriteCommand("gba"); // demande les coordonnées des blocs detectés
                }
                while (cpt < nbOfBlocks)    // Boucle qui affiche les coordonnées des blocks
                {
                    string dataReceived = cZordCommucication.SerialCheckAvailableAndRead();
                    CZordCommucication.Command commandAnalyzed = cZordCommucication.AnalyzeCommand(dataReceived);
                    if (commandAnalyzed == CZordCommucication.Command.ERROR_TIME_OUT
                        || commandAnalyzed == CZordCommucication.Command.ERROR_PORT
                        || commandAnalyzed == CZordCommucication.Command.UNKNOWN_COMMAND)
                    {
                        cpt = nbOfBlocks;
                    }
                    else if (commandAnalyzed == CZordCommucication.Command.COORD_BLOC)
                    {
                        cpt++;
                    }
                    ProcessingCommand(commandAnalyzed, dataReceived);
                }
            }
            else
            {
                MessageBox.Show("Configurez la caméra au Dobot et les zones voulues avant de pouvoir scanner", "INFO");
            }
        }

        // Le bras va chercher les blocs séléctionnés
        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (!dobot.IsConnected)
            {
                MessageBox.Show("Connectez le Dobot", "INFO");
                return;
            }

            if (lst_blocksScaned_Taken.Items.Count > 0)
            {
                cZordCommucication.WriteCommand("gt");  // Demande l'état du train
                bool enable = true;
                while (enable)  // Attend de recevoir une commande
                {
                    string dataReceived = cZordCommucication.SerialCheckAvailableAndRead();
                    CZordCommucication.Command commandAnalyzed = cZordCommucication.AnalyzeCommand(dataReceived);
                    if (commandAnalyzed < CZordCommucication.Command.PROCESSING)
                    {
                        enable = false;
                    }
                    else if (commandAnalyzed == CZordCommucication.Command.TRAIN_HERE
                        || commandAnalyzed == CZordCommucication.Command.TRAIN_NOT_HERE
                        || commandAnalyzed == CZordCommucication.Command.TRAIN_STATE_UNKNOWN)
                    {
                        enable = false;
                    }
                    ProcessingCommand(commandAnalyzed, dataReceived);
                }

                if (rct_StatusOfTrain.Fill == Brushes.DarkGreen)
                {
                    for (int i = 0; i < lst_blocksScaned_Taken.Items.Count; i++)
                    {
                        // Inversion
                        float ArmX = Convert.ToSingle(lst_blocksScaned_Taken.Items[i].ToString().Substring(22, 7)); // camY
                        float ArmY = Convert.ToSingle(lst_blocksScaned_Taken.Items[i].ToString().Substring(33, 7));  // camX

                        if (i <= config.trainCharacteristics.nbOfSeat)
                        {
                            config.CalculatingLocationObject(i);
                            dobot.GoCoordinateXYZR(ArmX, ArmY, 45F, 45F);
                            dobot.SuctionCupCatch();
                            dobot.GoCoordinateXYZR(ArmX, ArmY, config.storageCharacteristics.height + (int)Config.ObjectCharacteristics.HEIGHT, 45F);
                            dobot.GoCoordinateXYZR(ArmX, ArmY, 45F, 45F);
                            dobot.GoCoordinateXYZR(config.trainCharacteristics.locationXOfSeat, config.trainCharacteristics.locationYOfSeat, 45F, 45F);
                            dobot.GoCoordinateXYZR(config.trainCharacteristics.locationXOfSeat, config.trainCharacteristics.locationYOfSeat, config.trainCharacteristics.height
                                + (int)Config.ObjectCharacteristics.HEIGHT, 45F);
                            dobot.SuctionCupRelease();
                            dobot.GoCoordinateXYZR(config.trainCharacteristics.locationXOfSeat, config.trainCharacteristics.locationYOfSeat, 45F, 45F);
                        }
                    }
                }
                else if (rct_StatusOfTrain.Fill == Brushes.DarkRed)
                {
                    MessageBox.Show("Le train n'est pas encore arrivé", "INFO");
                }
                else if (rct_StatusOfTrain.Fill == Brushes.DarkOrange)
                {
                    MessageBox.Show("une erreur est survenue", "BUG");
                }
                ClearDisplay();
            }
            else
            {
                MessageBox.Show("Séléctionnez des blocs", "INFO");
            }
        }

        // Déplace le bras pour sortir du champ de vision de la caméra
        private void Btn_Move_Click(object sender, RoutedEventArgs e)
        {
            if (dobot.IsConnected)
            {
                dobot.GoCoordinateXYZR(dobot.ArmGetX(), dobot.ArmGetY(), 45F, 45F); // Monte le bras par sécurité
                dobot.GoCoordinateXYZR(60F, 200F, 45F, 45F);    // coordonnée qui sert à bouger le bras en dehors du champ de vision de la caméra
            }
        }

        // Contrôle les touches pressé quand le TexBox est sélectionné
        private void Tbx_spacing_Object_KeyDown(object sender, KeyEventArgs e)
        {
            // Autorise que les touches des chifres, les chiifres du NumPad et la touche "back" (je sais pas c'est la quel)
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key == Key.Back))
            {
                e.Handled = false;
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        // Sauvegarde de la valeur mise dans le TextBlock dans l'attribut object_Spacing de la frame config
        private void Btn_Save_Object_Spacing_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbx_spacing_Object.Text, out int nb))
            {
                if (nb >= 10 && nb <= 20)
                {
                    config.object_Spacing = nb;
                    config.ConfigurationOfTrainAndStorageChaaracteristics();
                }
                else
                {
                    MessageBox.Show("Mettre une valeur entre 10 et 20", "INFO");
                }
            }
            else
            {
                MessageBox.Show("Conversion en int du TexBlock à échoué", "ERROR");
            }
        }

        #endregion

        #region Gestion de l'interface graphique (ListBox, ClearDisplay)

        // Efface le contenu des labels / textBox
        private void ClearDisplay()
        {
            lst_blocksScaned.Items.Clear();
            lst_blocksScaned_Taken.Items.Clear();
            lbl_StatusRxError.Content = "";
            tbx_NbOfBlocks.Text = "";
        }

        private void Lst_blocksScaned_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool add = true;
            if (lst_blocksScaned.SelectedItem != null)
            {
                // Lis le texte du bloc sélectrionné
                string selectedItem = lst_blocksScaned.SelectedItem.ToString();
                if (lst_blocksScaned_Taken.Items.Count > 0)
                {
                    for (int i = 0; i < lst_blocksScaned_Taken.Items.Count; i++)
                    {
                        // Compare si deux blocs sont les mêmes
                        if (selectedItem == lst_blocksScaned_Taken.Items[i].ToString())
                        {
                            MessageBox.Show("Bloc déja sélectionné", "INFO");
                            return;
                        }
                    }
                }

                // contrôle le nombre de place dans le train
                if (lst_blocksScaned_Taken.Items.Count >= config.trainCharacteristics.nbOfSeat)
                {
                    MessageBox.Show("Plus de place dans le train", "INFO");
                    add = false;
                }

                // contrôle si le bloc sélectionné est inaccesible
                if (selectedItem.Substring(0, 12) == INACCESSIBLE)
                {
                    MessageBox.Show("le bloc n'est pas dans la zone de stockage", "INFO");
                    add = false;
                }

                // ajoute l'item dans l'autre liste boxe, si il y a aucune erreur
                if (add)
                {
                    btn_Start.IsEnabled = true;
                    lst_blocksScaned_Taken.Items.Add(selectedItem);
                }
            }
        }

        private void Lst_blocksScaned_Taken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lst_blocksScaned_Taken.SelectedItem != null)
            {
                // désélectionne l'objet, pour qu'a la supression du bloc,
                // le sélction ne change pas automatiquem d'objet
                // et qu'il ne revienne pas dans l'évènement
                lst_blocksScaned.UnselectAll();
                // enlève l'item sélectionné
                lst_blocksScaned_Taken.Items.Remove(lst_blocksScaned_Taken.SelectedItem);
                if (lst_blocksScaned.Items.Count == 0)
                {
                    // rend le bouton intouchable, car 0 bloc sélecionné
                    btn_Start.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Met à jour l'interface graphique en fonction du type de la commande
        /// et la donné reçue
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataReceived"></param>
        private void ProcessingCommand(CZordCommucication.Command command, string dataReceived)
        {
            switch (command)
            {
                case CZordCommucication.Command.NB_OF_BLOCS_DETECTED:
                    tbx_NbOfBlocks.Text = dataReceived.Substring(2);
                    lbl_StatusRxError.Content = "";
                    break;
                case CZordCommucication.Command.TRAIN_NOT_HERE:
                    rct_StatusOfTrain.Fill = Brushes.DarkRed;
                    lbl_StatusRxError.Content = "";
                    break;
                case CZordCommucication.Command.TRAIN_HERE:
                    rct_StatusOfTrain.Fill = Brushes.DarkGreen;
                    lbl_StatusRxError.Content = "";
                    break;
                case CZordCommucication.Command.TRAIN_STATE_UNKNOWN:
                    rct_StatusOfTrain.Fill = Brushes.DarkOrange;
                    lbl_StatusRxError.Content = "";
                    break;
                case CZordCommucication.Command.COORD_BLOC:
                    string color = "";
                    string Coord = dataReceived.Substring(9, 13);
                    // Inversion
                    float camX = Convert.ToSingle(Coord.Substring(10, 3)); // x: 001 y; 001
                    float camY = Convert.ToSingle(Coord.Substring(3, 3));
                    camX = config.ConvertTheAxis(Config.Axes.X, camX);
                    camY = config.ConvertTheAxis(Config.Axes.Y, camY);

                    if (Convert.ToInt32(dataReceived.Substring(5, 3)) == (int)Signature.RED)
                    {
                        color = RED;
                    }
                    else if (Convert.ToInt32(dataReceived.Substring(5, 3)) == (int)Signature.GREEN)
                    {
                        color = GREEN;
                    }
                    else if (Convert.ToInt32(dataReceived.Substring(5, 3)) == (int)Signature.BLUE)
                    {
                        color = BLUE;
                    }

                    if (config.IsInTheZoneOfStorage(camX, camY))
                    {
                        lst_blocksScaned.Items.Add("|" + color + " " + Coord + "|" + string.Format("x: {0,7:F2} y: {1,7:F2}", camX, camY) + "|");
                    }
                    else
                    {
                        lst_blocksScaned.Items.Add("|" + color + " " + Coord + "|" + string.Format("x: {0,7:F2} y: {1,7:F2}", camX, camY) + "| " + INACCESSIBLE);
                    }

                    lbl_StatusRxError.Content = "";
                    break;
                case CZordCommucication.Command.WAITING:
                case CZordCommucication.Command.PROCESSING:
                case CZordCommucication.Command.ERROR_NO_START:
                case CZordCommucication.Command.ERROR_TIME_OUT:
                case CZordCommucication.Command.ERROR_PORT:
                    lbl_StatusRxError.Content = dataReceived;
                    break;
                case CZordCommucication.Command.UNKNOWN_COMMAND:
                    lbl_StatusRxError.Content = "unknown command";
                    break;
                default:
                    lbl_StatusRxError.Content = "bug";
                    break;
            }
        }

        #endregion

    }
}
