   M S B A M L   `   `   `  ����@  <DELTA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:DELTADELTA  5   	       DELTA.AdminWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:DELTA     5   	     � Title$  Админ Панель��5   	   $	��550��6   $	��800��6+   
������ $	��550��5	   "   $	��800��62   $	��500��6A    � 	Resources 5       �� 
TargetType   	CardStyle �� Property �� Value �� 
TargetType ��Name �� CornerRadius �� Triggers	 P� Property
 P� Value N� EnterActions �� 
TargetName �� TargetProperty	 �� To g� Duration N� ExitActions=  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�  &	        '�� �     '  �    �� 5   
   " ��6   .�5      ��  �@��CornerRadius$ 10��6-   �� 5       � �� �	#FF6A6A6D6,   �� 5       � ��$ 1q�60   �� 5       �@��Padding$ 10q�6(   �� 5       � �� �	#FF4B4B4F6+   5      �� 5   
   " ��6   .�5      ��  � �� �	#FF007ACC6+   �� 5       � ��
 �����6+   �� 5       � ��
 ���� 6,   �� 5       � ��$ 0q�60   �� 5       � ��$
 10,0q�6(   �� 5       � ��$ 30��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $ border��6!   $ 10��61   #��z��6C   ._�5      �� $��Center=�6/   $��Center=�6L   5        5!      P� 5"      	 � ��
 .  5#   "   �� 5$   &   .��5%   *   �� .��5&   .   �� $ border��6=   $1 +(Border.Background).(SolidColorBrush.Color)�5'   =   $ 	#FF0055AAX�5(   =   $ 0:0:0.3D�6L   5)   +   5*   '    5,   "   �� 5-   &   .��5.   *   �� .��5/   .   �� $ border��6=   $1 +(Border.Background).(SolidColorBrush.Color)�50   =   $ 	#FF007ACCX�51   =   $ 0:0:0.3D�6L   52   +   53   '   55      57      59      5:      �� 5=   
   "  6   .�5>      ��  � �� �	#FF6A6A6D6+   �� 5?       � ��
 �����6+   �� 5A       � ��$
 Bold"�6+   �� 5B       � ��$ Center=�6;   �� 5C       � ��$ 5q�6(   5D      5E      .��5G      � ���	#FF3F3F47.+�5H   
   ��  �� Orientation$ 
Horizontal=�6   $	��Top=�6.   $��0,10,0,0q�6F   $��Center=�6X   $	��780��6u   .+�5I      �� -   5        $��
Выйти��5I      $��75��6%   $��	10,0,10,0q�60   $
��Left=�6Z   5K      �� 5L   
   $	��Top=�6   $��10q�6-   $��Center=�69   .+�5N      �� $7��1Управление пользователями��6   $��24(�5O      $
��Bold"�5P      $��Center=�5Q      
�������$��	0,10,0,20q�5S      �� 5V      #��[  6   $��10q�68   ._�5W      &   System.Windows.Controls.DataGrid  -   5          Name$ UsersDataGrid��5W         AutoGenerateColumns .  ���	#FF4B4B4F���	#FF6A6A6D
�������   RowBackground �	#FF3F3F47   AlternatingRowBackground �	#FF2F2F33   ColumnHeaderStyle# [B 5^      $	��300��5_      $��Center=�5`      $	��Top=�5a         Columns 5b      0  *System.Windows.Controls.DataGridTextColumn  5c      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6T   1  +System.Windows.Controls.DataGridBoundColumn   Binding 69   �� *UserId+6T     5d      $ 
Логин��6-   $	 200 6Y    6<   �� *
Username+6Y     5e      $ Роль��6-   $	 150 6T    6;   �� *Role+6T     5f      $$ Данные для связи��6-   $	 200 6c    6G   �� *	Contact+6c   5h      5i      �� 5l      $ 
Horizontal=�6   $��Center=�62   $��10q�6O   .+�5m      �� -   5        $-��'Удалить пользователя��5m      $	��150��68   $��10q�6D   6P   �� 5n      -   5        $/��)Изменить пользователя��5n      $	��150��69   $��10q�6E   6Q   �� 5o      -   5        $1��+Редактирование товаров��5o      $	��150��6:   $��10q�6F   6R   5p      5q      5r      5s      