   M S B A M L   `   `   `  ����F  BSportSchool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:SportSchoolSportSchool  5   	   "    SportSchool.AddLearnerWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:SportSchool     5   	     � 
ResizeMode$  NoResize=�5   	    � Icon$_ YC:\Users\Alexander\Documents\Visual Studio 2015\Projects\SportSchool\SportSchool\icon.ico��5	   	    � Title$- 'Добавление Учащегося��5
   	   $	��425��6&   $	��410��63   .��5      � .+�5   
   �� -   5         ��Name$ BG_jpg��5      $��-25,-25,-25,-45q�6    $��BG.jpg��69   $
��Fill=�6I   �� 5   
   -   5         ��Name$ 
WarningLbl��5      $
��Auto��6$   $
��Auto��61   
���  ��5      �� 5   
   -   5        $ 
ValueAdded��5      $
��Auto��6$   $
��Auto��61   
��� � �$��130,335,0,0q�6R   �� 5   
   -   5         ��Name$ 
stackPanel��5      $
��Left=�6)   $��	10,10,0,0q�6D   .+�5      �� $#��Выберите секцию��6   $	��120��6.   $��20��6:   $��0,0,0,0q�6F   $��Center=�6X   �� 5      -   5         ��Name$ 	SectionCB��5      ��6*   �� �� 5      $��Имя��6   $	��120��6"   $��20��6.   $��0,0,0,0q�6:   $��Center=�6L   �� 5      -   5         ��Name$ NameTB��5      $	��120��6&   $��20��62   $��Center=�6>   �� 5      $��Фамилия��6   $	��120��6&   $��20��62   $��0,0,0,0q�6>   $��Center=�6P   �� 5      -   5        $ 	SurnameTB��5      $	��120��6)   $��20��65   $��Center=�6A   �� 5      $��Отчество��6   $	��120��6'   $��20��63   $��0,0,0,0q�6?   $��Center=�6Q   �� 5      -   5        $ MiddleNameTB��5      $	��120��6,   $��20��68   $��Center=�6D   �� 5      $��Дата рождения��6   $	��120��6,   $��20��68   $��0,0,0,0q�6D   $��Center=�6V   (  "System.Windows.Controls.DatePicker  5      -	   5          Name$ 
DatePicker��5      �� 5      $%��Телефонный номер��6   $	��120��6/   $��20��6;   $��0,0,0,0q�6G   $��Center=�6Y   �� 5       -
   5        $ TelephoneNumberTB��5       $	��120��61   $��20��6=   $��Center=�6I   �� 5!      $��Email��6   $	��120��6$   $��20��60   $��0,0,0,0q�6<   $��Center=�6N   �� 5"      -   5        $ EmailTB��5"      $	��120��6'   $��20��63   $��Center=�6?   �� 5#      $��Пароль��6   $	��120��6%   $��20��61   $��0,0,0,0q�6=   $��Center=�6O   �� 5$      -   5        $ 
PasswordTB��5$      $	��120��6*   $��20��66   $��Center=�6B   �� 5%      -   5        	 ��Name$	 AddBtn��5%      $��Добавить��6%   $��0,5,0,0q�68   6I   5&      5'      5(      