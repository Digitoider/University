   M S B A M L   `   `   `  ���� F  BSportSchool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:SportSchoolSportSchool  #    SportSchool.FindLearnerWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:SportSchool       � 
ResizeMode$  NoResize=� � Icon$_ YC:\Users\Alexander\Documents\Visual Studio 2015\Projects\SportSchool\SportSchool\icon.ico�� � Title$# Найти учащегося��$	��425��$	��450��.��� .+��� -    ��Name$ BG_jpg��$��-25,-25,-25,-25q�$��BG.jpg��$
��Fill=��� .+���  �� Orientation$ 
Horizontal=�$��5,5,0,10q�.+��� $	��130��.+��� $��Имя���� -    ��Name$ nameTB��$��5,0,5,0q��� $	��130��.+��� $��Фамилия���� -   $ 	surnameTB��$��5,0,5,0q��� $	��130��.+��� $��Отчество���� -   $ middleNameTB��$��5,0,5,0q��� -    ��Name$ searchButton��$��
Найти��$��5,5,5,0q��� $ 
Horizontal=�.+��� $ Vertical=�.+��� -    ��Name$ peopleLabel��$��Люди��$	��250��$��5,0,0,0q�$
��Left=�
����ح��� -    ��Name$ 	pepopleLB��$
��Left=�$��5,0,5,0q�$	��250��$	��256������ .R��� ���� *FIO+�� -   	 ��Name$	 informationSP��$ Vertical=�.+��� $��Информация��$	��150��$
��Left=�
����ح��� $��1q�
�������._��� $	��148��$	��254��.+��� $��Имя��$��15��$��5,0,0,0q��� -	   $ nameInformationTB��$��5,0,0,0q�$��Center=�$
��Left=�$	��105��$��20���� $��Фамилия��$��15��$��5,0,0,0q��� -
   $ surnameInformationTB��$��5,0,0,0q�$��Center=�$
��Left=�$	��105��$��20���� $��Отчество��$��15��$��5,0,0,0q��� -   $ middleNameInformationTB��$��5,0,0,0q�$��Center=�$
��Left=�$	��105��$��20���� -   $ birthdayLabel��$��Дата рождения��$��15��$��5,0,0,0q�(  "System.Windows.Controls.DatePicker  -   
  Name$
 
birthdayDP��$��5,0,0,0q��� $%��Телефонный номер��$��15��$��5,0,0,0q��� -   $" telephoneNumberInformationTB��$��5,0,0,0q�$��Center=�$
��Left=�$	��105��$��20���� $��Email��$��15��$��5,0,0,0q��� -   $ emailInformationTB��$��5,0,0,0q�$��Center=�$
��Left=�$	��105��$��20���� -   $" changePersonInDataBaseButton��$��Изменить��$	��105��$��5,5,0,0q�$
��Left=�