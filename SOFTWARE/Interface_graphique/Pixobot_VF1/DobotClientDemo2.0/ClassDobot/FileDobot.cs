using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;

namespace ObjDobot
{
    class FileDobot
    {
        // Remplacer avec la class Dobot prochainement

        public string Name {
            get;
        }
        public FileDobot(string fileName)
        {
            Name = fileName;
        }

        private static OpenFileDialog filePath;


        public static FileDobot OpenFileWindow() // Ouvre l'explorateur de fichier, enregistre le dossier selectionner
        {
            string fileName = null;
            filePath = new OpenFileDialog();
            if (filePath.ShowDialog() == true)
            {
                fileName = Path.GetFileName(filePath.FileName); // Prend le nom du fichier
            }
            return new FileDobot(fileName);
        }

        public string[] GetCommands() // retourne le contenu d'un fichier text sous forme de tableau de string
        {
            string[] tabText = null;
            if (Name != null)
            {
                tabText = File.ReadAllLines($@"{filePath.FileName}"); // Ecris ce qu'il y a dans le fichier text dans le tableau // GERER ERREUR QUAND ON ANNULE LA SELECTION DU FICHIER DANS L
            }
            return tabText;
        }

        public static void Create(string fileName, RichTextBox richTextBox)
        {
            string richText = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text; // Lis le text de A à Z
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Sauvegard le fichier sur MesDocuments
            TextWriter writer = new StreamWriter($@"{filePath}\{fileName}.txt");    // Crée et donne un nom au fichier
            writer.Write(richText); // Ecris dans le fichier ce qu'on a dans le richText
            writer.Close();
        }

    }
}