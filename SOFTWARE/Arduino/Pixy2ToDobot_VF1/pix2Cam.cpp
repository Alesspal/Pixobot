#include "pix2Cam.h"

// La camèra pixy2 travail en SPI car le SPI peut travailler à 1MHz donc peut transmettre bcp plus d'information que l'I2C
// Objet principal Pixy

Pixy2 pixy;

/// <summary>
/// Initialise la caméra et allume la lampe
/// </summary>
void Pixy_Init()
{
	pixy.init();
	pixy.setLamp(1, 0);
}

/// <summary>
/// Detecte les blocks qui sont dans le champ de vison de la camèra.
/// Lire la fonction à chaque entré du loop
/// </summary>
void Scan_Blocks()
{
	pixy.ccc.getBlocks();
}

/// <summary>
/// Demande à la caméra le nombre de blocs qu'elle à détecté
/// </summary>
/// <returns> Le nombre de blocs détecté </returns>
int NumberOfBlocksDetected()
{
	return pixy.ccc.numBlocks;
}

/// <summary>
/// Demande à la camèra les infomations du blocs demandé
/// </summary>
/// <param name="block"></param>
/// <returns> les caractéristique du bloc </returns>
String ValuesOfBlock(int block)
{
	char buf[64];
	int sig = pixy.ccc.blocks[block].m_signature;
	int x = pixy.ccc.blocks[block].m_x;
	int y = pixy.ccc.blocks[block].m_y;
	int width = pixy.ccc.blocks[block].m_width;
	int height = pixy.ccc.blocks[block].m_height;
	int index = pixy.ccc.blocks[block].m_index;
	int age = pixy.ccc.blocks[block].m_age;

	sprintf(buf, "sig: %3d x: %3d y: %3d w: %3d h: %3d ind: %3d age: %3d", sig, x, y, width, height, index, age);

	return buf;
}
