   M S B A M L   `   `   `  ���� F  BSportSchool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:SportSchoolSportSchool  "    SportSchool.InstructorWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:SportSchool       � 
ResizeMode$  NoResize=� � Icon$_ YC:\Users\Alexander\Documents\Visual Studio 2015\Projects\SportSchool\SportSchool\icon.ico�� � Title$* $ Вы вошли как тренер��$	��500��$	��350��.��� -    �Name$ gridka��.+��� -    ��Name$ BG_jpg��$��-25,-25,-25,-25q�$��BG.jpg��$
��Fill=��� $	��Top=�.+�o� $��Lucida Bright)�$��14(�$��20��.@�m� $��Соревнование��.@�m� -   $��Создать��m� -   $��Просмотреть��m� $��Журнал��.@�m� -    m�Name$ attendanceMI��$3��-Отметить присутствующих��m� -   $ showJournalMI��$��Показать��m� $��Справка��.@�m� $%��Показать справку���� -    ��Name$ attendanceField��$	��Top=�$��10,25,100,0q�.+�(  "System.Windows.Controls.DatePicker  -     Name$ 
datePicker��$
��Left=�$	��Top=��� $��
Время��$
��Left=�$	��Top=��� -	    ��Name$ timeCB��$
��Left=�$	��Top=�$	��120������ �� $$��Список Учащихся:��$
��Left=�$	��Top=��� -
   	 ��Name$	 
learnersLB��$	��224��$	��310��$
��Left=����� .R��� ���� *FIO+
 d� 	IsChecked
 �� *	IsChecked+�� -    ��Name$ 
saveButton��$��Сохранить��$��Center=�$
��Left=�$��92,5,0,0q�$	��Top=�$	��132���� -   $ competitionSP��$��10,25,50,10q�.+�  -   $ competitionDP��$��	0,0,150,0q��� $��Участники:���� -   $	 competitorsLB��$	��224��$	��150��$��0,0,0,0q�$
��Left=����� .R��� ���� *FIO+
 �� *	IsChecked+�� $��Описание:���� -    ��Name$ descriptionTB�� �� TextWrapping$
 Wrap=�$	��165��$	��270��$	��Top=�$
��Left=��� -   $ addCompetitionButton��$/��)Добавить соревеновние��$	��150��$��Right=�$��0,4,5,5q�