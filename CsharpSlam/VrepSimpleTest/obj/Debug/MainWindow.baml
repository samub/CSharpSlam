   M S B A M L   `   `   `  ����  Neobotix_mapping_VREP    CSharpSlam.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  V-REP simple test��5   	   $	��500��6#   $	��800��60    � DataContext 5   	   ��  �� RelativeSource �� *Self+.��5      � $��0,1,0,-1q�6   .+�5   
   �� $
��Left=�6   $	��200��60   .+�5	      �� -   5         ��Name$ ButtonConnect��5	   q   $��4q�6   $��Connect��6<   
���\\��$��30��6e   6q   5
      �� 5   
   -   5         ��Name$ StackControls��5   R   $
��Left=�6   $	��200��60   $��0, 36, 0, 0q�6<   .+�5      �� -   5        $ 	ButtonFwd��5   J   $��4q�6   $��Forward��68   6J   �� 5      -   5        $ 	ButtonBck��5   L   $��4q�6   $��Backward��69   6L   �� 5      -   5        $ 
ButtonLeft��5   H   $��4q�6   $
��Left��69   6H   �� 5      -   5        $ ButtonRight��5   J   $��4q�6   $��Right��6:   6J   �� 5      -   5        $ 
ButtonStop��5   I   $��4q�6   $
��Stop��6:   
���\\��6]   �� 5      -	   5        $ ButtonResetSim��5   Y   $��4q�6   $��Reset simulation��6>   
�������6q   �� 5      -
   5         ��Name$ cbRobotLayer��5      $��Robot Path Layer��6-   6H   �� 5      -   5        $ cbWallLayer��5      $��
Wall Layer��6,   6A   �� 5      -   5        $ cbEmptyLayer��5      $��Empty Layer��6-   6C   5      � 5   
   $��	205,0,0,0q�6   .+�5      �� -   5         ��Name$ 	ImageScan��5      $��
0,0,128,10q�6'   5      5      5      