   M S B A M L   `   `   `  ����`  \ArcGIS.Desktop.Framework, Version=13.4.0.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad86wQclr-namespace:ArcGIS.Desktop.Framework.Controls;assembly=ArcGIS.Desktop.Framework!ArcGIS.Desktop.Framework.Controls  5   	   a ]ArcGIS.Desktop.Extensions, Version=13.4.0.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad86hJclr-namespace:ArcGIS.Desktop.Extensions;assembly=ArcGIS.Desktop.ExtensionsArcGIS.Desktop.Extensions 5   	    VETRO_Integration/   )VETRO_Integration.VETRO_IntegrationWindow   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   fcontrolsQclr-namespace:ArcGIS.Desktop.Framework.Controls;assembly=ArcGIS.Desktop.Framework     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   a
extensionsJclr-namespace:ArcGIS.Desktop.Extensions;assembly=ArcGIS.Desktop.Extensions     5   	     � Title$  VETRO Integration��5	   	   $	��300��6#   $	��300��60    � WindowStartupLocation$ CenterScreen=�5
   	   9   3ArcGIS.Desktop.Internal.Framework.Metro.MetroWindow   TitlebarFontSize$ 17Q�6.    � 	Resources 5       �� MergedDictionaries<  6ArcGIS.Desktop.Extensions.DesignOnlyResourceDictionary   Source�� 5   
    5        5      $S Mpack://application:,,,/ArcGIS.Desktop.Framework;component\Themes\Default.xaml?�6:   %    5      .��5      � $��	33,10,0,0q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6   �� 5      $
��Auto �6   $��22.66��6*   .+�5   
   �� $��0��6   $��Layer to Push to VETRO��6!   $��0,0,0,15q�6?   �� 5   
   -   5        $��mapLayersCombo��5       $��1��6    �� Text$! Select a layer from the map��66   $��19,10,48,26q�6Y   �� 5   
   $��2��6   $��	API Token��6!   $��0,10,0,1q�62   � 5    
   -   5        $��progressPopup��5       $��3��6   $��Center=�62   $	��300��6E   $	��100��6Q    � 	StaysOpen$
 True��6^   .��5!      �  5"      �� 5#      $
��Auto �6$   .+�5%      �� $��0��6   $
��Auto=�6,   .��5&      �� -   5        $��progressPopupText��5&       $��Pushing layer to VETRO...��69   $	��300��6Z   $��75��6f   $��10,0,0,0q�6r   $
��Wrap=�6�   5'      5(      5)      �� 5*   
   -   5        $��apiToken��5*      $��3��6   $��
0,10,22,13q�6/   $~��xeyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MTM4NDYsImlhdCI6MTcxMTU3NjczMH0.9FzLNWobpcNxdr_flQbX-59IBflY122ZidKfyOodELw��6C   �� 5+   
   -   5        $��emptyValsError��5+      $��5��6*   $;��5All fields are required. Please review empty entries.��67   $��0,0,0,15q�6t   $
��Bold"�6�   $��10(�6�   $��Center=�6�   $y�Hidden=�6�   	 �� TextEffects	 5-      z� 
 z� 
Foreground
 �	#FFF318186   ��5/      �� 50      52      �� 53   
   -   5        $��	apiSubmit��53   g   $��6��6   $��0,0,0,20q�6   $��Submit to VETRO��60   $��Center=�6J   6x   54      55      