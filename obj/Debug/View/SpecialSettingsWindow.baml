   M S B A M L   `   `   `  ����C  ?replacer, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:replacer.Viewreplacer.View  5   	   )    #replacer.View.SpecialSettingsWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:replacer.View     5   	   -   5          �Name$  specialSettingWindow��5   	    � Title$ Special Settings��5   	   $��217.807��6"   $	��800��63    � WindowStartupLocation$ CenterScreen=�6?    � WindowStyle$ ThreeDBorderWindow=�6d   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��16* �6   �� 5      $	��83* �6   .+�5   
   �� $I��CGive the  pattern of the text to look for at test-environment cell:��6   $
��Left=�6^   $��38��6y   $��0,36,0,0q�6�   $	��Top=�6�   $	��375��6�   $��2��6�   �� 5   
   -   5         ��Name$ 	tbPattern��5      $��Center=�6%   $��38��6B   $��217,42,15,0q�6N    �� TextWrapping$
 Wrap=�6c   $c��]Separator Character: {,} Robots: KMP omniMove 200  |  KMP omniMove 1500  |  KMP1200  | KMP600��6w   $	��Top=�6�   $	��432��6�   $��1��6   $��Center=�6   �� 	IsEnabled . $
��Bold"�6P  $��Arial Black)�6b  $��Center=�6{  �� 5   
   -   5         ��Name$
 btOK��5      $��OK��6   $��1��6,   $
��Left=�6<   $��513,107,0,0q�6W   $	��Top=�6l   $��75��6�   $
��Bold"�6�   6�   �� 5   
   -   5        $ btCancel��5      $��Cancel��6#   $��1��64   $
��Left=�6D   $��366,107,0,0q�6_   $	��Top=�6t   $��75��6�   $
��Bold"�6�   6�   �� 5   
   -   5        $ btHelp��5      $
��Help��6!   $��1��60   $
��Left=�6@   $��173,107,0,0q�6[   $	��Top=�6p   $��75��6�   $
��Bold"�6�   6�   5      5      