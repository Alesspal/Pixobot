   M S B A M L   `   `   `  ����   DobotClientDemo     DobotClientDemo.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     �Name$  wnd_Pixobot�� � Title$ 
MainWindow��$
��1000��$	��765�� � 
ResizeMode$ NoResize=� � WindowStyle$
 None=�.��� -    �Name$ 
btn_CoordC�����	#FF353535.+��� $��1q�
�������$��0,0,0,724.8q��� -    ��Name$ 	cnv_Title��$	��998��$��	1,1,1,726q����	#FF292929.+��� -    ��Name$ btn_CloseApp�� �� Left$	 960��
 �� Top$ 7������ 	 �� 
TargetType"	 ��.��� 
 �� Property
 � �� �� Value
 �  ���� 
 �@�Cursor$
 Hand�� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ��� 
 � �� �	#FFFF3939����  �� 
TargetType" ��.R��� $��25��$��25�� �� CornerRadius$ 15��#��z���� -   $ btn_MinimizedApp��$	 920��$ 7������ "	 ��.��� 
 � �� �	#FFEE8A23�� 
 �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ��� 
 � �� �	#FFFDAE5D���� " ��.R��� $��25��$��25��$ 15��#��z���� -   $ btn_ConnectDobot��$	 790��$ 7������ "	 ��.��� 
 � ��$ Connect���� 
 � ��
 ������� 
 � ��
 �  ���� 
 �@�Cursor$
 Hand������ " ��.R��� $	��100��$��25��$ 15��#��z��._��� $��Center=�$��Center=��� -   $ btn_CalibrateDobot��$	 675��$ 7������ "	 ��.��� 
 � ��$ 	Calibrate���� 
 � ��
 ������� 
 � �� �	#FF3C6C85�� 
 �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ��� 
 � �� �	#FF4E7D96���� " ��.R��� $	��100��$��25��$ 15��#��z��._��� $��Center=�$��Center=��� -   $ btn_RebootDobot��$	 560��$ 7������ "	 ��.��� 
 � ��$ Reboot���� 
 � ��
 ������� 
 � �� �	#FF3C6C85�� 
 �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ��� 
 � �� �	#FF4E7D96���� " ��.R��� $	��100��$��25��$ 15��#��z��._��� $��Center=�$��Center=��� -	   $ btn_emergencyStop��$	 132��$ 7������ "	 ��.��� 
 � ��$ Emergency stop���� 
 � ��
 ������� 
 � ��
 �  ���� 
 �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ��� 
 � �� �	#FFFF3939���� " ��.R��� $	��200��$��25��$ 15��#��z��._��� $��Center=�$��Center=��� -
   $ btn_Accueil��$ 7��$ 4����Z� ���� "	 ��.��� 
 � ��$ homepage���� 
 � ��
 ������� 
 � ��
 ������� 
 �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ��� 
 � ��
 ��������� " ��.R��� $��65��$��25��$ 15��#��z��._��� $��Center=�$��Center=��� -   $ btn_Accueil_Copy��$ 74��$ 7����Z� ���� "	 ��.��� 
 � ��$ Config���� 
 � ��
 ������� 
 � ��
 ������� 
 �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ��� 
 � ��
 ��������� " ��.R��� $��50��$��25��$ 15��#��z��._��� $��Center=�$��Center=��� $��1q�
�������$
��1000��$��0,40,0,0q��� -   $ cnv_SerialConnect��$��32��$	��Top=�$��1,41,0,0q�$
��Left=�$	��998������  �� Color# � �� � -     �Name$ 
Frame_Main��$��1,73,1,1q�  � NavigationUIVisibility$ Hidden=�