# Pixobot

Ce projet vise à charger des cubes colorés sur un train. Utilisant une caméra Pixy2 pour la détection, un bras robotique Dobot Magician pour la manipulation, et une interface utilisateur pour le contrôle, le système entier est orchestré via un Arduino MEGA.

## Sommaire

- [Présentation du projet](#présentation-du-projet)
- [Hardware](#hardware)
- [Châssis](#châssis)
- [Interface graphique](#interface-graphique)
- [Conclusion](#conclusion)

---

## Présentation du projet

Le but de ce projet est de charger un train à l’aide d’un bras robotique. Le système est composé d’une caméra Pixy2, d’un Arduino MEGA, d’un bras robotique Dobot Magician, d’un ordinateur et d’un train simulé par une zone définie.

Ce projet a été réalisé pour mon travail pratique individuel de fin de CFC (Certificat fédéral de capacité) d’électronicien en avril 2022. Je précise que ce README ne vous donnera pas toutes les explications pour le mettre en œuvre parfaitement. Je n’expliquerai pas toutes les choses en détail et ne parlerai pas de code. Il sert à montrer un de mes premiers projets assez concrets que j’ai pu réaliser pendant mes études. Je souligne aussi que je débutais en code et qu’il n’est pas démonstratif de mon niveau actuel. Si vous voulez des explications bien plus précises, consultez mon rapport « Rapport TPI 2022 ».

## Principe de fonctionnement

- Un signal externe, ici simulé par un interrupteur connecté à l’Arduino, indique que le train est en position pour être chargé.
- La caméra récupère les coordonnées des objets présents dans son champ de vision en fonction de leurs signatures configurées dans l’application PixyMon. Les objets sont déposés manuellement sur le plan de travail.
- L’application établit une liste des objets trouvés, et, indique s’ils sont dans la zone de stockage configurée dans l’application PixoBot. Si oui, on peut alors sélectionner le ou les objets désirés et les charger de manière autonome sur le train.

## Description du travail

### 1. Caméra et Arduino

- Utilisation d’une caméra connectée à un Arduino MEGA par le bus SPI et utilisation de la classe dédiée Pixy. Celle-ci permet de récupérer la liste des objets détectés (signature, coordonnées, taille, etc.).
- Les signatures seront définies à l’aide de l’application PixyMon. Les objets sont identifiés selon leurs couleurs et formes. Deux objets de même couleur mais de formes différentes peuvent être différenciés.
- L’Arduino communique avec le PC en mode COMMAND via l’UART1. Un câble UART to USB sera utilisé.

### 2. Bras robotique Dobot Magician et PC

Le bras robotique Dobot Magician est connecté au PC. Il est contrôlé par l’API fournie par Dobot. L’application réalisée en langage C# (Pixobot) permet :
- De connecter la caméra et le bras robotique.
- De configurer la zone de stockage et celle du train.
- De convertir les axes de la caméra en fonction de ceux du bras robotique.
- De récupérer la liste des objets disposés sur la zone de stockage, sur demande de l’utilisateur (clic d’un bouton).
- De sélectionner les objets et l’ordre dans lequel on désire les charger sur le train.
- De manipuler le bras robotique afin de charger ces objets sur demande de l’utilisateur sur le train, s’il est présent (clic d’un bouton). L’interrogation du train se fait au moment du clic.

## Exigences particulières

- Pour simplifier, le sens de la caméra et du train sont fixes, et ceux-ci sont placés parallèlement au système d’axes du bras.
- Les objets et le train seront placés dans le rayon d’action du bras. La documentation indiquera les limites à prendre en compte afin d’être dessinées sur le plan de travail, mais elles ne seront pas calculées par l’application.
- Un mode configuration permettra de convertir le système d’axes de la caméra par rapport au bras robotique et de définir où se situent la zone de stockage et du train (x,y,z) par rapport à celui-ci.
- Les objets seront placés sur le train les uns derrière les autres avec une distance configurable.
- Tous les objets ont la même hauteur.
- Dans les limites du possible, une tolérance de +/- 2 mm est demandée pour la précision du bras à la prise des objets.

## Informations complémentaires

Le projet sera réalisé avec le matériel, logiciel et documentation déjà utilisés partiellement en théorie/atelier, en particulier :
- Visual Studio 2019 et Visual Micro
- .NET FrameWork 4.8 minimum
- Caméra pixy2, carte Arduino MEGA standard, Arduino MEGA click SHIELD, câble UART to USB, composant divers
- Librairies cZordBase_v4e
- Logiciel TermiZord_Project
- Contenu du répertoire EL34 à disposition
- Anciens codes

## Hardware

Commençons par parler du hardware. Comme dit plus haut, nous avons un bouton qui va simuler l’état du train et un câble UART qui va gérer la communication de l’Arduino avec un ordinateur. J’ai fait un schéma bloc pour représenter comment vont être branchés les appareils entre eux et un Véroboard. Le vérobord va être imbriqué sur la plaque Arduino MEGA click SHIELD.

### Schéma bloc :

![Pasted Graphic](https://github.com/Alesspal/Pixobot/assets/119937254/f8b2335e-762e-4375-89f6-a700e8bbd0c0)

### Véroboard :

![Pasted Graphic 1](https://github.com/Alesspal/Pixobot/assets/119937254/47cf8f9b-7cf1-459f-88b5-9bcc0eea6d10)

### Résultat du schéma bloc :

![Pasted Graphic 2](https://github.com/Alesspal/Pixobot/assets/119937254/0bb25603-2cad-45b6-b79d-4206fa28400d)

## Châssis

Le châssis sert à tenir la caméra qui va servir à localiser les blocs dans la zone de stockage, en haut du bras robotique. La structure est construite avec des barres en métal. La base forme un rectangle qui viendra entourer le Dobot. Cette base aura un côté modulable pour venir le serrer.

![largeur](https://github.com/Alesspal/Pixobot/assets/119937254/80addcef-6101-4a6f-98a3-ac8af7b4c1cc)

Sur le côté arrière de la base, il y a une deuxième barre. Ces deux barres vont servir d’accroche pour les deux autres barres qui vont faire la hauteur de la structure. Une troisième barre va être accrochée sur ces deux barres qui sera modulable pour pouvoir régler la hauteur de la caméra.

![devant du Dobot](https://github.com/Alesspal/Pixobot/assets/119937254/d3d0f66b-9c8b-4a91-9594-47ad5b673d3c)

Voici un aperçu visuel de toute la structure :

![Pasted Graphic 5](https://github.com/Alesspal/Pixobot/assets/119937254/3031eced-6da6-414b-9caa-951f5f5b6cad)

## Interface graphique

En haut de l’interface graphique, il y’a deux barres d’actions. Celle tout en haut sert au Dobot Magician et celle d’en bas sert à se connecter à la caméra.

![Pasted Graphic 6](https://github.com/Alesspal/Pixobot/assets/119937254/ab3855ed-5b80-488b-97aa-384c7dcd4065)

On y retrouve des boutons importants comme « Connect » pour connecter le bras ou « Emergency stop » dans le cas où il faut arrêter le bras robotique d'urgence. Les deux autres boutons servent au bon fonctionnement du Dobot Magician.

![Pasted Graphic 7](https://github.com/Alesspal/Pixobot/assets/119937254/3fed634e-5095-4dc4-8257-ce16728d8b78)

De même pour la caméra, on y retrouve un bouton « Connect » avec des listes où l'on peut choisir le port série voulu et la vitesse de transmission.

### Pages de l'interface

L'interface comprend deux pages. La page « Config » qui va servir à calibrer l'axe de la caméra à l'axe du Dobot Magician, et la page « Homepage » qui servira à l'utilisateur de scanner les blocs, choisir lesquels il veut mettre dans le train et lancer l'opération.

#### Interface de la page Config

![Configuration des axes et du stockage](https://github.com/Alesspal/Pixobot/assets/119937254/c8775293-9eaf-4974-bbc7-72ac2477a571)

On y voit des instructions à suivre pour réussir à calibrer les deux axes. On parle de calibrage d’axe, car l’axe du Dobot Magician est en millimètre, alors que la caméra est en pixel et on tout les deux une plage différente. Pour les synchroniser il suffit de prendre deux points physiques différents, comparer les deux points pris dans chaque axes et faire des simples calculs, pour qu’une coordonnée de la caméra, se transforme en une coordonné du bras robotique.

#### Interface de la page Homepage

![Pasted Graphic 10](https://github.com/Alesspal/Pixobot/assets/119937254/2bd10abb-3e5e-4e62-b2e4-f59bd289e8e7)

Il y a deux zones : la première affiche les blocs scannés, et la seconde montre les blocs que l'on a sélectionnés depuis la première zone.

## Conclusion

Après avoir traversé chaque étape et composant de ce projet, il est clair que celui-ci représente une combinaison de différentes compétences, allant de la mécanique à la programmation en passant par l'interaction homme-machine. 

Ce sera tout pour ce Readme, j’espère que ça vous a plu et je vous remercie d’avoir lu jusqu’au bout. Comme mentionné au tout début, si vous souhaitez obtenir des informations plus précises, je vous invite à consulter mon rapport.

Merci et bonne continuation !
