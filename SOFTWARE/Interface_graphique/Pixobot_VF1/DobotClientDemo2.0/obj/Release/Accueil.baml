   M S B A M L   `   `   `  ���� J  FDobotClientDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DobotClientDemoDobotClientDemo      DobotClientDemo.Accueil   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     /localclr-namespace:DobotClientDemo       N� Title$  Accueil��.��� ���	#FF353535.+��� -    ��Name$ cnv_Application��$��1q�.+��� -    ��Name$ tbx_NbOfBlocks��$
��Wrap=�$��20��$��Normal"�$��16(�$��Normal$�$��Consolas)�
������� �� Left$	 169��
 �� Top$ 60��$��40����Z� �� -    ��Name$ lbl_StatusRxError��$��Error status Rx��$��29��$ 24��$ 25��$	��260��
���  ��$��0q�$��Normal"�$��Center=�$��16(�$
��Left=�$
��Left=�$��Center=� J� RenderTransformOrigin$ -0.095,-5.697&�$��Italic$��� -   $ lbl_NbOfBlocks��$��Number of blocks :��$��30��$ 54��$	��139��
�������$��0q�$��Normal"�$��Center=�$��16(�$
��Left=�$
��Left=�$ 24��$��Center=��� -    ��Name$ lst_blocksScaned��$	��463��$	��549��$ 24��$ 83��$��0q�$��1q�
�������$��Normal"�$��15(�$��Normal$�$��Consolas)����	#FFB0B0B0�� -   $ lst_blocksScaned_Taken��$	��463��$	��549��$	 513��$ 83��$��0q�$��1q�
�������$��Normal"�$��15(�$��Normal$�$��Consolas)����	#FFB0B0B0�� -    ��Name$ 	btn_Start��$��Start��$	 906��$	 650��$��16(�$��Normal"�$��1q�	 � UseLayoutRounding	 . $
��Left=�$��Center=����� 
 �� 
TargetType"
 ��.���  �� Property � �� �� Value$
 scan����  � ��
 �������  � �� �	#FF797A7A��  �@�Cursor$
 Hand�� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ���  � �� �	#FFA4A6A6����  �� 
TargetType" ��.R��� $��70��$��25�� �� CornerRadius$ 5��#��z��._��� $��Center=�$��Center=��� -   $ btn_Scan��$	 417��$	 650��$��16(�$��Normal"�$��1q�	 . $
��Left=�$��Center=����� "
 ��.���  � ��$
 scan����  � ��
 �������  � �� �	#FF797A7A��  �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ���  � �� �	#FFA4A6A6���� " ��.R��� $��70��$��25��$ 5��#��z��._��� $��Center=�$��Center=��� -	    ��Name$ rct_StatusOfTrain��
�������$��26��$	 869��$	 650��$��25���� -
   $ btn_Move��$	 335��$	 650��$��16(�$��Normal"�$��1q�	 . $
��Left=�$��Center=����� "
 ��.���  � ��$
 move����  � ��
 �������  � �� �	#FF797A7A��  �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ���  � �� �	#FFA4A6A6���� " ��.R��� $��70��$��25��$ 5��#��z��._��� $��Center=�$��Center=��� -   $ 	tbk_Coord��$	 512��$
��Wrap=�$	 652��$��21��$	��342��
�������$��16(� �� TextAlignment$ Center=�