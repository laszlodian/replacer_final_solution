   M S B A M L   `   `   `  ����C  ?replacer, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:replacer.Viewreplacer.View  5   	   /clr-namespace:replacer.Modelreplacer.Model  5      "    replacer.View.ProgressWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:replacer.View     5   	   .modelclr-namespace:replacer.Model     5      -   5          �Name$  ProgressWindow1��5   	    � Title$ ProgressWindow��5	   	   $��154.434��6    $	��800��61    � WindowStartupLocation$ CenterScreen=�6=    � UseLayoutRounding .  � Topmost .  � 	Resources 5
      ,   &replacer.View.ProgressInformationModel   InfoText��%   &	          5   
   6=   5      .��5      �  � DataContext 6   ��  �� Source# [  .+�5   
   � -   5         �Name$ pbProgressWindow��5      $��Center=�60   $��25��6M   $��
10,0,10,59q�6Y   $	��772��6m   	 � IsIndeterminate	 . $��Center=�6�   $��Bottom=�6�   $��Center=�6�   �� 5   
   -   5        
 ��Name$
 	lbWorking��5      $��Program is working on...��6#   $
��Left=�6F   $��	55,10,0,0q�6a   $	��Top=�6t   $
��Bold"�6�   $��14(�6�   $	��440��6�   �� 5   
   -   5        $
 lbInfo��5      $��Stretch=�6n   $��	10,86,0,0q�6�   $	��Top=�6�   $
��Bold"�6�   $��14(�6�   $��32��6�   $��Center=�6�   ��69   �� # [  6�   5      5      