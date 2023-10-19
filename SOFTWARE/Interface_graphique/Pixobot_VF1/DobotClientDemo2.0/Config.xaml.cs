using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using DobotAPI;

namespace DobotClientDemo
{
    /// <summary>
    /// Logique d'interaction pour Config.xaml
    /// </summary>
    public partial class Config : Page
    {

        #region Attributs et constantes de la frames

        private static Config instance;
        public static Config getInstance()
        {
            if (instance == null)
            {
                instance = new Config();
            }
            return instance;
        }

        private enum EllipsePosition
        {
            TOP_LEFT_CORNER,
            BOT_RIGHT_CORNER
        }

        public AxeSettings axeSettings;
        public struct AxeSettings
        {
            public int ref_CamX0;
            public int ref_CamY0;
            public float ref_ArmX0;
            public float ref_ArmY0;
            public float MultiplicatorOfConversionAxeX;
            public float MultiplicatorOfConversionAxeY;
        }


        // Pour les structures "Characteristics" height : Z du bras, lenth : diff ebtre X0 et X1 du bras, width : diff entre Y0 et Y1 du bras
        // coord : coordonnée du point, location : emplacement dédié au futur objet
        public struct StorageCharacteristics
        {
            public float coordX0;
            public float coordX1;
            public float coordY0;
            public float coordY1;
            public float height;
            public float lenth;
            public float width;
        }
        public StorageCharacteristics storageCharacteristics;

        public struct TrainCharacteristics
        {
            public float coordX0;
            public float coordX1;
            public float coordY0;
            public float coordY1;
            public float height;
            public float lenth;
            public float width;
            public int nbOfSeat;
            public float locationXOfSeat;
            public float locationYOfSeat;
        }
        public TrainCharacteristics trainCharacteristics;

        public enum ObjectCharacteristics
        {
            HEIGHT = 25,
            WIDTH = 25,
        }

        public enum Axes
        {
            X,
            Y
        }

        public int object_Spacing = 15;

        private readonly DispatcherTimer tmr_Dobot;

        private CZordCommucication cZordCommucication;
        private Dobot dobot;
        private Ellipse _ellipse = new Ellipse();
        private int nbEllipseBlueRemoved = 0;
        private int nbEllipseRedRemoved = 0;
        private float[] storageArmX = new float[2];
        private float[] storageArmY = new float[2];
        private int[] storageCamX = new int[2];
        private int[] storageCamY = new int[2];

        private bool _isConfigured;
        public bool IsConfigured {
            get {
                return _isConfigured;
            }
        }

        #endregion

        #region Initialisation

        private Config()
        {
            InitializeComponent();

            cZordCommucication = CZordCommucication.GetInstance();
            dobot = Dobot.GetInstance();

            tmr_Dobot = new DispatcherTimer();
            tmr_Dobot.Stop();
            tmr_Dobot.Interval = new TimeSpan(10000000);            // Interval d'appel de l'événement Tick (100ms)
            tmr_Dobot.Tick += new EventHandler(Tmr_Dobot_Tick);     // Abonnement à l'événement du timer

            tmr_Dobot.Start();

            lbl_StatusRxError.Content = "";

            FileSearch();
        }

        #endregion

        #region Affichage des coordonnées du bras

        /// <summary>
        /// Donner les coordonnées du bras chaque 100ms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmr_Dobot_Tick(object sender, EventArgs e)
        {
            if (dobot.IsConnected)
            {
                tbk_Coord.Text = $"X : {dobot.ArmGetX()}    Y : {dobot.ArmGetY()}   Z : {dobot.ArmGetZ()}   R : {dobot.ArmGetRHead()}";
            }
        }

        #endregion

        #region Gestion de la nouvelle et ancienne configuration (btn_Restart, btn_SaveConfig, fichier Config)

        /// <summary>
        /// Redemarre la configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Restart_Click(object sender, RoutedEventArgs e)
        {
            ConfigRestart();
        }

        /// <summary>
        /// Sauvegarde la configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            // configuration des axes
            AxisConfigMultiplicator(storageCamX, storageCamY, storageArmX, storageArmY);
            // configuration de la taille des zones
            ConfigurationOfTrainAndStorageChaaracteristics();
            // mettre à jour l'affichage
            EndOfConfigurationDisplay();
            // Crée le fichier "config.txt"
            CreateFileConfig();
        }

        /// <summary>
        /// Recherche si il y un fichier Config.txt
        /// </summary>
        private void FileSearch()
        {
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(".\\Config.txt");
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                int cpt = 0;
                while (line != null)
                {
                    cpt++;
                    if (float.TryParse(line, out float nb))
                    {
                        switch (cpt)
                        {
                            case 1:
                                axeSettings.ref_CamX0 = (int)nb;
                                break;
                            case 2:
                                axeSettings.ref_CamY0 = (int)nb;
                                break;
                            case 3:
                                axeSettings.ref_ArmX0 = nb;
                                break;
                            case 4:
                                axeSettings.ref_ArmY0 = nb;
                                break;
                            case 5:
                                storageCamX[0] = (int)nb;
                                break;
                            case 6:
                                storageCamY[0] = (int)nb;
                                break;
                            case 7:
                                storageCamX[1] = (int)nb;
                                break;
                            case 8:
                                storageCamY[1] = (int)nb;
                                break;
                            case 9:
                                storageArmX[0] = nb;
                                storageCharacteristics.coordX0 = nb;
                                break;
                            case 10:
                                storageArmY[0] = nb;
                                storageCharacteristics.coordY0 = nb;
                                break;
                            case 11:
                                storageArmX[1] = nb;
                                storageCharacteristics.coordX1 = nb;
                                break;
                            case 12:
                                storageArmY[1] = nb;
                                storageCharacteristics.coordY1 = nb;
                                break;
                            case 13:
                                storageCharacteristics.height = nb;
                                break;
                            case 14:
                                trainCharacteristics.coordX0 = nb;
                                break;
                            case 15:
                                trainCharacteristics.coordY0 = nb;
                                break;
                            case 16:
                                trainCharacteristics.coordX1 = nb;
                                break;
                            case 17:
                                trainCharacteristics.coordY1 = nb;
                                break;
                            case 18:
                                trainCharacteristics.height = nb;
                                break;
                            case 19:
                                axeSettings.MultiplicatorOfConversionAxeX = nb;
                                break;
                            case 20:
                                axeSettings.MultiplicatorOfConversionAxeY = nb;
                                ConfigurationOfTrainAndStorageChaaracteristics();
                                EndOfConfigurationDisplay();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ConfigRestart();
                        MessageBox.Show("Un problème est survenu pendant la configuration automatique.\n" +
                            "Faire la configuration manuellement", "ERROR");
                        return;
                    }
                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
                if (cpt != 0 && cpt != 20)  // Finis la configuration seulement si il lit toutes les lignes
                {
                    MessageBox.Show("Un problème est survenu pendant la configuration automatique.\n" +
                        "Faire la configuration manuellement", "ERROR");
                    ConfigRestart();
                }
            }
            catch (Exception e)
            {
                ConfigRestart();
            }
        }

        /// <summary>
        /// Ecrase le fixier du même nom existant et créer un nouveau fichier
        /// avec les données eu apres une configuration
        /// </summary>
        private void CreateFileConfig()
        {
            try
            {
                // crée un fichier "Config.txt" dans le dossier du code (relese ou debug)
                StreamWriter sw = new StreamWriter(".\\Config.txt");
                //Write lines
                sw.WriteLine(axeSettings.ref_CamX0);
                sw.WriteLine(axeSettings.ref_CamY0);
                sw.WriteLine(axeSettings.ref_ArmX0);
                sw.WriteLine(axeSettings.ref_ArmY0);
                sw.WriteLine(storageCamX[0]);
                sw.WriteLine(storageCamY[0]);
                sw.WriteLine(storageCamX[1]);
                sw.WriteLine(storageCamY[1]);
                sw.WriteLine(storageArmX[0]);
                sw.WriteLine(storageArmY[0]);
                sw.WriteLine(storageArmX[1]);
                sw.WriteLine(storageArmY[1]);
                sw.WriteLine(storageCharacteristics.height);
                sw.WriteLine(trainCharacteristics.coordX0);
                sw.WriteLine(trainCharacteristics.coordY0);
                sw.WriteLine(trainCharacteristics.coordX1);
                sw.WriteLine(trainCharacteristics.coordY1);
                sw.WriteLine(trainCharacteristics.height);
                sw.WriteLine(axeSettings.MultiplicatorOfConversionAxeX);
                sw.WriteLine(axeSettings.MultiplicatorOfConversionAxeY);
                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e.Message, "ERROR");
            }
        }

        #endregion

        #region Créations/destruction des ellipses et sauvegarde des valeurs des coordonnées

        /// <summary>
        /// Crée une ellipse de la couleur donnée à la position donnée
        /// </summary>
        /// <param name="brushes"> Couleur de l'ellipse </param>
        /// <param name="positon"> Position de l'ellipse </param>
        private void CreateEllipse(SolidColorBrush brushes, EllipsePosition positon)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 80;
            ellipse.Height = 80;
            ellipse.Fill = brushes;
            if (positon == EllipsePosition.TOP_LEFT_CORNER)
            {
                ellipse.Margin = new Thickness(86, 69, 0, 0);
            }
            else if (positon == EllipsePosition.BOT_RIGHT_CORNER)
            {
                ellipse.Margin = new Thickness(835, 543, 0, 0);
            }
            _ellipse = ellipse;
            cnv_Config.Children.Add(ellipse);
        }

        /// <summary>
        /// Quand l'évenement click du canavas est déclenché. Recherche l'ellipse et l'enleve,
        /// puis sauvegarde les valeurs reçu de la caméra ou du dobot en fonction de l'ellipse enlevé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cnv_Config_MouseDown_Remove_Ellipse(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse ellipse)
            {
                cZordCommucication.WriteCommand("gnb");
                cZordCommucication.WriteCommand("gba");

                if (ellipse.Fill == Brushes.DarkBlue)
                {
                    if (cZordCommucication._flagSerialPortConnected)
                    {
                        bool enable = true;
                        while (enable)
                        {
                            lbl_StatusRxError.Content = "";
                            string dataReceived = cZordCommucication.SerialCheckAvailableAndRead();
                            CZordCommucication.Command commandAnalyzed = cZordCommucication.AnalyzeCommand(dataReceived);
                            if (commandAnalyzed == CZordCommucication.Command.ERROR_TIME_OUT || commandAnalyzed == CZordCommucication.Command.ERROR_PORT)
                            {
                                lbl_StatusRxError.Content = dataReceived;
                                enable = false;
                            }
                            else if (commandAnalyzed == CZordCommucication.Command.NB_OF_BLOCS_DETECTED)
                            {
                                int nb = Convert.ToInt32(dataReceived.Substring(2));
                                if (nb > 1)
                                {
                                    MessageBox.Show("Mettre qu'un seul bloc de couleur sur le champ de vision de la caméra", "ERROR");
                                    enable = false;
                                }
                                else if (nb == 0)
                                {
                                    MessageBox.Show("Mettre un bloc de couleur sur le champ de vision de la caméra", "ERROR");
                                    enable = false;
                                }

                            }
                            else if (commandAnalyzed == CZordCommucication.Command.COORD_BLOC)
                            {
                                cnv_Config.Children.Remove(ellipse);
                                nbEllipseBlueRemoved++;
                                switch (nbEllipseBlueRemoved)
                                {
                                    case 1:
                                        storageCamX[0] = Convert.ToInt32(dataReceived.Substring(12, 3));
                                        storageCamY[0] = Convert.ToInt32(dataReceived.Substring(19, 3));
                                        tbk_Indication.Text = "Placez le dobot sur le bloc de couleur déja positionné" +
                                       ", ensuite cliquez sur le cercle vert de l'interface";
                                        tbk_ValueAxeSave.Text += $"CamX0 : {storageCamX[0]}, ";
                                        tbk_ValueAxeSave.Text += $"CamY0 : {storageCamY[0]}, ";
                                        CreateEllipse(Brushes.DarkGreen, EllipsePosition.TOP_LEFT_CORNER);
                                        break;
                                    case 2:
                                        storageCamX[1] = Convert.ToInt32(dataReceived.Substring(12, 3));
                                        storageCamY[1] = Convert.ToInt32(dataReceived.Substring(19, 3));
                                        tbk_Indication.Text = "Placez le dobot sur le bloc de couleur déja positionné" +
                                       ", ensuite cliquez sur le cercle vert de l'interface";
                                        tbk_ValueAxeSave.Text += $"CamX1 : {storageCamX[1]}, ";
                                        tbk_ValueAxeSave.Text += $"CamY1 : {storageCamY[1]}, ";
                                        CreateEllipse(Brushes.DarkGreen, EllipsePosition.BOT_RIGHT_CORNER);
                                        break;
                                    default:
                                        break;
                                }
                                enable = false;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Connecter la caméra", "Info");
                    }
                }
                else if (ellipse.Fill == Brushes.DarkGreen)
                {
                    if (dobot.IsConnected)
                    {
                        float x = dobot.ArmGetX();
                        float y = dobot.ArmGetY();
                        float z = dobot.ArmGetZ();
                        cnv_Config.Children.Remove(ellipse);
                        nbEllipseRedRemoved++;
                        switch (nbEllipseRedRemoved)
                        {
                            case 1:
                                tbk_Indication.Text = "déplacez le même bloc de couleur à l'angle opposé de la zone de stockage" +
                                    ", ensuite cliquez sur le cercle bleu sur l'interface";
                                CreateEllipse(Brushes.DarkBlue, EllipsePosition.BOT_RIGHT_CORNER);
                                storageArmX[0] = x;
                                storageArmY[0] = y;
                                storageCharacteristics.coordX0 = x;
                                storageCharacteristics.coordY0 = y;
                                storageCharacteristics.height = z - (int)ObjectCharacteristics.HEIGHT;
                                tbk_ValueAxeSave.Text += $"StockageX0 : {x}, ";
                                tbk_ValueAxeSave.Text += $"StockageY0 : {y}, ";
                                break;
                            case 2:
                                tbk_modeConfig.Text = "Configuration du train";
                                tbk_Indication.Text = "Placez le dobot dans un angle de la zone du train" +
                                    ", ensuite cliquez sur le cercle vert de l'interface";
                                CreateEllipse(Brushes.DarkGreen, EllipsePosition.TOP_LEFT_CORNER);
                                storageArmX[1] = x;
                                storageArmY[1] = y;
                                storageCharacteristics.coordX1 = x;
                                storageCharacteristics.coordY1 = y;
                                tbk_ValueAxeSave.Text += $"StockageX1 : {x}, ";
                                tbk_ValueAxeSave.Text += $"StockageY1 : {y}, ";
                                break;
                            case 3:
                                tbk_Indication.Text = "Placez le dobot sur l'angle opposé de la zone du train" +
                                ", ensuite cliquez sur le cercle vert de l'interface";
                                CreateEllipse(Brushes.DarkGreen, EllipsePosition.BOT_RIGHT_CORNER);
                                trainCharacteristics.coordX0 = x;
                                trainCharacteristics.coordY0 = y;
                                trainCharacteristics.height = z;
                                tbk_ValueAxeSave.Text += $"TrainX0 : {x}, ";
                                tbk_ValueAxeSave.Text += $"TrainY0 : {y}, ";
                                break;
                            case 4:
                                tbk_modeConfig.Text = "Configurations terminées";
                                tbk_Indication.Text = "Appuyez sur le bouton save pour sauvegarder les valeurs ou restart pour recommencer";
                                btn_SaveConfig.IsEnabled = true;
                                trainCharacteristics.coordX1 = x;
                                trainCharacteristics.coordY1 = y;
                                tbk_ValueAxeSave.Text += $"TrainX1 : {x},  ";
                                tbk_ValueAxeSave.Text += $"TrainY1 : {y} ";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Connectez le Dobot", "INFO");
                    }
                }

            }
        }

        #endregion

        #region Configuration de l'axe de la caméra au dobot, configuration du train et du stockage

        /// <summary>
        /// Mets tous les paramètres de l'enum _AxeSettings à 0
        /// </summary>
        private void ConfigRestart()
        {
            axeSettings.ref_CamX0 = 0;
            axeSettings.ref_CamY0 = 0;
            axeSettings.ref_ArmX0 = 0;
            axeSettings.ref_ArmY0 = 0;
            axeSettings.MultiplicatorOfConversionAxeX = 0;
            axeSettings.MultiplicatorOfConversionAxeY = 0;
            storageCharacteristics.coordX0 = 0;
            storageCharacteristics.coordX1 = 0;
            storageCharacteristics.coordY0 = 0;
            storageCharacteristics.coordY1 = 0;
            storageCharacteristics.height = 0;
            storageCharacteristics.lenth = 0;
            storageCharacteristics.width = 0;
            trainCharacteristics.coordX0 = 0;
            trainCharacteristics.coordX1 = 0;
            trainCharacteristics.coordY0 = 0;
            trainCharacteristics.coordY1 = 0;
            trainCharacteristics.height = 0;
            trainCharacteristics.lenth = 0;
            trainCharacteristics.width = 0;
            trainCharacteristics.locationXOfSeat = 0;
            trainCharacteristics.locationYOfSeat = 0;
            trainCharacteristics.nbOfSeat = 0;

            for (int i = 0; i < storageArmX.Length; i++)
            {
                storageArmX[i] = 0;
                storageArmY[i] = 0;
                storageCamX[i] = 0;
                storageCamY[i] = 0;
            }

            nbEllipseBlueRemoved = 0;
            nbEllipseRedRemoved = 0;
            StartOfConfigurationDisplay();
        }

        /// <summary>
        /// Affichage de début de configuration
        /// </summary>
        private void StartOfConfigurationDisplay()
        {
            if (_ellipse != null)
            {
                cnv_Config.Children.Remove(_ellipse);
            }
            CreateEllipse(Brushes.DarkBlue, EllipsePosition.TOP_LEFT_CORNER);
            tbk_modeConfig.Text = " Configuration des axes et du stockage";
            tbk_Indication.Text = "Placer un bloc de couleur dans un angle du stockage" +
                ", ensuite cliquez sur le cercle sur l'interface";
            tbk_ValueAxeSave.Text = "";
            btn_SaveConfig.IsEnabled = false;
            _isConfigured = false;
        }

        /// <summary>
        /// Affichage de fin de configuartion
        /// </summary>
        private void EndOfConfigurationDisplay()
        {
            // Mets à jour l'interface graphique
            btn_SaveConfig.IsEnabled = false;
            tbk_modeConfig.Text = "Configurations terminées";
            tbk_Indication.Text = "";
            cnv_Config.Children.Remove(_ellipse);
            _isConfigured = true;
        }

        /// <summary>
        /// Configure les références et les multiplicateurs des axes
        /// </summary>
        /// <param name="camX"></param>
        /// <param name="camY"></param>
        /// <param name="armX"></param>
        /// <param name="armY"></param>
        private void AxisConfigMultiplicator(int[] camX, int[] camY, float[] armX, float[] armY)
        {
            // A TESTER!!!!!!!!!!!!
            // Inversion des axes X et Y de la caméra pour les mettres en fonction du Dobot
            for (int i = 0; i < camX.Length; i++)
            {
                int tmp = camX[i];
                camX[i] = camY[i];
                camY[i] = tmp;
            }

            axeSettings.ref_CamX0 = camX[0];
            axeSettings.ref_CamY0 = camY[0];
            axeSettings.ref_ArmX0 = armX[0];
            axeSettings.ref_ArmY0 = armY[0];

            float deltaCamX = Math.Abs(camX[1] - camX[0]);
            float deltaCamY = Math.Abs(camY[1] - camY[0]);
            float deltaArmX = Math.Abs(armX[1] - armX[0]);
            float deltaArmY = Math.Abs(armY[1] - armY[0]);

            axeSettings.MultiplicatorOfConversionAxeX = deltaArmX / deltaCamX;
            axeSettings.MultiplicatorOfConversionAxeY = deltaArmY / deltaCamY;
        }

        /// <summary>
        /// Prend la valeur de la caméra mise en paramètre et la convertis en le type d'axe donné
        /// en fonction de l'axe du dobot
        /// </summary>
        /// <param name="axes"></param>
        /// <param name="valueOfCamToConvert"></param>
        /// <returns></returns>
        public float ConvertTheAxis(Axes axes, float valueOfCamToConvert)
        {
            if (axes == Axes.X)
            {
                valueOfCamToConvert -= axeSettings.ref_CamX0;
                valueOfCamToConvert *= axeSettings.MultiplicatorOfConversionAxeX;
                valueOfCamToConvert += axeSettings.ref_ArmX0;
            }
            else if (axes == Axes.Y)
            {
                valueOfCamToConvert -= axeSettings.ref_CamY0;
                valueOfCamToConvert *= axeSettings.MultiplicatorOfConversionAxeY;
                valueOfCamToConvert += axeSettings.ref_ArmY0;
            }
            return (float)Math.Round(valueOfCamToConvert, 2);
        }

        /// <summary>
        /// Calcule la largeur et la longeur des zones definies,
        /// puis calcule le nombre de place dans le train
        /// </summary>
        public void ConfigurationOfTrainAndStorageChaaracteristics()
        {
            storageCharacteristics.lenth = Math.Abs(storageCharacteristics.coordX1 - storageCharacteristics.coordX0);
            storageCharacteristics.width = Math.Abs(storageCharacteristics.coordY1 - storageCharacteristics.coordY0);
            trainCharacteristics.lenth = Math.Abs(trainCharacteristics.coordX1 - trainCharacteristics.coordX0);
            trainCharacteristics.width = Math.Abs(trainCharacteristics.coordY1 - trainCharacteristics.coordY0);
            trainCharacteristics.nbOfSeat = (int)trainCharacteristics.lenth / ((int)ObjectCharacteristics.WIDTH + object_Spacing);
        }


        /// <summary>
        /// Calcule l'emplacement ou l'objet doit aller
        /// Donne les emblacments à la structure trainCharacteristics.LocationX et Y
        /// </summary>
        public void CalculatingLocationObject(int seatNumber = 0)
        {
            // calcul pour centrer les objets sur train
            int centering = (int)trainCharacteristics.lenth - trainCharacteristics.nbOfSeat * ((int)ObjectCharacteristics.WIDTH + object_Spacing);
            centering += object_Spacing;   //  pour enlever le dernier espacement des objets
            centering /= 2;

            if (trainCharacteristics.coordY0 > 0)   // calcule la postion y dans le train
            {
                trainCharacteristics.locationYOfSeat = trainCharacteristics.coordY0 - trainCharacteristics.width / 2;
            }
            else
            {
                trainCharacteristics.locationYOfSeat = trainCharacteristics.coordY0 + trainCharacteristics.width / 2;
            }

            if (seatNumber >= 0)                    // calcule la postion x dans le train
            {
                if (trainCharacteristics.coordX0 > trainCharacteristics.coordX1)
                {
                    trainCharacteristics.locationXOfSeat = trainCharacteristics.coordX1 + centering +
                        (int)ObjectCharacteristics.WIDTH / 2 + seatNumber * ((int)ObjectCharacteristics.WIDTH + object_Spacing);
                }
                else
                {
                    trainCharacteristics.locationXOfSeat = trainCharacteristics.coordX0 + centering +
                        (int)ObjectCharacteristics.WIDTH / 2 + seatNumber * ((int)ObjectCharacteristics.WIDTH + object_Spacing);
                }
            }
        }

        /// <summary>
        /// Calcule si le bloc est dans la zone de stockage
        /// </summary>
        /// <param name="armX"></param>
        /// <param name="armY"></param>
        /// <returns> true : dans la zone, fale : en dehors de la zone </returns>
        public bool IsInTheZoneOfStorage(float armX, float armY)
        {
            bool isInTheZoneOfStorage = true;

            if (storageCharacteristics.coordX0 > storageCharacteristics.coordX1)
            {
                if (armX > storageCharacteristics.coordX0 || armX < storageCharacteristics.coordX1)
                {
                    isInTheZoneOfStorage = false;
                }
            }
            else if (storageCharacteristics.coordX0 <= storageCharacteristics.coordX1)
            {
                if (armX < storageCharacteristics.coordX0 || armX > storageCharacteristics.coordX1)
                {
                    isInTheZoneOfStorage = false;
                }
            }

            if (storageCharacteristics.coordY0 > storageCharacteristics.coordY1)
            {
                if (armY > storageCharacteristics.coordY0 || armY < storageCharacteristics.coordY1)
                {
                    isInTheZoneOfStorage = false;
                }
            }
            else if (storageCharacteristics.coordY0 <= storageCharacteristics.coordY1)
            {
                if (armY < storageCharacteristics.coordY0 || armY > storageCharacteristics.coordY1)
                {
                    isInTheZoneOfStorage = false;
                }
            }
            return isInTheZoneOfStorage;
        }

        #endregion

    }
}